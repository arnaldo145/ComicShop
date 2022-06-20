using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ComicShop.Domain.Features.Publishers;
using ComicShop.Infra.Data.Contexts;
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

        public async Task<Guid> AddAsync(Publisher publisher)
        {
            publisher = _context.Publishers.Add(publisher).Entity;

            await _context.SaveChangesAsync();

            return publisher.Id;
        }

        public async Task<IEnumerable<Publisher>> GetAllAsync()
        {
            return await Task.Run(() => _context.Publishers.ToListAsync());
        }

        public async Task<bool> HasAnyAsync(string publisherName)
        {
            return await Task.Run(() => _context.Publishers.AnyAsync(p => p.Name.ToLower() == publisherName.ToLower()));
        }

        public async Task<bool> HasAnyByIdAsync(Guid publisherId)
        {
            return await Task.Run(() => _context.Publishers.AnyAsync(p => p.Id == publisherId));
        }
    }
}
