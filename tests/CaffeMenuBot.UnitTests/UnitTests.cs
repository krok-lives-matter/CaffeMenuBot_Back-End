using System.Linq;
using Xunit;
using CaffeMenuBot.Data.Models.Dashboard;
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Identity;
using CaffeMenuBot.AppHost.Helpers;
using Microsoft.Extensions.Options;
using CaffeMenuBot.AppHost.Options;
using Moq;

namespace CaffeMenuBot.UnitTests
{
    public sealed class UnitTests
    {
        [Fact]
        public void JwtHelper_NoCommaInTheEnd()
        {
            var options = new JwtOptions()
            {
                SecretKey = "",
                Audience = "",
                Issuer = ""
            };
            var monitor = Mock.Of<IOptionsMonitor<JwtOptions>>(_ => _.CurrentValue == options);

            var helper = new JwtHelper
            (
                null, null, null, monitor
            );

            var roles = new Collection<DashboardUserRole> 
            {
                new DashboardUserRole()
                {
                    Role = new IdentityRole()
                    {
                        Name = "testRole1"
                    }
                },
                new DashboardUserRole()
                {
                    Role = new IdentityRole()
                    {
                        Name = "testRole2"
                    }
                },
            };
            string jwtRoles = helper.ConvertRolesToJwtFormat(roles);

            Assert.False(jwtRoles.EndsWith(","));
        }

        [Fact]
        public void JwtHelper_jwtToRolesCountEquals()
        {
            string rolesJwt = "test1,test2,test";
            int count = rolesJwt.Split(",").Count();
            
            var options = new JwtOptions()
            {
                SecretKey = "",
                Audience = "",
                Issuer = ""
            };
            var monitor = Mock.Of<IOptionsMonitor<JwtOptions>>(_ => _.CurrentValue == options);

            var helper = new JwtHelper
            (
                null, null, null, monitor
            );

            var roles = helper.ConvertJwtRolesToIdentity(rolesJwt);
            Assert.Equal(count, roles.Count());
        }

        [Theory]
        [InlineData("echo", "hello world")]
        public void ShellHelper_ExecuteSuccess(string command, string parameter)
        {
            string result = $"{command} {parameter}".Bash().Trim();
            Assert.True(parameter == result);
        }
    }
}
