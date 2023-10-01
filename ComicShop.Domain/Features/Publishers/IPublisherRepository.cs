using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ComicShop.Infra.Structs;

namespace ComicShop.Domain.Features.Publishers
{
    public interface IPublisherRepository
    {
        Result<Exception, Publisher> Add(Publisher publisher);
        Task<Result<Exception, IEnumerable<Publisher>>> GetAllAsync();
        Task<Result<Exception, bool>> HasAnyAsync(string publisherName);
        Task<Result<Exception, bool>> HasAnyByIdAsync(Guid publisherId);
        Task<Result<Exception, Publisher>> GetByIdAsync(Guid publisherId);
        Result<Exception, Unit> Update(Publisher publisher);
        Task<Result<Exception, Unit>> SaveChangesAsync();
    }
}
