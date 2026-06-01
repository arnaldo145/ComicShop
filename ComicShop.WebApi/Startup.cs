using System;
using System.Linq;
using System.Text;
using ComicShop.Application;
using ComicShop.Application.Features.Users.Services;
using ComicShop.Domain.Features.Users;
using ComicShop.Infra.Data.Contexts;
using ComicShop.WebApi.Extensions;
using ComicShop.WebApi.Options;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Opw.HttpExceptions;
using Opw.HttpExceptions.AspNetCore;

namespace ComicShop.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Conventions.ConfigureRouteConvention();
            })
                 .AddHttpExceptions(options =>
                 {
                     // This is the same as the default behavior; only include exception details in a development environment.
                     options.IncludeExceptionDetails = context => context.RequestServices.GetRequiredService<IWebHostEnvironment>().EnvironmentName == Environments.Development;
                     // This is a simplified version of the default behavior; only map exceptions for 4xx and 5xx responses.
                     options.IsExceptionResponse = context => (context.Response.StatusCode >= 400 && context.Response.StatusCode < 600);
                     // Only log the when it has a status code of 500 or higher, or when it not is a HttpException.
                     options.ShouldLogException = exception =>
                     {
                         if ((exception is HttpExceptionBase httpException && (int)httpException.StatusCode >= 500) || !(exception is HttpExceptionBase))
                             return true;
                         return false;
                     };
                 });

            services.AddDbContext<ComicShopCommonDbContext>(options =>
                   options.UseSqlServer(Configuration.GetConnectionString("ComicShopContext")));

                 services.Configure<BootstrapAdminOptions>(Configuration.GetSection(BootstrapAdminOptions.SectionName));

            services.AddDependencies();

            #region JWT Config

            var jwtSection = Configuration.GetSection(JwtOptions.SectionName);
            services.Configure<JwtOptions>(jwtSection);

            var jwtOptions = jwtSection.Get<JwtOptions>()
                ?? throw new System.InvalidOperationException($"Missing '{JwtOptions.SectionName}' configuration section.");

            if (string.IsNullOrWhiteSpace(jwtOptions.Secret))
                throw new System.InvalidOperationException($"'{JwtOptions.SectionName}:Secret' must be configured.");

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
                    ValidateIssuer = !string.IsNullOrWhiteSpace(jwtOptions.Issuer),
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = !string.IsNullOrWhiteSpace(jwtOptions.Audience),
                    ValidAudience = jwtOptions.Audience,
                    ValidateLifetime = true,
                    ClockSkew = System.TimeSpan.FromSeconds(30)
                };
            });

            #endregion

            services.ConfigureSwaggerServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpExceptions();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(c =>
            {
                c.SerializeAsV2 = true;
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ComicShop Web API");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            UpdateDatabase(app);
        }

        private static void UpdateDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using var context = serviceScope.ServiceProvider.GetRequiredService<ComicShopCommonDbContext>();
                context.Database.Migrate();

                BootstrapAdmin(serviceScope.ServiceProvider, context);
            }
        }

        private static void BootstrapAdmin(IServiceProvider serviceProvider, ComicShopCommonDbContext context)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<Startup>>();
            var passwordService = serviceProvider.GetRequiredService<IPasswordService>();
            var bootstrapOptions = serviceProvider.GetRequiredService<IOptions<BootstrapAdminOptions>>().Value;

            var adminUsers = context.Users.Where(user => user.Type == 2).ToList();
            var legacySeededAdmin = adminUsers.SingleOrDefault(user => user.Email == "admin@admin.com" && user.Name == "UserAdmin");

            if (!bootstrapOptions.IsConfigured)
            {
                if (legacySeededAdmin != null)
                {
                    context.Users.Remove(legacySeededAdmin);
                    context.SaveChanges();
                    logger.LogWarning("Removed legacy seeded admin because bootstrap admin credentials are not configured.");
                }

                return;
            }

            if (adminUsers.Count == 0)
            {
                var adminUser = new User
                {
                    Name = bootstrapOptions.Name,
                    Email = bootstrapOptions.Email.Trim(),
                    Type = 2
                };

                adminUser.Password = passwordService.HashPassword(adminUser, bootstrapOptions.Password);

                context.Users.Add(adminUser);
                context.SaveChanges();

                logger.LogInformation("Bootstrap admin user created successfully.");
                return;
            }

            if (legacySeededAdmin != null && adminUsers.Count == 1)
            {
                legacySeededAdmin.Name = bootstrapOptions.Name;
                legacySeededAdmin.Email = bootstrapOptions.Email.Trim();
                legacySeededAdmin.Password = passwordService.HashPassword(legacySeededAdmin, bootstrapOptions.Password);

                context.SaveChanges();

                logger.LogInformation("Legacy seeded admin updated from bootstrap configuration.");
            }
        }
    }
}
