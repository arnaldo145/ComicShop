using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ComicShop.Domain.Exceptions;
using ComicShop.Domain.Features.Comics;
using ComicShop.Domain.Features.Publishers;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

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
            private readonly IMapper _mapper;

            public Handler(IComicBookRepository comicBookRepository, IPublisherRepository publisherRepository, IMapper mapper)
            {
                _comicBookRepository = comicBookRepository;
                _publisherRepository = publisherRepository;
                _mapper = mapper;
            }

            public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
            {
                var comicBook = _mapper.Map<ComicBook>(request);

                var hasAnyPublisher = await _publisherRepository.HasAnyByIdAsync(comicBook.PublisherId);

                if (!hasAnyPublisher)
                    throw new BadRequestException($"Unable to find publisher with id {comicBook.PublisherId}.");

                var hasAnyComicBook = await _comicBookRepository.HasAnyAsync(comicBook.Name, comicBook.PublisherId);

                if (hasAnyComicBook)
                    throw new BadRequestException("Already exists comic book with same name and same publisher.");

                var addCallback = await _comicBookRepository.AddAsync(comicBook);

                return addCallback;
            }
        }
    }
}
