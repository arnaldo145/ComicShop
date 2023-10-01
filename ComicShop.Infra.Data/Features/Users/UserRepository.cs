using System;
using System.Linq;
using System.Threading.Tasks;
using ComicShop.Domain.Features.Users;
using ComicShop.Infra.Data.Contexts;
using ComicShop.Infra.Structs;
using Microsoft.EntityFrameworkCore;

namespace ComicShop.Infra.Data.Features.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly ComicShopCommonDbContext _context;

        public UserRepository(ComicShopCommonDbContext context)
        {
            _context = context;
        }

        public Result<Exception, User> Add(User user)
        {
            return _context.Users.Add(user).Entity;
        }

        public async Task<Result<Exception, User>> GetByEmailAsync(string email)
        {
            return await Result.Run(() => _context.Users.SingleOrDefaultAsync(x => x.Email.Equals(email)));
        }
        public async Task<Result<Exception, Guid>> GetIdByEmailAsync(string email)
        {
            return await Result.Run(() => _context.Users
                .Where(x => x.Email.Equals(email))
                .Select(x => x.Id)
                .SingleOrDefaultAsync());
        }


        public async Task<Result<Exception, Unit>> SaveChangesAsync()
        {
            var callback = await Result.Run(() => _context.SaveChangesAsync());

            if (callback.IsFailure)
                return callback.Failure;

            return Unit.Successful;
        }
    }
}
