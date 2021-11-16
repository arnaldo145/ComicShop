using System;
using System.Threading.Tasks;

namespace ComicShop.Domain.Features.Publishers
{
    public interface IPublisherRepository
    {
        Task<Guid> Add(Publisher publisher);
    }
}