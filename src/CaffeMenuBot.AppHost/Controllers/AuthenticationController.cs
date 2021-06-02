using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CaffeMenuBot.AppHost.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CaffeMenuBot.AppHost.Models.DTO.Requests;
using CaffeMenuBot.AppHost.Models.DTO.Responses;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using CaffeMenuBot.Data.Models.Dashboard;
using Microsoft.AspNetCore.Hosting;
using CaffeMenuBot.AppHost.Helpers;
using CaffeMenuBot.Data;
using Microsoft.EntityFrameworkCore;

namespace CaffeMenuBot.AppHost.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly CaffeMenuBotContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<DashboardUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtHelper _jwtHelper;
        

        // used to save profile photos of user while adding new user or updating him
        private const string MEDIA_SUBFOLDER = "profile_photos";

        public AuthenticationController
            (
            CaffeMenuBotContext context,
            IWebHostEnvironment webHostEnvironment,
            UserManager<DashboardUser> userManager,
            RoleManager<IdentityRole> roleManager,
            JwtHelper jwtHelper
            )
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtHelper = jwtHelper;
        }

        
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        [SwaggerOperation("Authorizes the specified user.",
            Tags = new[] {"Authentication"})]
        [SwaggerResponse(200, "Successfully authorized the specified user.", typeof(AuthResponse))]
        [SwaggerResponse(400, "Bad request data, read the response body for more information.", typeof(ErrorResponse))]
        [SwaggerResponse(404, "User with the specified email was not found.", typeof(ErrorResponse))]
        [SwaggerResponse(500, "Internal server error.", typeof(ErrorResponse))]
        public async Task<ActionResult> Login([FromBody] UserLoginRequest user)
        {
            // check if the user with the same email exist
            var existingUser = await _userManager.FindByEmailAsync(user.Email);

            if (existingUser == null)
            {
                // We dont want to give to much information on why the request has failed for security reasons
                return BadRequest(new AuthResponse
                {
                    Result = false,
                    Errors = new List<string>
                    {
                        "User with the specified email was not found"
                    }
                });
            }
            

            // Now we need to check if the user has filled the right password
            var isCorrect = await _userManager.CheckPasswordAsync(existingUser, user.Password);

            if (isCorrect)
            {
                // include roles in user object
                _context.UserRoles.Include(r => r.Role).FirstOrDefault(r => r.UserId == existingUser.Id);

                var jwtToken = _jwtHelper.GenerateJwtToken(existingUser);

                return Ok(new AuthResponse
                {
                    User = new UserResponse
                    {
                        Id = existingUser.Id,
                        Email = existingUser.Email,
                        UserName = existingUser.UserName,
                        Roles = _jwtHelper.ConvertRolesToJwtFormat(existingUser.Roles),
                        ProfilePhotoUrl = $"{Startup.BaseImageUrl}/{MEDIA_SUBFOLDER}/{existingUser.ProfilePhotoFileName}"
                    },
                    Result = true,
                    Token = jwtToken
                });
            }

            // We dont want to give to much information on why the request has failed for security reasons
            return BadRequest(new AuthResponse
            {
                Result = false,
                Errors = new List<string>
                {
                    "Bad email or password, try again"
                }
            });
        }

        [HttpGet]
        [Route("me")]
        [Authorize]
        [SwaggerOperation("gets authenticated user object",
            Tags = new[] { "Authentication" })]
        [SwaggerResponse(200, "Successfully authorized user to get it's object", typeof(UserResponse))]
        [SwaggerResponse(400, "Bad request data, read the response body for more information.")]
        [SwaggerResponse(401, "User unathorized.")]
        [SwaggerResponse(500, "Internal server error.", typeof(ErrorResponse))]
        public async Task<ActionResult> Me()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            // include roles in user object
            _context.UserRoles.Include(r => r.Role).FirstOrDefault(r => r.UserId == user.Id);

            if (user == null)
                return Unauthorized();

            UserResponse response = new UserResponse()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Roles = _jwtHelper.ConvertRolesToJwtFormat(user.Roles),
                ProfilePhotoUrl = $"{Startup.BaseImageUrl}/{MEDIA_SUBFOLDER}/{user.ProfilePhotoFileName}"
            };

            return Ok(response);
        }
    }
}