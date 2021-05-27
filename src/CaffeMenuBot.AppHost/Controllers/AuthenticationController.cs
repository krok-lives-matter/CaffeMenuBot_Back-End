using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CaffeMenuBot.AppHost.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using CaffeMenuBot.AppHost.Options;
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
        private readonly JwtOptions _jwtConfig;

        // used to save profile photos of user while adding new user or updating him
        private const string MEDIA_SUBFOLDER = "profile_photos";

        public AuthenticationController
            (
            CaffeMenuBotContext context,
            IWebHostEnvironment webHostEnvironment,
            UserManager<DashboardUser> userManager,
            IOptionsMonitor<JwtOptions> optionsMonitor
            )
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _jwtConfig = optionsMonitor.CurrentValue;
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

            // add roles
            _context.UserRoles.Include(r => r.Role).FirstOrDefault(r => r.UserId == user.Id);

            if (user == null)
                return Unauthorized();

            UserResponse response = new UserResponse()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Roles = this.ConvertRolesToJwtFormat(user.Roles),
                ProfilePhotoUrl = $"{Startup.BaseImageUrl}/{MEDIA_SUBFOLDER}/{user.ProfilePhotoFileName}"
            };

            return Ok(response);
        }

        [HttpPut]
        [Route("me")]
        [Authorize]
        [SwaggerOperation("updates authenticated user object",
            Tags = new[] { "Authentication" })]
        [SwaggerResponse(200, "Successfully authorized user to update it's object", typeof(UserResponse))]
        [SwaggerResponse(400, "Bad request data, read the response body for more information.", typeof(ErrorResponse))]
        [SwaggerResponse(401, "User unathorized.")]
        [SwaggerResponse(500, "Internal server error.", typeof(ErrorResponse))]
        public async Task<ActionResult> Me([FromBody] UserUpdateRequest updatedUser)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            // add roles
            _context.UserRoles.Include(r => r.Role).FirstOrDefault(r => r.UserId == user.Id);

            if (user == null)
                return Unauthorized();

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

            /*if(updatedUser.ProfilePhoto != null)
            {
                string uniqueProfilePhotoFileName = 
                    ImageHelper.SaveImage(updatedUser.ProfilePhoto, _webHostEnvironment, MEDIA_SUBFOLDER);

                user.ProfilePhotoFileName = uniqueProfilePhotoFileName;
            }*/

            _context.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            await _context.SaveChangesAsync();

            UserResponse response = new UserResponse()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Roles = this.ConvertRolesToJwtFormat(user.Roles),
                ProfilePhotoUrl = $"{Startup.BaseImageUrl}/{MEDIA_SUBFOLDER}/{user.ProfilePhotoFileName}"
            };

            return Ok(response);
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
                // add roles
                _context.UserRoles.Include(r => r.Role).FirstOrDefault(r => r.UserId == existingUser.Id);

                var jwtToken = GenerateJwtToken(existingUser);

                return Ok(new AuthResponse
                {
                    User = new UserResponse
                    {
                        Id = existingUser.Id,
                        Email = existingUser.Email,
                        UserName = existingUser.UserName,
                        Roles = this.ConvertRolesToJwtFormat(existingUser.Roles),
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


        [HttpPost]
        [Route("register")]
        [Authorize(Roles = "admin")]
        [SwaggerOperation("Registers a new user.", "Administration rights are required.",
            Tags = new[] {"Authentication"})]
        [SwaggerResponse(200, "Successfully registered a new user.", typeof(AuthResponse))]
        [SwaggerResponse(400, "Bad request data, read the response body for more information.", typeof(ErrorResponse))]
        [SwaggerResponse(401, "User unathorized.")]
        [SwaggerResponse(403, "Role not allowed.")]
        [SwaggerResponse(403, "User with the specified email already exists.", typeof(ErrorResponse))]
        [SwaggerResponse(500, "Internal server error.", typeof(ErrorResponse))]
        public async Task<ActionResult> Register([FromBody] UserRegisterRequest user)
        {
            // check if the user with the same email exist
            var existingUser = await _userManager.FindByEmailAsync(user.Email) ??
                               await _userManager.FindByNameAsync(user.UserName);

            if (existingUser != null)
            {
                return BadRequest(new AuthResponse
                {
                    Result = false,
                    Errors = new List<string>
                    {
                        "User with the specified email or username already exists."
                    }
                });
            }

            var newUser = new DashboardUser
            {
                Email = user.Email,
                NormalizedEmail = user.NormalizedEmail ?? user.Email,
                UserName = user.UserName
            };
            var isCreated = await _userManager.CreateAsync(newUser, user.Password);
            if (isCreated.Succeeded)
            {
                var jwtToken = GenerateJwtToken(newUser);

                return Ok(new AuthResponse
                {
                    User = new UserResponse
                    {
                        Id = newUser.Id,
                        Email = newUser.NormalizedEmail,
                        UserName = newUser.UserName
                    },
                    Result = true,
                    Token = jwtToken
                });
            }
            
            return new JsonResult(new AuthResponse
            {
                Result = false,
                Errors = isCreated.Errors.Select(x => x.Description).ToList()
            }) {StatusCode = 500};
        }

        private string GenerateJwtToken(DashboardUser user)
        {
            // Now its time to define the jwt token which will be responsible of creating our tokens
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            // We get our secret from the appsettings
            var key = Encoding.ASCII.GetBytes(_jwtConfig.SecretKey);

            // we define our token descriptor
            // We need to utilise claims which are properties in our token which gives information about the token
            // which belong to the specific user who it belongs to
            // so it could contain their id, name, email the good part is that these information
            // are generated by our server and identity framework which is valid and trusted
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim("Username", user.UserName),
                    new Claim("Email", user.Email),
                    // the JTI is used for our refresh token which we will be converting in the next video
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())             
                }),
                // the life span of the token needs to be shorter and utilise refresh token to keep the user signedin
                // but since this is a demo app we can extend it to fit our current need
                Expires = DateTime.Now.AddHours(3),
                // here we are adding the encryption algorithm information which will be used to decrypt our token
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            // add roles
            _context.UserRoles.Include(r => r.Role).FirstOrDefault(r => r.UserId == user.Id);

            string? roles = this.ConvertRolesToJwtFormat(user.Roles);

            if(roles != null)
            {
                var claim = new Claim("Roles", roles);
                tokenDescriptor.Subject.AddClaim(claim);
            }
            
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }

        private string? ConvertRolesToJwtFormat(ICollection<DashboardUserRole> userRoles)
        {
            if(userRoles.Count == 0)
                return null;
            
            string roles = "";

                foreach (var role in userRoles)
                    roles += role.Role.Name + ",";

                // remove comma in the end
                roles = roles.Remove(roles.Length - 1, 1);
            
            return roles;
        }
    }

}