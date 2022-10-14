using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ComicShop.WebApi.Extensions
{
    public static class AppModelConventionsExtensions
    {
        public static void ConfigureRouteConvention(this IList<IApplicationModelConvention> actionModelConvention)
        {
            actionModelConvention.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
        }
    }

    public class SlugifyParameterTransformer : IOutboundParameterTransformer
    {
        public string TransformOutbound(object value)
        {
            if (value == null) { return null; }

            return Regex.Replace(value.ToString(), "([a-z])([A-Z])", "$1-$2").ToLower();
        }
    }
}
