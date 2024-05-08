using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace BAL.Repository
{
    public interface IJwtToken
    {

        public string generateJwtToken(string email, string role);

        public bool ValidateToken(string token, out JwtSecurityToken jwtSecurityToken);

    }
}
