using System;
using System.Collections.Generic;

namespace ComicShop.Infra.Extensions
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Retorna a Exception e suas InnerExceptions caso existam.
        /// </summary>
        public static IEnumerable<Exception> GetInnerExceptions(this Exception exception)
        {
            do
            {
                yield return exception;
                exception = exception.InnerException;
            }
            while (exception != null);
        }
    }
}
