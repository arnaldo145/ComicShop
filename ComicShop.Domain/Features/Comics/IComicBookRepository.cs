using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ComicShop.Infra.Structs;

namespace ComicShop.Domain.Features.Comics
{
    public interface IComicBookRepository
    {
        Result<Exception, Guid> Add(ComicBook comicBook);
        Task<Result<Exception, bool>> HasAnyAsync(string comicBookName, Guid publisherId);
        Task<Result<Exception, IEnumerable<ComicBook>>> GetAllAsync();
        Task<Result<Exception, Unit>> SaveChangesAsync();
    }
}
