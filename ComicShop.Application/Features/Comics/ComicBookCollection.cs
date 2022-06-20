using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ComicShop.Domain.Features.Comics;
using MediatR;

namespace ComicShop.Application.Features.Comics
{
    public class ComicBookCollection
    {
        public class Query : IRequest<IEnumerable<ComicBook>> { }

        public class Handler : IRequestHandler<Query, IEnumerable<ComicBook>>
        {
            private readonly IComicBookRepository _comicBookRepository;

            public Handler(IComicBookRepository comicBookRepository)
            {
                _comicBookRepository = comicBookRepository;
            }

            public async Task<IEnumerable<ComicBook>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _comicBookRepository.GetAllAsNoTrackingAsync();
            }
        }
    }
}
