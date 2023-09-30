using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ComicShop.Domain.Features.Users;
using ComicShop.Infra.Structs;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ComicShop.Application.Features.Users
{
    public class UserCreate
    {
        public class Command : IRequest<Result<Exception, User>>
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
                    RuleFor(s => s.Email).EmailAddress().NotNull().NotEmpty().MaximumLength(255);
                    RuleFor(s => s.Password).NotNull().NotEmpty().MaximumLength(255);
                }
            }
        }

        public class Handler : IRequestHandler<Command, Result<Exception, User>>
        {
            private readonly IUserRepository _userRepository;
            private readonly ILogger<Handler> _logger;
            private readonly IMapper _mapper;

            public Handler(IUserRepository userRepository,
                ILogger<Handler> logger,
                IMapper mapper)
            {
                _userRepository = userRepository;
                _logger = logger;
                _mapper = mapper;
            }

            public async Task<Result<Exception, User>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = _mapper.Map<User>(request);

                var addCallback = _userRepository.Add(user);

                if (addCallback.IsFailure)
                {
                    _logger.LogError(addCallback.Failure, "An error occurred while trying to add user {userName} with email {userEmail}.", user.Name, user.Email);
                    return addCallback.Failure;
                }

                user = addCallback.Success;

                var saveChangesCallback = await _userRepository.SaveChangesAsync();

                if (saveChangesCallback.IsFailure)
                {
                    _logger.LogError(saveChangesCallback.Failure, "An error occurred while trying to save changes of user {userName} with email {userEmail}.", user.Name, user.Email);
                    return saveChangesCallback.Failure;
                }

                _logger.LogInformation("User {userName} created successfully.", user.Name);

                return addCallback;
            }
        }

    }
}
