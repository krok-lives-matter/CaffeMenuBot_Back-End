using System;
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
using System.Threading;
using System.IO;

namespace CaffeMenuBot.AppHost.Controllers
{
    [Route("api/userManagement")]
    [Authorize(Roles = "root")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private readonly CaffeMenuBotContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<DashboardUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtHelper _jwtHelper;
        

        // used to save profile photos of user while adding new user or updating him
        private const string MEDIA_SUBFOLDER = "profile_photos";

        public UserManagementController
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

        [HttpPut]
        [Route("updateOtherUser")]
        [SwaggerOperation("updates other user (root required)",
            Tags = new[] { "User Management" })]
        [SwaggerResponse(200, "Successfully authorized user to update it's object", typeof(UserResponse))]
        [SwaggerResponse(400, "Bad request data, read the response body for more information.", typeof(ErrorResponse))]
        [SwaggerResponse(401, "User unathorized.")]
        [SwaggerResponse(500, "Internal server error.", typeof(ErrorResponse))]
        public async Task<ActionResult> UpdateOtherUser([FromBody] UserUpdateRequest updatedUser)
        {
            var user = await _userManager.FindByIdAsync(updatedUser.Id);

            if(user == null)
            {
                return BadRequest(new ErrorResponse
                {
                    Result = false,
                    Errors = new List<string>
                    {
                        "User to update was not found, check user id that you are passing"
                    }
                });
            }

            // include roles in user object
            _context.UserRoles.Include(r => r.Role).FirstOrDefault(r => r.UserId == user.Id);

            if(updatedUser == null)
            {
                return BadRequest(new ErrorResponse
                {
                    Result = false,
                    Errors = new List<string>
                    {
                        "Information to update user was not provided"
                    }
                });
            }

            if(!String.IsNullOrEmpty(updatedUser.UserName))
            {
                user.UserName = updatedUser.UserName;
            }

            if(!String.IsNullOrEmpty(updatedUser.Roles))
            {
                var roles = _jwtHelper.ConvertJwtRolesToIdentity(updatedUser.Roles);
                await _jwtHelper.AssignRolesAsync(user, roles);
            }

            _context.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            await _context.SaveChangesAsync();

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

        [HttpPost("setOtherProfilePhoto")]
        [SwaggerOperation("Sets profile photo of other user. (root required)",
            Tags = new[] { "User Management" })]
        [SwaggerResponse(200, "Successfully set profile photo and returned a link to it.", typeof(CreateItemLinkResult))]
        [SwaggerResponse(400, "Bad request.")]
        public async Task<ActionResult<CreateItemLinkResult>> SetOtherProfilePhoto([FromForm] SetProfilePhotoRequest request,
            CancellationToken cancellationToken)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                {
                    return NotFound();
                }
                _context.UserRoles.Include(r => r.Role).FirstOrDefault(r => r.UserId == user.Id);

                user.ProfilePhotoFileName =
                    ImageHelper.SaveImage(new ImageModel
                    {
                        ContentType = request.File.ContentType,
                        ImageStream = request.File.OpenReadStream(),
                        FileExtension = Path.GetExtension(request.File.FileName)
                    }, _webHostEnvironment, MEDIA_SUBFOLDER);
                await _context.SaveChangesAsync(cancellationToken);

                return new CreateItemLinkResult
                {
                    ImageLink = $"{Startup.BaseImageUrl}/{MEDIA_SUBFOLDER}/{user.ProfilePhotoFileName}"
                };
            }
            return BadRequest(ModelState);
        }
    }
}