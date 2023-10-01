using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ComicShop.Domain.Features.Publishers;
using ComicShop.Infra.Structs;
using MediatR;

namespace ComicShop.Application.Features.Publishers
{
    public class PublisherCollection
    {
        public class Query : IRequest<Result<Exception, IEnumerable<Publisher>>> { }

        public class Handler : IRequestHandler<Query, Result<Exception, IEnumerable<Publisher>>>
        {
            private readonly IPublisherRepository _publisherRepository;

            public Handler(IPublisherRepository publisherRepository)
            {
                _publisherRepository = publisherRepository;
            }

            public async Task<Result<Exception, IEnumerable<Publisher>>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _publisherRepository.GetAllAsync();
            }
        }
    }
}
