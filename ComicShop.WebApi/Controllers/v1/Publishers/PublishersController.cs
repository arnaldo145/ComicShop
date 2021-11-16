using System.Threading.Tasks;
using ComicShop.Application.Features.Publishers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ComicShop.WebApi.Controllers.v1.Publishers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PublishersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] PublisherCreate.Command publisherCreateCommand)
        {
            var response = await _mediator.Send(publisherCreateCommand);

            return Ok(response);
        }
    }
}
