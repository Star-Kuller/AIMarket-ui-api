using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;
using IAE.Microservice.Application.Interfaces;

namespace IAE.Microservice.Application.Common.Jobs
{
    public static class Scheduler
    {
        public static void AddScheduler(this IServiceCollection services)
        {
            services.AddHangfire(c => c.UseMemoryStorage());
            services.AddHangfireServer();
        }

        public static void UseScheduler(this IApplicationBuilder app)
        {
            app.UseHangfireDashboard();
        }

        public static async Task ResetSchedulerAsync(IMicriserviceDbContext context)
        {

            await context.SaveChangesAsync(CancellationToken.None);
        }
    }
}