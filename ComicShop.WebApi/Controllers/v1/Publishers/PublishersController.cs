using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ComicShop.Application.Features.Publishers;
using ComicShop.WebApi.Controllers.v1.Publishers.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ComicShop.WebApi.Controllers.v1.Publishers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public PublishersController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] PublisherCreate.Command publisherCreateCommand)
        {
            var response = await _mediator.Send(publisherCreateCommand);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var response = await _mediator.Send(new PublisherCollection.Query());

            var publisherViewModelList = _mapper.Map<IEnumerable<PublisherResumeViewModel>>(response);

            return Ok(publisherViewModelList);
        }

    }
}
