using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ComicShop.Infra.Structs.Helpers;

namespace ComicShop.Infra.Structs
{
    public static class Result
    {
        public static Result<Exception, TSuccess> Run<TSuccess>(this Func<TSuccess> func)
        {
            try
            {
                return func();
            }
            catch (Exception e)
            {
                return e;
            }
        }

        public static Result<Exception, Unit> Run(this Action action) => Run(ToFunc(action));

        public static Result<Exception, TSuccess> Run<TSuccess>(this Exception ex) => ex;

        public static Result<Exception, IQueryable<TSuccess>> AsResult<TSuccess>(this IEnumerable<TSuccess> source) =>
            Result.Run(() => source.AsQueryable());

        public static async Task<Result<Exception, TSuccess>> Run<TSuccess>(Func<Task<TSuccess>> func)
        {
            try
            {
                return await func();
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}
