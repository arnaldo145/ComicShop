using System;
using System.Threading.Tasks;

namespace ComicShop.Domain.Features.Users
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);

        Task<Guid> AddAsync(User user);
    }
}
