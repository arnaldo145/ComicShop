using System.Threading;
using System.Threading.Tasks;
using ComicShop.Application.Features.Users.DTOs;
using ComicShop.Application.Features.Users.Services;
using ComicShop.Domain.Exceptions;
using ComicShop.Domain.Features.Users;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ComicShop.Application.Features.Users
{
    public class UserTokenCreate
    {
        public class Command : IRequest<UserTokenDTO>
        {
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
                    RuleFor(s => s.Email).NotNull().NotEmpty().MaximumLength(255);
                    RuleFor(s => s.Password).NotNull().NotEmpty().MaximumLength(50);
                }
            }
        }

        public class Handler : IRequestHandler<Command, UserTokenDTO>
        {
            private readonly IUserRepository _userRepository;
            private readonly IAuthService _authService;
            private readonly ILogger<Handler> _logger;

            public Handler(IUserRepository userRepository,
                IAuthService authService,
                ILogger<Handler> logger)
            {
                _userRepository = userRepository;
                _authService = authService;
                _logger = logger;
            }

            public async Task<UserTokenDTO> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetByEmailAsync(request.Email);

                if (user == null)
                {
                    var notFoundException = new NotFoundException("User not found");
                    _logger.LogError(notFoundException, "User {userEmail} not found", request.Email);
                    throw notFoundException;
                }

                if (!user.Password.Equals(request.Password))
                {
                    var badRequestException = new BadRequestException("Password not match");
                    _logger.LogError(badRequestException, "Password for user {userEmail} not match.", request.Email);
                    throw badRequestException;
                }

                var tokenGenerated = _authService.GenerateToken(user);

                _logger.LogInformation("Token for user {userEmail} generated successfully.", request.Email);

                return new UserTokenDTO(tokenGenerated, user.Name);
            }
        }
    }
}
