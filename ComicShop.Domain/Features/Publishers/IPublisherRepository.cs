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
        Task<bool> HasAnyByIdAsync(Guid publisherId);
        Task<Publisher> GetByIdAsync(Guid publisherId);
        Task UpdateAsync(Publisher publisher);
    }
}
