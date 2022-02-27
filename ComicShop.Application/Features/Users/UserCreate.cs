using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ComicShop.Domain.Features.Users;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace ComicShop.Application.Features.Users
{
    public class UserCreate
    {
        public class Command : IRequest<Guid>
        {
            public int Type { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }

            public ValidationResult Validate()
            {
                return new Validator().Validate(this);
            }

            public class Validator : AbstractValidator<Command>
            {
                public Validator()
                {
                    RuleFor(s => s.Type).GreaterThan(0);
                    RuleFor(s => s.Name).NotNull().NotEmpty().MaximumLength(255);
                    RuleFor(s => s.Email).NotNull().NotEmpty().MaximumLength(255);
                    RuleFor(s => s.Password).NotNull().NotEmpty().MaximumLength(255);
                }
            }
        }

        public class Handler : IRequestHandler<Command, Guid>
        {
            private readonly IUserRepository _userRepository;
            private readonly IMapper _mapper;

            public Handler(IUserRepository userRepository,
                IMapper mapper)
            {
                _userRepository = userRepository;
                _mapper = mapper;
            }

            public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = _mapper.Map<User>(request);

                var addCallback = await _userRepository.AddAsync(user);

                return addCallback;
            }
        }

    }
}
