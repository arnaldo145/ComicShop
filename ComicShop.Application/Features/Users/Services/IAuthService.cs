using ComicShop.Domain.Features.Users;

namespace ComicShop.Application.Features.Users.Services
{
    public interface IAuthService
    {
        string GenerateToken(User user);
    }
}
