using System;
using System.Net;
using AutoMapper;
using ComicShop.Infra.Structs;
using ComicShop.WebApi.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opw.HttpExceptions;

namespace ComicShop.WebApi.Controllers.v1.Base
{
    public class ApiControllerBase : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ExceptionPayloadFactory _exceptionPayloadFactory;

        public ApiControllerBase(IMapper mapper, ExceptionPayloadFactory exceptionPayloadFactory)
        {
            _mapper = mapper;
            _exceptionPayloadFactory = exceptionPayloadFactory;
        }

        protected IActionResult HandleCommand<TFailure, TSuccess>
            (Result<TFailure, TSuccess> result) where TFailure : Exception
        {
            return result.IsFailure ? HandleFailure(result.Failure) : Ok(result.Success);
        }

        protected IActionResult HandleFailure<T>(T exceptionToHandle) where T : Exception
        {
            var exceptionPayload = _exceptionPayloadFactory.Create(exceptionToHandle);

            if (exceptionToHandle is NotFoundException)
                return NotFound(exceptionPayload);

            if (exceptionToHandle is BadRequestException)
                return StatusCode(StatusCodes.Status400BadRequest, exceptionPayload);

            return StatusCode(StatusCodes.Status500InternalServerError, exceptionPayload);
        }

        protected IActionResult HandleMappedCommand<TSource, TResult>(Result<Exception, TSource> result)
        {
            return result.IsSuccess ? Ok(_mapper.Map<TSource, TResult>(result.Success)) : HandleFailure(result.Failure);
        }
    }
}
