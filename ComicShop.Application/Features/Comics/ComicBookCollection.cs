using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ComicShop.Domain.Features.Comics;
using ComicShop.Infra.Structs;
using MediatR;

namespace ComicShop.Application.Features.Comics
{
    public class ComicBookCollection
    {
        public class Query : IRequest<Result<Exception, IEnumerable<ComicBook>>> { }

        public class Handler : IRequestHandler<Query, Result<Exception, IEnumerable<ComicBook>>>
        {
            private readonly IComicBookRepository _comicBookRepository;

            public Handler(IComicBookRepository comicBookRepository)
            {
                _comicBookRepository = comicBookRepository;
            }

            public async Task<Result<Exception, IEnumerable<ComicBook>>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _comicBookRepository.GetAllAsync();
            }
        }
    }
}
