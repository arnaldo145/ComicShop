using System.Text;
using ComicShop.Application;
using ComicShop.Infra.Data.Contexts;
using ComicShop.WebApi.Extensions;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

            services.AddDependencies();

            #region JWT Config

            //aqui vai nossa key secreta, o recomendado é guarda - la no arquivo de configuração
            var secretKey = "ZWRpw6fDo28gZW0gY29tcHV0YWRvcmE=";

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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false
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
                using var context = serviceScope.ServiceProvider.GetService<ComicShopCommonDbContext>();
                context.Database.Migrate();
            }
        }
    }
}
