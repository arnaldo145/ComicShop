using System;
using System.Threading;
using System.Threading.Tasks;
using ComicShop.Application.Features.Identity.DTOs;
using ComicShop.Application.Features.Identity.Services;
using ComicShop.Domain.Features.Identity;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace ComicShop.Application.Features.Identity
{
    public class TokenCreate
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
                    RuleFor(s => s.Password).NotNull().NotEmpty().MaximumLength(30);
                }
            }
        }

        public class Handler : IRequestHandler<Command, UserTokenDTO>
        {
            private readonly IUserRepository _userRepository;
            private readonly IAuthService _authService;

            public Handler(IUserRepository userRepository,
                IAuthService authService)
            {
                _userRepository = userRepository;
                _authService = authService;
            }

            public Task<UserTokenDTO> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = _userRepository.GetByEmail(request.Email);

                if (user == null)
                    throw new Exception("User not found");

                if (!user.Password.Equals(request.Password))
                    throw new Exception("Password not match");

                var tokenGenerated = _authService.GenerateToken(user);

                return Task.Run(() => new UserTokenDTO(tokenGenerated, user.Name));
            }
        }
    }
}
