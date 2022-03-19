using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComicShop.Domain.Features.Publishers
{
    public interface IPublisherRepository
    {
        Task<Guid> AddAsync(Publisher publisher);
        Task<IEnumerable<Publisher>> GetAllAsync();
        Task<bool> HasAnyAsync(string publisherName);
    }
}
