using System.Threading;
using System.Threading.Tasks;
using ComicShop.Application.Features.Users.DTOs;
using ComicShop.Application.Features.Users.Services;
using ComicShop.Domain.Exceptions;
using ComicShop.Domain.Features.Users;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

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

            public Handler(IUserRepository userRepository,
                IAuthService authService)
            {
                _userRepository = userRepository;
                _authService = authService;
            }

            public async Task<UserTokenDTO> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetByEmailAsync(request.Email);

                if (user == null)
                    throw new NotFoundException("User not found");

                if (!user.Password.Equals(request.Password))
                    throw new BadRequestException("Password not match");

                var tokenGenerated = _authService.GenerateToken(user);

                return new UserTokenDTO(tokenGenerated, user.Name);
            }
        }
    }
}
