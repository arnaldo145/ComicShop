using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComicShop.Domain.Features.Comics
{
    public interface IComicBookRepository
    {
        Task<Guid> AddAsync(ComicBook comicBook);
        Task<bool> HasAnyAsync(string comicBookName, Guid publisherId);
        Task<IEnumerable<ComicBook>> GetAllAsNoTrackingAsync();
    }
}
