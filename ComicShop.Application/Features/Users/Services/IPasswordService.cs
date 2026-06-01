using ComicShop.Domain.Features.Users;

namespace ComicShop.Application.Features.Users.Services
{
    public enum PasswordVerificationStatus
    {
        Failed,
        Success,
        SuccessRehashNeeded
    }

    public interface IPasswordService
    {
        string HashPassword(User user, string password);

        PasswordVerificationStatus VerifyPassword(User user, string passwordHash, string providedPassword);
    }
}
