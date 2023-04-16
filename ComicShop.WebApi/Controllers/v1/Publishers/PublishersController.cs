using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ComicShop.Application.Features.Publishers;
using ComicShop.Domain.Features.Publishers;
using ComicShop.WebApi.Controllers.v1.Base;
using ComicShop.WebApi.Controllers.v1.Publishers.ViewModels;
using ComicShop.WebApi.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ComicShop.WebApi.Controllers.v1.Publishers
{
    [Route("v1/[controller]")]
    [ApiController]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public class PublishersController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public PublishersController(IMediator mediator, IMapper mapper, ExceptionPayloadFactory exceptionPayloadFactory) : base(mapper, exceptionPayloadFactory)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles = "Default,Admin")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostAsync([FromBody] PublisherCreate.Command publisherCreateCommand)
        {
            var response = await _mediator.Send(publisherCreateCommand);

            return Created(string.Empty, response);
        }

        [HttpGet]
        [Authorize(Roles = "Default,Admin")]
        [ProducesResponseType(typeof(IEnumerable<PublisherResumeViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAsync()
        {
            var response = await _mediator.Send(new PublisherCollection.Query());

            return HandleMappedCommand<IEnumerable<Publisher>, IEnumerable<PublisherResumeViewModel>>(response);
        }

        [HttpPut]
        [Authorize(Roles = "Default,Admin")]
        [ProducesResponseType(typeof(PublisherResumeViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PutPublisherAsync([FromBody] PublisherUpdate.Command publisherUpdateCommand)
        {
            var response = await _mediator.Send(publisherUpdateCommand);

            return HandleMappedCommand<Publisher, PublisherResumeViewModel>(response);
        }
    }
}
