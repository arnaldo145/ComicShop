using System.Reflection;
using AutoMapper;
using ComicShop.Application;
using ComicShop.Application.Features.Identity.Services;
using ComicShop.Domain.Features.Identity;
using ComicShop.Domain.Features.Publishers;
using ComicShop.Infra.Data.Features.Identity;
using ComicShop.Infra.Data.Features.Publishers;
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

            AddPublisherFeature(services);
            AddAuthFeature(services);
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
            services.AddMediatR(typeof(AppModule).GetTypeInfo().Assembly);
        }

        private static void AddPublisherFeature(this IServiceCollection services)
        {
            services.AddScoped<IPublisherRepository, PublisherRepository>();
        }

        private static void AddAuthFeature(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthService, AuthService>();
        }
    }
}
