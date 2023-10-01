using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ComicShop.Domain.Features.Users;
using ComicShop.Infra.Structs;
using Microsoft.IdentityModel.Tokens;

namespace ComicShop.Application.Features.Users.Services
{
    public class AuthService : IAuthService
    {
        public Result<Exception, string> GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes("ZWRpw6fDo28gZW0gY29tcHV0YWRvcmE=");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Role, RoleFactory(user.Type))
                }),
                Expires = DateTime.UtcNow.AddHours(10),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private string RoleFactory(int roleNumber)
        {
            switch (roleNumber)
            {
                case 1:
                    return "Default";

                case 2:
                    return "Admin";

                default:
                    throw new Exception();
            }
        }
    }
}
