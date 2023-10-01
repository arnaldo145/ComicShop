using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ComicShop.Domain.Features.Publishers;
using ComicShop.Infra.Structs;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Opw.HttpExceptions;

namespace ComicShop.Application.Features.Publishers
{
    public class PublisherCreate
    {
        public class Command : IRequest<Result<Exception, Guid>>
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

        public class Handler : IRequestHandler<Command, Result<Exception, Guid>>
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

            public async Task<Result<Exception, Guid>> Handle(Command request, CancellationToken cancellationToken)
            {
                var publisher = _mapper.Map<Publisher>(request);

                var hasAnyCallback = await _publisherRepository.HasAnyAsync(publisher.Name);

                if (hasAnyCallback.IsFailure)
                {
                    _logger.LogError(hasAnyCallback.Failure, "An error occurred while trying to check if has any publisher with name {publisherName}.", publisher.Name);
                    return hasAnyCallback.Failure;
                }

                var hasAny = hasAnyCallback.Success;

                if (hasAny)
                {
                    var badRequestException = new BadRequestException("There is already a registered publisher with the same name.");
                    _logger.LogError(badRequestException, "There is already a registered publisher with the same name {publisherName}.", publisher.Name);
                    return badRequestException;
                }

                var addCallback = _publisherRepository.Add(publisher);

                if (addCallback.IsFailure)
                {
                    _logger.LogError(addCallback.Failure, "An error occurred while trying to add publisher with name {publisherName}.", publisher.Name);
                    return addCallback.Failure;
                }

                var saveChangesCallback = await _publisherRepository.SaveChangesAsync();

                if (saveChangesCallback.IsFailure)
                {
                    _logger.LogError(saveChangesCallback.Failure, "An error occurred while trying to save changes of publisher with name {publisherName}.", publisher.Name);
                    return saveChangesCallback.Failure;
                }

                return addCallback.Success.Id;
            }
        }
    }
}
