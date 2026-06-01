using System;
using System.Threading;
using System.Threading.Tasks;
using ComicShop.Application.Features.Users.DTOs;
using ComicShop.Application.Features.Users.Services;
using ComicShop.Domain.Features.Users;
using ComicShop.Infra.Structs;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Opw.HttpExceptions;

namespace ComicShop.Application.Features.Users
{
    public class UserTokenCreate
    {
        public class Command : IRequest<Result<Exception, UserTokenDTO>>
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

        public class Handler : IRequestHandler<Command, Result<Exception, UserTokenDTO>>
        {
            private readonly IUserRepository _userRepository;
            private readonly IAuthService _authService;
            private readonly IPasswordService _passwordService;
            private readonly ILogger<Handler> _logger;

            public Handler(IUserRepository userRepository,
                IAuthService authService,
                IPasswordService passwordService,
                ILogger<Handler> logger)
            {
                _userRepository = userRepository;
                _authService = authService;
                _passwordService = passwordService;
                _logger = logger;
            }

            public async Task<Result<Exception, UserTokenDTO>> Handle(Command request, CancellationToken cancellationToken)
            {
                var userIdCallback = await _userRepository.GetByEmailAsync(request.Email);

                if (userIdCallback.IsFailure)
                {
                    _logger.LogError(userIdCallback.Failure, "An error occurred while trying to get user with email {userEmail}", request.Email);
                    return userIdCallback.Failure;
                }

                var user = userIdCallback.Success;

                if (user is null)
                {
                    var unauthorizedException = new UnauthorizedException("Invalid credentials.");
                    _logger.LogWarning("Invalid credentials for user {userEmail}", request.Email);
                    return unauthorizedException;
                }

                var passwordVerification = _passwordService.VerifyPassword(user, user.Password, request.Password);

                if (passwordVerification == PasswordVerificationStatus.Failed)
                {
                    var unauthorizedException = new UnauthorizedException("Invalid credentials.");
                    _logger.LogWarning("Invalid credentials for user {userEmail}", request.Email);
                    return unauthorizedException;
                }

                if (passwordVerification == PasswordVerificationStatus.SuccessRehashNeeded)
                {
                    user.Password = _passwordService.HashPassword(user, request.Password);

                    var saveChangesCallback = await _userRepository.SaveChangesAsync();

                    if (saveChangesCallback.IsFailure)
                    {
                        _logger.LogError(saveChangesCallback.Failure, "An error occurred while trying to upgrade password hash for user {userEmail}", request.Email);
                        return saveChangesCallback.Failure;
                    }
                }

                var tokenGenerated = _authService.GenerateToken(user);

                _logger.LogInformation("Token for user {userEmail} generated successfully.", request.Email);

                return new UserTokenDTO(tokenGenerated.Success, user.Name);
            }
        }
    }
}
