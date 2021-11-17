using System.Collections.Generic;
using System.Threading.Tasks;
using ComicShop.Domain.Features.Publishers;
using MediatR;

namespace ComicShop.Application.Features.Publishers
{
    public class PublisherCollection
    {
        public class Query : IRequest<Task<IEnumerable<Publisher>>> { }

        public class Handler : RequestHandler<Query, Task<IEnumerable<Publisher>>>
        {
            private readonly IPublisherRepository _publisherRepository;

            public Handler(IPublisherRepository publisherRepository)
            {
                _publisherRepository = publisherRepository;
            }

            public Handler()
            {

            }

            protected override async Task<IEnumerable<Publisher>> Handle(Query request)
            {
                return await _publisherRepository.GetAllAsync();
            }
        }
    }
}
