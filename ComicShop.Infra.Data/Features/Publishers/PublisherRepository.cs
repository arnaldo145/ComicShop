using System;
using System.Threading.Tasks;
using ComicShop.Domain.Features.Publishers;
using ComicShop.Infra.Data.Contexts;

namespace ComicShop.Infra.Data.Features.Publishers
{
    public class PublisherRepository : IPublisherRepository
    {
        private readonly ComicShopCommonDbContext _context;

        public PublisherRepository(ComicShopCommonDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Add(Publisher publisher)
        {
            publisher = _context.Publishers.Add(publisher).Entity;

            await _context.SaveChangesAsync();

            return publisher.Id;
        }
    }
}