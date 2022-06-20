using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ComicShop.Domain.Features.Comics;
using ComicShop.Infra.Data.Contexts;
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

        public async Task<Guid> AddAsync(ComicBook comicBook)
        {
            comicBook = _context.ComicBooks.Add(comicBook).Entity;

            await _context.SaveChangesAsync();

            return comicBook.Id;
        }

        public async Task<bool> HasAnyAsync(string comicBookName, Guid publisherId)
        {
            return await Task.Run(() => _context.ComicBooks.AnyAsync(c => c.Name.ToLower() == comicBookName.ToLower() && c.PublisherId == publisherId));
        }

        public async Task<IEnumerable<ComicBook>> GetAllAsNoTrackingAsync()
        {
            return await Task.Run(() => _context.ComicBooks
            .Include(c => c.Publisher)
            .AsNoTracking()
            .ToListAsync());
        }
    }
}
