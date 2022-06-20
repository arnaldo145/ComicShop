using System.Reflection;
using AutoMapper;
using ComicShop.Application;
using ComicShop.Application.Features.Users.Services;
using ComicShop.Domain.Features.Users;
using ComicShop.Domain.Features.Publishers;
using ComicShop.Infra.Data.Features.Users;
using ComicShop.Infra.Data.Features.Publishers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ComicShop.Domain.Features.Comics;
using ComicShop.Infra.Data.Features.Comics;

namespace ComicShop.WebApi.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static void AddDependencies(this IServiceCollection services)
        {
            AddMediatr(services);
            AddAutoMapper(services);

            AddPublisherFeature(services);
            AddComicBookFeature(services);
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

        private static void AddComicBookFeature(this IServiceCollection services)
        {
            services.AddScoped<IComicBookRepository, ComicBookRepository>();
        }

        private static void AddAuthFeature(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthService, AuthService>();
        }
    }
}
