using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ComicShop.Domain.Features.Publishers;
using ComicShop.Infra.Data.Contexts;
using ComicShop.Infra.Structs;
using Microsoft.EntityFrameworkCore;

namespace ComicShop.Infra.Data.Features.Publishers
{
    public class PublisherRepository : IPublisherRepository
    {
        private readonly ComicShopCommonDbContext _context;

        public PublisherRepository(ComicShopCommonDbContext context)
        {
            _context = context;
        }

        public Result<Exception, Publisher> Add(Publisher publisher)
        {
            publisher = _context.Publishers.Add(publisher).Entity;
            return publisher;
        }

        public async Task<Result<Exception, IEnumerable<Publisher>>> GetAllAsync()
        {
            return await Task.Run(() => _context.Publishers.ToListAsync());
        }

        public async Task<Result<Exception, bool>> HasAnyAsync(string publisherName)
        {
            return await Result.Run(() => _context.Publishers.AnyAsync(p => p.Name.ToLower() == publisherName.ToLower()));
        }

        public async Task<Result<Exception, bool>> HasAnyByIdAsync(Guid publisherId)
        {
            return await Result.Run(() => _context.Publishers.AnyAsync(p => p.Id == publisherId));
        }

        public async Task<Result<Exception, Publisher>> GetByIdAsync(Guid publisherId)
        {
            return await Result.Run(() => _context.Publishers.SingleOrDefaultAsync(p => p.Id == publisherId));
        }

        public Result<Exception, Unit> Update(Publisher publisher)
        {
            _context.Entry(publisher).State = EntityState.Modified;
            return Unit.Successful;
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
