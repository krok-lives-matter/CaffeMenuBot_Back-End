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
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

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

        [HttpDelete]
        [Route("users/{id:required}")]
        [SwaggerOperation("deletes user by provided id (root required)"
        , Tags = new[] {"User Management"})]
        [SwaggerResponse(200, "Successfully deleted dashboard user")]
        [SwaggerResponse(404, "User has not been found by provided id")]
        [SwaggerResponse(401, "User unathorized.")]
        [SwaggerResponse(403, "You're trying to delete the ROOT user. Action forbidden.")]
        [SwaggerResponse(500, "Internal server error.", typeof(ErrorResponse))]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if(user == null)
                return NotFound();

            if (user.Roles.Any(r => r.Role.Name.Equals("root", StringComparison.OrdinalIgnoreCase)))
            {
                return StatusCode(403);
            }

            await _userManager.DeleteAsync(user);

            return Ok();
        }
        
        [HttpGet]
        [Route("users")]
        [SwaggerOperation("gets all dashboard users (root required)",
         Tags = new[] {"User Management"})]
        [SwaggerResponse(200, "Successfully got all dashboard users", typeof(List<UserResponse>))]
        [SwaggerResponse(401, "User unathorized.")]
        [SwaggerResponse(500, "Internal server error.", typeof(ErrorResponse))]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetDashboardUsers()
        {
            var users = await _context.Users
                .AsNoTracking()
                .Include(u => u.Roles)
                .ToListAsync();

            return Ok(users.Select(u => new UserResponse
            {
                Email = u.Email,
                Id = u.Id,
                Roles = _jwtHelper.ConvertRolesToJwtFormat(u.Roles),
                UserName = u.UserName,
                ProfilePhotoUrl = $"{Startup.BaseImageUrl}/{MEDIA_SUBFOLDER}/{u.ProfilePhotoFileName}"
            }));
        }

        [HttpGet]
        [Route("roles")]
        [SwaggerOperation("gets all dashboard roles (root required)"
        , Tags = new[] {"User Management"})]
        [SwaggerResponse(200, "Successfully got all dashboard roles", typeof(RoleResponse))]
        [SwaggerResponse(401, "User unathorized.")]
        [SwaggerResponse(500, "Internal server error.", typeof(ErrorResponse))]
        public async Task<ActionResult<RoleResponse>> GetDashboardRoles()
        {
            var roles = await _context.Roles.AsNoTracking().ToListAsync();

            return new RoleResponse
            {
                Roles = roles.Select(r => r.Name).ToList()
            };
        }

        [HttpPost]
        [Route("resetOtherUserPassword")]
        [SwaggerOperation("resets other user password (root required), password should contain an uppercase character, lowercase character, a digit, and a non-alphanumeric character. Password must be at least six characters long.",
            Tags = new[] {"User Management"})]
        [SwaggerResponse(200, "Successfully reset user's password")]
        [SwaggerResponse(404, "Specified user was not found")]
        [SwaggerResponse(400, "Bad request data, read the response body for more information.", typeof(ErrorResponse))]
        [SwaggerResponse(401, "User unathorized.")]
        [SwaggerResponse(500, "Internal server error.", typeof(ErrorResponse))]
        public async Task<ActionResult>  ResetOtherUserPassword([FromBody] PasswordResetRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            
            if(user == null)
                return NotFound();

            // include roles in user object
            _context.UserRoles.Include(r => r.Role).FirstOrDefault(r => r.UserId == user.Id);

            // reset user password to new
            UserStore<DashboardUser> store = new(_context);
            string hashedNewPassword = _userManager.PasswordHasher.HashPassword(user, request.NewPassword);                    
            await store.SetPasswordHashAsync(user, hashedNewPassword);
            await store.UpdateAsync(user);

            return Ok();
        }


        [HttpPost]
        [Route("create")]
        [Authorize(Roles = "root")]
        [SwaggerOperation("Creates new user (root required), password should contain an uppercase character, lowercase character, a digit, and a non-alphanumeric character. Password must be at least six characters long.",
            Tags = new[] {"User Management"})]
        [SwaggerResponse(200, "Successfully registered a new user.", typeof(AuthResponse))]
        [SwaggerResponse(400, "Bad request data, read the response body for more information.", typeof(ErrorResponse))]
        [SwaggerResponse(401, "User unathorized.")]
        [SwaggerResponse(403, "Role not allowed.")]
        [SwaggerResponse(403, "User with the specified email already exists.", typeof(ErrorResponse))]
        [SwaggerResponse(500, "Internal server error.", typeof(ErrorResponse))]
        public async Task<ActionResult> CreateUser([FromBody] UserRegisterRequest user)
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

            if(user.Roles != null)
            {
                var roles = _jwtHelper.ConvertJwtRolesToIdentity(user.Roles);
                await _jwtHelper.AssignRolesAsync(newUser, roles);
            }
            
            if (isCreated.Succeeded)
            {
                var jwtToken = _jwtHelper.GenerateJwtToken(newUser);

                return Ok(new AuthResponse
                {
                    User = new UserResponse
                    {
                        Id = newUser.Id,
                        Email = newUser.Email,
                        UserName = newUser.UserName,
                        Roles = _jwtHelper.ConvertRolesToJwtFormat(newUser.Roles)
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