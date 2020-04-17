using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webApp_urlshortener.Models.jwt_validation
{
    public static class AuthenicationExtensions
    {
        public static TokenValidationParameters ToTokenValidationParameters(this Authentication authentication)
        {
            var result = new TokenValidationParameters()
            {
                ValidateAudience = authentication.JwtValidation.Options.ValidateAudience.Required,
                ValidAudiences = authentication.JwtValidation.Options.ValidateAudience.ValidAudiences,
                ValidateLifetime = authentication.JwtValidation.Options.ValidateLifetime.Required,
                ValidateIssuer = authentication.JwtValidation.Options.ValidateIssuer.Required,
                ValidIssuer = authentication.JwtValidation.Options.ValidateIssuer.ValidIssuer,
                RequireSignedTokens = authentication.JwtValidation.Options.ValidateIssuerSigningKey.Required,
                ValidateIssuerSigningKey = authentication.JwtValidation.Options.ValidateIssuerSigningKey.Required,
                ClockSkew = new TimeSpan(0, authentication.JwtValidation.Options.ValidateLifetime.ClockSkewMin, 0)
            };
            return result;
        }

    }
    public partial class Authentication
    {
        [JsonProperty("jwt-validation")]
        public JwtValidation JwtValidation { get; set; }
    }

    public partial class JwtValidation
    {
        [JsonProperty("authority")]
        public string Authority { get; set; }

        [JsonProperty("options")]
        public Options Options { get; set; }
    }

    public partial class Options
    {
        [JsonProperty("validateAudience")]
        public ValidateAudience ValidateAudience { get; set; }

        [JsonProperty("validateIssuer")]
        public ValidateIssuer ValidateIssuer { get; set; }

        [JsonProperty("validateLifetime")]
        public ValidateLifetime ValidateLifetime { get; set; }

        [JsonProperty("validateIssuerSigningKey")]
        public ValidateIssuerSigningKey ValidateIssuerSigningKey { get; set; }
    }

    public partial class ValidateAudience
    {
        [JsonProperty("required")]
        public bool Required { get; set; }

        [JsonProperty("validAudiences")]
        public List<string> ValidAudiences { get; set; }
    }

    public partial class ValidateIssuer
    {
        [JsonProperty("required")]
        public bool Required { get; set; }

        [JsonProperty("validIssuer")]
        public string ValidIssuer { get; set; }
    }

    public partial class ValidateIssuerSigningKey
    {
        [JsonProperty("required")]
        public bool Required { get; set; }
    }

    public partial class ValidateLifetime
    {
        [JsonProperty("required")]
        public bool Required { get; set; }

        [JsonProperty("clockSkew-min")]
        public int ClockSkewMin { get; set; }
    }
}
