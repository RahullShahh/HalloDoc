using BAL.Interfaces;
using DAL.DataContext;
using DAL.ViewModels;
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
    public class ResetPasswordServiceRepo : IResetPasswordService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly IRequestRepo _patient_Request;
        private readonly IJwtToken _jwtToken;
        private readonly IEmailService _emailService;
        public ResetPasswordServiceRepo(ApplicationDbContext context, IConfiguration config, IRequestRepo patient_Request, IJwtToken jwtToken, IEmailService emailService)
        {
            _context = context;
            _config = config;
            _patient_Request = patient_Request;
            _jwtToken = jwtToken;
            _emailService = emailService;
        }
        public string GenerateJWTTokenForPassword(string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("email", email) }),
                Expires = DateTime.UtcNow.AddHours(24),
                Issuer = _config["Jwt:Issuer"],
                //Audience = _audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);
            return jwtToken;
        }

        public string ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
            // 5. Verify the JWT token and allow the user to reset the password if the token is valid
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

            var jwtToken = (JwtSecurityToken)validatedToken;
            var email = jwtToken.Claims.First(x => x.Type == "email").Value;
            return email;
        }
    }
}
