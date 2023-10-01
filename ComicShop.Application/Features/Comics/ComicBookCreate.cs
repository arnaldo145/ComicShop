using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ComicShop.Domain.Features.Comics;
using ComicShop.Domain.Features.Publishers;
using ComicShop.Infra.Structs;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Opw.HttpExceptions;

namespace ComicShop.Application.Features.Comics
{
    public class ComicBookCreate
    {
        public class Command : IRequest<Result<Exception, Guid>>
        {
            public string Name { get; set; }
            public double Price { get; set; }
            public Guid PublisherId { get; set; }
            public int Amount { get; set; }
            public DateTime ReleaseDate { get; set; }

            public ValidationResult Validate()
            {
                return new Validator().Validate(this);
            }

            public class Validator : AbstractValidator<Command>
            {
                public Validator()
                {
                    RuleFor(s => s.Name).NotNull().NotEmpty().MaximumLength(255);
                    RuleFor(s => s.Price).GreaterThan(0);
                    RuleFor(s => s.PublisherId).NotNull().NotEmpty();
                    RuleFor(s => s.Amount).GreaterThan(0);
                }
            }
        }

        public class Handler : IRequestHandler<Command, Result<Exception, Guid>>
        {
            private readonly IComicBookRepository _comicBookRepository;
            private readonly IPublisherRepository _publisherRepository;
            private readonly ILogger<Handler> _logger;
            private readonly IMapper _mapper;

            public Handler(IComicBookRepository comicBookRepository,
                IPublisherRepository publisherRepository,
                ILogger<Handler> logger,
                IMapper mapper)
            {
                _comicBookRepository = comicBookRepository;
                _publisherRepository = publisherRepository;
                _logger = logger;
                _mapper = mapper;
            }

            public async Task<Result<Exception, Guid>> Handle(Command request, CancellationToken cancellationToken)
            {
                var comicBook = _mapper.Map<ComicBook>(request);

                var hasAnyPublisherCallback = await _publisherRepository.HasAnyByIdAsync(comicBook.PublisherId);

                if (hasAnyPublisherCallback.IsFailure)
                {
                    _logger.LogError(hasAnyPublisherCallback.Failure, "An error occurred while trying to check if has any publisher with id {comicBookPublisherId}.", comicBook.PublisherId);
                    return hasAnyPublisherCallback.Failure;
                }

                var hasAnyPublisher = hasAnyPublisherCallback.Success;

                if (!hasAnyPublisher)
                {
                    var badRequestException = new BadRequestException($"Unable to find publisher with id {comicBook.PublisherId}.");
                    _logger.LogError(badRequestException, "Unable to find publisher with id {comicBookPublisherId}", comicBook.PublisherId);
                    return badRequestException;
                }

                var hasAnyComicBookCallback = await _comicBookRepository.HasAnyAsync(comicBook.Name, comicBook.PublisherId);

                if (hasAnyComicBookCallback.IsFailure)
                {
                    _logger.LogError(hasAnyComicBookCallback.Failure, "An error occurred while trying to check if has any comic book with name {comicBookName} in publisher with id {comicBookPublisherId}.", comicBook.Name, comicBook.PublisherId);
                    return hasAnyComicBookCallback.Failure;
                }

                var hasAnyComicBook = hasAnyComicBookCallback.Success;

                if (hasAnyComicBook)
                {
                    var badRequestException = new BadRequestException("Already exists comic book with same name and same publisher.");
                    _logger.LogError(badRequestException, "Already exists comic book with same name ({comicBookName}) and same publisher ({publisherName}).", comicBook.Name, comicBook.Publisher.Name);
                    return badRequestException;
                }

                var addComicBookCallback = _comicBookRepository.Add(comicBook);

                if (addComicBookCallback.IsFailure)
                {
                    _logger.LogError(addComicBookCallback.Failure, "An error occurred while trying to add comic book with name {comicBookName}.", comicBook.Name);
                    return addComicBookCallback.Failure;
                }

                var saveChangesCallback = await _comicBookRepository.SaveChangesAsync();

                if (saveChangesCallback.IsFailure)
                {
                    _logger.LogError(saveChangesCallback.Failure, "An error ocurred while trying to save changes of comic book.");
                    return saveChangesCallback.Failure;
                }

                return addComicBookCallback.Success;
            }
        }
    }
}
