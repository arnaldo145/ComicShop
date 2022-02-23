using ComicShop.Domain.Features.Identity;

namespace ComicShop.Application.Features.Identity.Services
{
    public interface IAuthService
    {
        string GenerateToken(User user);
    }
}
