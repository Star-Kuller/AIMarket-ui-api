using IAE.Microservice.Persistence.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace IAE.Microservice.Persistence
{
    public class TradingDeskDbContextFactory : DesignTimeDbContextFactoryBase<TradingDeskDbContext>
    {
        protected override TradingDeskDbContext CreateNewInstance(DbContextOptions<TradingDeskDbContext> options)
        {
            return new TradingDeskDbContext(options);
        }
    }
}
