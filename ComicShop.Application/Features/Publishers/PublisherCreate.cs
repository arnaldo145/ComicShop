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
    public class PublisherCreate
    {
        public class Command : IRequest<Guid>
        {
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
                    RuleFor(s => s.Name).NotNull().NotEmpty().MaximumLength(255);
                    RuleFor(s => s.Country).NotNull().NotEmpty();
                }
            }
        }

        public class Handler : IRequestHandler<Command, Guid>
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

            public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
            {
                var publisher = _mapper.Map<Publisher>(request);

                var hasAnyCallback = _publisherRepository.HasAnyAsync(publisher.Name);

                var hasAny = hasAnyCallback.Result;

                if (hasAny)
                {
                    var badRequestException = new BadRequestException("There is already a registered publisher with the same name.");
                    _logger.LogError(badRequestException, "There is already a registered publisher with the same name {publisherName}.", publisher.Name);
                    throw badRequestException;
                }

                var addCallback = await _publisherRepository.AddAsync(publisher);

                return addCallback;
            }
        }
    }
}
