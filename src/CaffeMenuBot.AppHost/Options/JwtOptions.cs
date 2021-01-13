using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CaffeMenuBot.AppHost.Options
{
    public sealed class JwtOptions
    {
        public string SecretKey { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public string Issuer { get; set; } = null!;
    }
}
