using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ComicShop.Domain.Features.Publishers;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Opw.HttpExceptions;

namespace ComicShop.Application.Features.Publishers
{
    public class PublisherUpdate
    {
        public class Command : IRequest<Publisher>
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Country { get; set; }

            public ValidationResult Validate()
            {
                return new Validator().Validate(this);
            }

            public class Validator : AbstractValidator<Command>
            {
                public Validator()
                {
                    RuleFor(x => x.Id).NotNull().NotEmpty();
                    RuleFor(s => s.Name).NotNull().NotEmpty().MaximumLength(255);
                }
            }
        }

        public class Handler : IRequestHandler<Command, Publisher>
        {
            private readonly IPublisherRepository _publisherRepository;
            private readonly ILogger<Handler> _logger;
            private readonly IMapper _mapper;

            public Handler(IPublisherRepository publisherRepository,
                ILogger<Handler> logger,
                IMapper mapper)
            {
                _publisherRepository = publisherRepository;
                _logger = logger;
                _mapper = mapper;
            }

            public async Task<Publisher> Handle(Command request, CancellationToken cancellationToken)
            {
                var publisher = await _publisherRepository.GetByIdAsync(request.Id);

                if (publisher is null)
                {

                    var notFoundException = new NotFoundException($"Unable to find publisher with id {request.Id}.");
                    _logger.LogError(notFoundException, "Unable to find publisher with id {publisherId}", request.Id);
                    throw notFoundException;
                }

                publisher.SetName(request.Name);
                publisher.SetCountry(request.Country);

                await _publisherRepository.UpdateAsync(publisher);

                return publisher;
            }
        }
    }
}
