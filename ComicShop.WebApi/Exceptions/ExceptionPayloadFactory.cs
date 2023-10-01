using System;
using System.Linq;
using ComicShop.Infra.Extensions;
using Microsoft.Extensions.Logging;

namespace ComicShop.WebApi.Exceptions
{
    public class ExceptionPayloadFactory
    {
        private readonly ILogger<ExceptionPayloadFactory> _logger;

        public ExceptionPayloadFactory(ILogger<ExceptionPayloadFactory> logger) => _logger = logger;

        public ExceptionPayload Create(Exception exception)
        {
            var errorMessages = string.Empty;
            errorMessages = string.Join("Inner Exception ---> ", exception
                .GetInnerExceptions()
                .Select(ex => ex.Message));

            _logger.LogError(errorMessages, $"{exception.GetType().Name} thrown");

            return new ExceptionPayload(errorMessages);
        }

        public class ExceptionPayload
        {
            public string ErrorMessage { get; set; }

            public ExceptionPayload(string errorMessage) => ErrorMessage = errorMessage;
        }
    }
}
