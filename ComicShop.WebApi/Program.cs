using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog.Sinks.Elasticsearch;
using Serilog;

namespace ComicShop.WebApi
{
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                var hostBuilder = CreateHostBuilder(args).Build();
                Serilog.Log.Information("Starting Web Host");
                hostBuilder.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Serilog.Log.Fatal(ex, "Host finished unexpectedly");
                return 1;
            }
            finally
            {
                Serilog.Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var settings = config.Build();
                    Serilog.Log.Logger = new LoggerConfiguration()
                        .Enrich.FromLogContext()
                        .WriteTo.Elasticsearch(
                            options:
                                new ElasticsearchSinkOptions(
                                    new Uri(settings["Elasticsearch:Uri"]))
                                {
                                    AutoRegisterTemplate = true,
                                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                                    IndexFormat = "comic-shop-api-{0:yyyy.MM}"
                                })
                        .WriteTo.Console()
                        .CreateLogger();
                })
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
