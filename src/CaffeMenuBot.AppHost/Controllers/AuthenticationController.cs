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

namespace CaffeMenuBot.AppHost.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtOptions _jwtConfig;

        public AuthenticationController(UserManager<IdentityUser> userManager, IOptionsMonitor<JwtOptions> optionsMonitor)
        {
            _userManager = userManager;
            _jwtConfig = optionsMonitor.CurrentValue;
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest user)
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
                        "Invalid authentication request"
                    }
                });
            }

            // Now we need to check if the user has filled the right password
            var isCorrect = await _userManager.CheckPasswordAsync(existingUser, user.Password);

            if (isCorrect)
            {
                var jwtToken = GenerateJwtToken(existingUser);

                return Ok(new AuthResponse
                {
                    User = new UserResponse
                    {
                        Id = existingUser.Id,
                        Email = existingUser.NormalizedEmail,
                        UserName = existingUser.UserName
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
                    "Invalid authentication request"
                }
            });
        }


        [HttpPost]
        [Route("register")]
        [SwaggerOperation("Registers a new user.", "Administration rights are required.",
            Tags = new[] {"Authentication"})]
        [SwaggerResponse(200, "Successfully registered a new user.", typeof(AuthResponse))]
        [SwaggerResponse(400, "Bad request data, read the response body for more information.", typeof(ErrorResponse))]
        [SwaggerResponse(401, "Unauthorized.")]
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

            var newUser = new IdentityUser
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

        private string GenerateJwtToken(IdentityUser user)
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
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    // the JTI is used for our refresh token which we will be converting in the next video
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                // the life span of the token needs to be shorter and utilise refresh token to keep the user signedin
                // but since this is a demo app we can extend it to fit our current need
                Expires = DateTime.UtcNow.AddHours(6),
                // here we are adding the encryption algorithm information which will be used to decrypt our token
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }
    }

}