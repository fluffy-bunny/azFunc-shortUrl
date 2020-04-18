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
        public static TokenValidationParameters ToTokenValidationParameters(this JwtValidation jwtValidation)
        {
            var result = new TokenValidationParameters()
            {
                ValidateAudience = jwtValidation.Options.Audience.Required,
                ValidAudiences = jwtValidation.Options.Audience.ValidAudiences,
                ValidateLifetime = jwtValidation.Options.Lifetime.Required,
                ClockSkew = new TimeSpan(0, jwtValidation.Options.Lifetime.ClockSkewMin, 0),
                ValidateIssuer = jwtValidation.Options.Issuer.Required,
                ValidIssuer = jwtValidation.Options.Issuer.ValidIssuer,
                RequireSignedTokens = jwtValidation.Options.SignedToken.Required,
                ValidateIssuerSigningKey = jwtValidation.Options.Issuer.ValidateIssuerSigningKey
            };
            return result;
        }

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
        [JsonProperty("audience")]
        public Audience Audience { get; set; }

        [JsonProperty("issuer")]
        public Issuer Issuer { get; set; }

        [JsonProperty("lifetime")]
        public Lifetime Lifetime { get; set; }

        [JsonProperty("signedToken")]
        public SignedToken SignedToken { get; set; }
    }

    public partial class Audience
    {
        [JsonProperty("required")]
        public bool Required { get; set; }

        [JsonProperty("validAudiences")]
        public string[] ValidAudiences { get; set; }
    }

    public partial class Issuer
    {
        [JsonProperty("required")]
        public bool Required { get; set; }

        [JsonProperty("validIssuer")]
        public string ValidIssuer { get; set; }

        [JsonProperty("validateIssuerSigningKey")]
        public bool ValidateIssuerSigningKey { get; set; }
        
    }

    public partial class Lifetime
    {
        [JsonProperty("required")]
        public bool Required { get; set; }

        [JsonProperty("clockSkew-min")]
        public int ClockSkewMin { get; set; }
    }

    public partial class SignedToken
    {
        [JsonProperty("required")]
        public bool Required { get; set; }
    }
}
