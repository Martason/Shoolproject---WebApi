using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GeoComment.Models;
using Microsoft.IdentityModel.Tokens;

namespace GeoComment.Services
{
    public class JwtManager
    {
        private readonly IConfiguration _configuration;

        public JwtManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(User user)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["JwtConfig:Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    // Eventuella roller måste läggas till här
                }),
                // Hur länge token ska vara giltlig
                Expires = DateTime.UtcNow.AddHours(6),
                // Den hemliga nyckeln och vilken typ av kryptering som används
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            var jwtEncryptedToken = jwtTokenHandler.WriteToken(token);
            return jwtEncryptedToken;
        }
    }
}
