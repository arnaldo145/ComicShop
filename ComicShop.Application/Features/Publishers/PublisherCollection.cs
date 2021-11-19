using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ComicShop.Domain.Features.Publishers;
using MediatR;

namespace ComicShop.Application.Features.Publishers
{
    public class PublisherCollection
    {
        public class Query : IRequest<IEnumerable<Publisher>> { }

        public class Handler : IRequestHandler<Query, IEnumerable<Publisher>>
        {
            private readonly IPublisherRepository _publisherRepository;

            public Handler(IPublisherRepository publisherRepository)
            {
                _publisherRepository = publisherRepository;
            }

            public async Task<IEnumerable<Publisher>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _publisherRepository.GetAllAsync();
            }
        }
    }
}
