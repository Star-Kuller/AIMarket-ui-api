using IAE.Microservice.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace IAE.Microservice.Persistence.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(GetRoles());
        }

        public static Role[] GetRoles()
        {
            return new[]
            {
                new Role
                {
                    Id = 1,
                    Name = Role.Administrator,
                    NormalizedName = Role.Administrator.ToUpper(),
                    NameRu = Role.AdministratorRu,
                    ConcurrencyStamp = "8451234d-9fff-4b45-9d91-5b2a48a23783"
                }
            };
        }
    }
}
