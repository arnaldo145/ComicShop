using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

                }
            }

        }

        public class Handler : IRequestHandler<Command, Guid>
        {
            public Handler()
            {

            }

            public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

    }
}
