using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ComicShop.Domain.Features.Users;
using ComicShop.Infra.Structs;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ComicShop.Application.Features.Users.Services
{
    public class AuthService : IAuthService
    {
        private readonly JwtOptions _jwtOptions;

        public AuthService(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions?.Value
                ?? throw new ArgumentNullException(nameof(jwtOptions));

            if (string.IsNullOrWhiteSpace(_jwtOptions.Secret))
                throw new InvalidOperationException("JWT secret is not configured.");
        }

        public Result<Exception, string> GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(_jwtOptions.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = string.IsNullOrWhiteSpace(_jwtOptions.Issuer) ? null : _jwtOptions.Issuer,
                Audience = string.IsNullOrWhiteSpace(_jwtOptions.Audience) ? null : _jwtOptions.Audience,
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Role, RoleFactory(user.Type))
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes),

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
