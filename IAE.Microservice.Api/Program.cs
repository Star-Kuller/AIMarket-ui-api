using IAE.Microservice.Application.Common.Jobs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using IAE.Microservice.Persistence;
using IAE.Microservice.Persistence.Ffprobe;
using Microsoft.Extensions.Hosting;

namespace IAE.Microservice.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await SetupFfprobeAndApplyDatabaseMigrationsAsync(host);

            host.Run();
        }

        private static async Task SetupFfprobeAndApplyDatabaseMigrationsAsync(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();

                try
                {
                    FfprobeInitializer.Initialize(false);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Adding {FfprobeInitializer.FfprobeFileName} error.");
                }

                try
                {
                    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Remote")
                    {
                        return;
                    }

                    await DatabaseInitializer.InitializeAsync(services);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Applying database migrations error.");
                }

                try
                {
                    await Scheduler.ResetSchedulerAsync(services.GetRequiredService<MicroserviceDbContext>());
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Initialization scheduler error.");
                }
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(serverOptions =>
                        {
                            serverOptions.Limits.MaxRequestBodySize = 62_914_560;
                            serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(1);
                        })
                        .UseWebRoot("wwwroot")
                        .UseStartup<Startup>();
                });
    }
}