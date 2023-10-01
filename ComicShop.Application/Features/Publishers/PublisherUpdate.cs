using System;
using System.Threading;
using System.Threading.Tasks;
using ComicShop.Domain.Features.Publishers;
using ComicShop.Infra.Structs;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Opw.HttpExceptions;

namespace ComicShop.Application.Features.Publishers
{
    public class PublisherUpdate
    {
        public class Command : IRequest<Result<Exception, Publisher>>
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

        public class Handler : IRequestHandler<Command, Result<Exception, Publisher>>
        {
            private readonly IPublisherRepository _publisherRepository;
            private readonly ILogger<Handler> _logger;

            public Handler(IPublisherRepository publisherRepository,
                ILogger<Handler> logger)
            {
                _publisherRepository = publisherRepository;
                _logger = logger;
            }

            public async Task<Result<Exception, Publisher>> Handle(Command request, CancellationToken cancellationToken)
            {
                var publisherCallback = await _publisherRepository.GetByIdAsync(request.Id);

                if (publisherCallback.IsFailure)
                {
                    _logger.LogError(publisherCallback.Failure, "An error occurred while trying to get publiser with id {publisherId}", request.Id);
                    return publisherCallback.Failure;
                }

                var publisher = publisherCallback.Success;

                if (publisher is null)
                {
                    var notFoundException = new NotFoundException($"Unable to find publisher with id {request.Id}.");
                    _logger.LogError(notFoundException, "Unable to find publisher with id {publisherId}", request.Id);
                    return notFoundException;
                }

                publisher.SetName(request.Name);
                publisher.SetCountry(request.Country);

                var updatePublisherCallback = _publisherRepository.Update(publisher);

                if (updatePublisherCallback.IsFailure)
                {
                    _logger.LogError(updatePublisherCallback.Failure, "An error occurred while trying to update publisher with id {publisherId}", publisher.Id);
                    return updatePublisherCallback.Failure;
                }

                var saveChangesCallback = await _publisherRepository.SaveChangesAsync();

                if (saveChangesCallback.IsFailure)
                {
                    _logger.LogError(saveChangesCallback.Failure, "An error occurred while trying to save changes of publisher with name {publisherName}", publisher.Name);
                    return saveChangesCallback.Failure;
                }

                return publisher;
            }
        }
    }
}
