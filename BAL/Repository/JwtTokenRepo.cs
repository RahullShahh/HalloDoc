using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repository
{
    public class JwtTokenServices : IJwtToken
    {
        private readonly IConfiguration _config;

        public JwtTokenServices(IConfiguration config)
        {
            _config = config;
        }

        public string generateJwtToken(string email, string role)
        {
            // Token Generation 

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("email", email), new Claim("Role", role) }),
                Expires = DateTime.UtcNow.AddHours(1), // Token expires in 1 hour
                Issuer = _config["Jwt:Issuer"],
                //Audience = _audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key)

                , SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);

            return jwtToken;
        }

        public bool ValidateToken(string token, out JwtSecurityToken jwtSecurityToken)
        {
            jwtSecurityToken = null;
            if (token == null)
            {
                return false;
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateAudience = false,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                jwtSecurityToken = (JwtSecurityToken)validatedToken;
                //var email = jwtSecurityToken.Claims.First(x => x.Type == "email").Value;
                if (jwtSecurityToken != null)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                return (false);
            }
            // 5. Verify the JWT token and allow the user to reset the password if the token is valid
        }
    }
}
