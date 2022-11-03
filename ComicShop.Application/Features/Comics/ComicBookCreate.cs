using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ComicShop.Domain.Features.Comics;
using ComicShop.Domain.Features.Publishers;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Opw.HttpExceptions;

namespace ComicShop.Application.Features.Comics
{
    public class ComicBookCreate
    {
        public class Command : IRequest<Guid>
        {
            public string Name { get; set; }
            public string ReleaseYear { get; set; }
            public double Price { get; set; }
            public Guid PublisherId { get; set; }

            public ValidationResult Validate()
            {
                return new Validator().Validate(this);
            }

            public class Validator : AbstractValidator<Command>
            {
                public Validator()
                {
                    RuleFor(s => s.Name).NotNull().NotEmpty().MaximumLength(255);
                    RuleFor(s => s.ReleaseYear).NotNull().NotEmpty().MaximumLength(4);
                    RuleFor(s => s.Price).GreaterThan(0);
                    RuleFor(s => s.PublisherId).NotNull().NotEmpty();
                }
            }
        }

        public class Handler : IRequestHandler<Command, Guid>
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

            public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
            {
                var comicBook = _mapper.Map<ComicBook>(request);

                var hasAnyPublisher = await _publisherRepository.HasAnyByIdAsync(comicBook.PublisherId);

                if (!hasAnyPublisher)
                {

                    var badRequestException = new BadRequestException($"Unable to find publisher with id {comicBook.PublisherId}.");
                    _logger.LogError(badRequestException, "Unable to find publisher with id {comicBookPublisherId}", comicBook.PublisherId);
                    throw badRequestException;
                }

                var hasAnyComicBook = await _comicBookRepository.HasAnyAsync(comicBook.Name, comicBook.PublisherId);

                if (hasAnyComicBook)
                {
                    var badRequestException = new BadRequestException("Already exists comic book with same name and same publisher.");
                    _logger.LogError(badRequestException, "Already exists comic book with same name ({comicBookName}) and same publisher ({publisherName}).", comicBook.Name, comicBook.Publisher.Name);
                    throw badRequestException;
                }

                return await _comicBookRepository.AddAsync(comicBook);
            }
        }
    }
}
