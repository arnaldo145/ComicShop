using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ComicShop.WebApi.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static void AddDependencies(this IServiceCollection services)
        {
            AddMediatr(services);
            AddAutoMapper(services);
        }

        private static void AddAutoMapper(this IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
        }

        private static void AddMediatr(this IServiceCollection services)
        {
            services.AddMediatR(typeof(Startup));
        }
    }
}
