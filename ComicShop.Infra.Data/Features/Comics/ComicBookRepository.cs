using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ComicShop.Domain.Features.Comics;
using ComicShop.Infra.Data.Contexts;
using ComicShop.Infra.Structs;
using Microsoft.EntityFrameworkCore;

namespace ComicShop.Infra.Data.Features.Comics
{
    public class ComicBookRepository : IComicBookRepository
    {
        private readonly ComicShopCommonDbContext _context;

        public ComicBookRepository(ComicShopCommonDbContext context)
        {
            _context = context;
        }

        public Result<Exception, Guid> Add(ComicBook comicBook)
        {
            comicBook = _context.ComicBooks.Add(comicBook).Entity;

            return comicBook.Id;
        }

        public async Task<Result<Exception, bool>> HasAnyAsync(string comicBookName, Guid publisherId)
        {
            return await Result.Run(() => _context.ComicBooks.AnyAsync(c => c.Name.ToLower() == comicBookName.ToLower() && c.PublisherId == publisherId));
        }

        public async Task<Result<Exception, IEnumerable<ComicBook>>> GetAllAsync()
        {
            return await Task.Run(() => _context.ComicBooks
                .Include(c => c.Publisher)
                .ToListAsync());
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
