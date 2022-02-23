using System.Threading.Tasks;
using ComicShop.Application.Features.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComicShop.WebApi.Controllers.v1.Identity
{
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("/v1/auth")]
        public async Task<IActionResult> AuthAsync([FromBody] TokenCreate.Command command)
        {
            var response = await _mediator.Send(command);

            return Ok(response);
        }
    }
}
