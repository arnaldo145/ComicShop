using System;
using System.Threading.Tasks;
using ComicShop.Infra.Structs;

namespace ComicShop.Domain.Features.Users
{
    public interface IUserRepository
    {
        Task<Result<Exception, User>> GetByEmailAsync(string email);

        Task<Result<Exception, Guid>> GetIdByEmailAsync(string email);

        Result<Exception, User> Add(User user);

        Task<Result<Exception, Unit>> SaveChangesAsync();
    }
}
