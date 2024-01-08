using Hospital.Database;
using Hospital.Services.Interfaces;
using Hospital.Database.TableModels;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Hospital.Services.Logic
{
    public class TokenService : ITokenService
    {
        private readonly AppDbContext _dbContext;
        private readonly HashSet<string> _bannedTokens;

        public TokenService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _bannedTokens = new HashSet<string>();
        }

        public void BanToken(string token)
        {
            _bannedTokens.Add(token);
        }

        public bool IsTokenBanned(string token)
        {
            return _bannedTokens.Contains(token);
        }

        public string GenerateToken(Doctor doctor)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("abcdefghijklmnopqrstuvwxyz");

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = "Hospital",
                Subject = new ClaimsIdentity(new []
                {
                    new Claim("id", doctor.Id.ToString())
                }),
                Audience = "audience"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
