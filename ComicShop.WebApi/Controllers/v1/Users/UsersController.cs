using System.Threading.Tasks;
using ComicShop.Application.Features.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComicShop.WebApi.Controllers.v1.Users
{
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("/v1/generate-token")]
        public async Task<IActionResult> GenerateTokenAsync([FromBody] UserTokenCreate.Command command)
        {
            var response = await _mediator.Send(command);

            return Ok(response);
        }
    }
}
