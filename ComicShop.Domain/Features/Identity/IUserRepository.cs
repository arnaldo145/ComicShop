namespace ComicShop.Domain.Features.Identity
{
    public interface IUserRepository
    {
        User GetByEmail(string email);
    }
}
