using CaffeMenuBot.AppHost.Options;
using CaffeMenuBot.Data;
using System.Security.Claims;
using System.Text;
using CaffeMenuBot.Data.Models.Authentication;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;

namespace CaffeMenuBot.AppHost.Authentication
{
    public sealed class DatabaseBasedAuthService : IAuthService
    {
        private readonly CaffeMenuBotContext _dbContext;
        private readonly JwtOptions _jwtOptions; 

        public DatabaseBasedAuthService(CaffeMenuBotContext context, IOptionsSnapshot<JwtOptions> options)
        {
            _dbContext = context;
            _jwtOptions = options.Value;
        }

        public async ValueTask<ApplicationUser?> AuthenticateUserAsync(string email, string password, CancellationToken ct)
        {
            var user = await _dbContext.ApplicationUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email, ct);

            ct.ThrowIfCancellationRequested();

            if (user == null || user.PasswordHash != EncryptionProvider.Encrypt(password, EncryptionProvider.ReadSaltFromBase64(user.Salt)))
            {
                return null;
            }

            return user;
        }

        public string GenerateJwtToken(ApplicationUser userInfo)
        {
            var secretKey = _jwtOptions.SecretKey;
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Email),
                new Claim("role", userInfo.Role)
            };
            var token = new JwtSecurityToken(_jwtOptions.Issuer, _jwtOptions.Audience, claims,
                expires: DateTime.Now.AddMinutes(30d), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
