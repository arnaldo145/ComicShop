using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ComicShop.Application.Features.Comics;
using ComicShop.WebApi.Controllers.v1.Comics.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ComicShop.WebApi.Controllers.v1.Comics
{
    [Route("v1/[controller]")]
    [ApiController]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public class ComicsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public ComicsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles = "Default,Admin")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostAsync([FromBody] ComicBookCreate.Command comicBookCreateCommand)
        {
            var response = await _mediator.Send(comicBookCreateCommand);

            return Created(string.Empty, response.Success);
        }

        [HttpGet]
        [Authorize(Roles = "Default,Admin")]
        [ProducesResponseType(typeof(IEnumerable<ComicBookResumeViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAsync()
        {
            var response = await _mediator.Send(new ComicBookCollection.Query());

            var comicBookViewModelList = _mapper.Map<IEnumerable<ComicBookResumeViewModel>>(response.Success);

            return Ok(comicBookViewModelList);
        }
    }
}
