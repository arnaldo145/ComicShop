using System.Threading.Tasks;
using AutoMapper;
using ComicShop.Application.Features.Users;
using ComicShop.WebApi.Controllers.v1.Base;
using ComicShop.WebApi.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComicShop.WebApi.Controllers.v1.Users
{
    public class UsersController : ApiControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator, ExceptionPayloadFactory exceptionPayloadFactory, IMapper mapper) : base(mapper, exceptionPayloadFactory)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("/v1/generate-token")]
        public async Task<IActionResult> GenerateTokenAsync([FromBody] UserTokenCreate.Command command)
        {
            var response = await _mediator.Send(command);

            return HandleCommand(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("/v1/users")]
        public async Task<IActionResult> CreateUserAsync([FromBody] UserCreate.Command command)
        {
            var response = await _mediator.Send(command);

            return HandleCommand(response);
        }
    }
}
