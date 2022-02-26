using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComicShop.Domain.Features.Users;
using ComicShop.Infra.Data.Contexts;

namespace ComicShop.Infra.Data.Features.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly ComicShopCommonDbContext _context;

        public UserRepository(ComicShopCommonDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await Task.Run(() => _context.Users.SingleOrDefault(x => x.Email.Equals(email)));
        }
    }
}
