using IAE.Microservice.Persistence.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace IAE.Microservice.Persistence
{
    public class MicroserviceDbContextFactory : DesignTimeDbContextFactoryBase<MicroserviceDbContext>
    {
        protected override MicroserviceDbContext CreateNewInstance(DbContextOptions<MicroserviceDbContext> options)
        {
            return new MicroserviceDbContext(options);
        }
    }
}
