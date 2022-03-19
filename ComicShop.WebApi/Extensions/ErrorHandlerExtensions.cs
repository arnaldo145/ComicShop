using System;
using System.Net;
using ComicShop.Domain.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ComicShop.WebApi.Extensions
{
    public static class ErrorHandlerExtensions
    {
        public static IApplicationBuilder UseErrorHandler(
                                     this IApplicationBuilder appBuilder)
        {
            return appBuilder.UseExceptionHandler(builder =>
            {
                builder.Run(async context =>
                {
                    var exceptionHandlerFeature = context
                                                    .Features
                                                    .Get<IExceptionHandlerFeature>();

                    if (exceptionHandlerFeature != null)
                    {
                        context.Response.StatusCode = (int)GetStatusCode(exceptionHandlerFeature.Error);
                        context.Response.ContentType = "application/json";

                        if (context.Response.StatusCode != (int)HttpStatusCode.InternalServerError)
                        {
                            var validationProblemDetails = new ValidationProblemDetails()
                            {
                                Status = (int)GetStatusCode(exceptionHandlerFeature.Error),
                                Title = GetStatusCodeTitle(exceptionHandlerFeature.Error),
                                Detail = exceptionHandlerFeature.Error.Message
                            };

                            await context.Response.WriteAsync(JsonConvert.SerializeObject(validationProblemDetails));
                        }
                        else
                        {
                            var problemDetails = new ProblemDetails
                            {
                                Status = (int)HttpStatusCode.InternalServerError,
                                Title = GetStatusCodeTitle(exceptionHandlerFeature.Error),
                                Detail = exceptionHandlerFeature.Error.Message
                            };

                            await context.Response.WriteAsync(JsonConvert.SerializeObject(problemDetails));
                        }
                    }
                });
            });
        }

        private static HttpStatusCode GetStatusCode(Exception exception)
        {
            if (exception is BadRequestException)
                return HttpStatusCode.BadRequest;

            if (exception is NotFoundException)
                return HttpStatusCode.NotFound;

            return HttpStatusCode.InternalServerError;
        }

        private static string GetStatusCodeTitle(Exception exception)
        {
            if (exception is BadRequestException)
                return "Bad Request";

            if (exception is NotFoundException)
                return "Not Found";

            return "Internal Error Server";
        }
    }
}
