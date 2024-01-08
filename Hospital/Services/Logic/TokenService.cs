using Hospital.Database;
using Hospital.Services.Interfaces;
using Hospital.Database.TableModels;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Hospital.Services.Logic
{
    public class TokenService : ITokenService
    {
        private readonly AppDbContext _dbContext;

        public TokenService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void BanToken(string token)
        {
            _dbContext.Add(new BannedToken
            {
                Id = GetTokenId(token),
                AddedAt = DateTime.UtcNow
            });
            _dbContext.SaveChanges();
        }

        private bool IsTokenBanned(string token)
        {
            var tokenId = GetTokenId(token);
            return _dbContext.BannedTokens
                .FirstOrDefault(x => x.Id == tokenId) != null;
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
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, doctor.Id.ToString()),
                    new Claim(ClaimTypes.Authentication, Guid.NewGuid().ToString())
                }),
                Audience = "audience"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private Guid GetTokenId(string token)
        {
            if (token == null || token == "")
            {
                throw new UnauthorizedAccessException();
            }

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
            var tokenS = jsonToken as JwtSecurityToken;

            return Guid.Parse(tokenS.Claims.First(claim => claim.Type == ClaimTypes.Authentication).Value);
        }

        public void ValidateToken(string token)
        {
            if (IsTokenBanned(token))
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}
