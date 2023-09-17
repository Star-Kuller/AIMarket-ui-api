using IAE.Microservice.Application.Interfaces;
using IAE.Microservice.Common;
using IAE.Microservice.Persistence.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IAE.Microservice.Domain.Entities.Common;
using IAE.Microservice.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IAE.Microservice.Persistence
{
    public class MicroserviceDbContext : IdentityDbContext<
        User, Role, long,
        UserClaim, UserRole, UserLogin,
        RoleClaim, UserToken>, IMicriserviceDbContext
    {

        public MicroserviceDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            SetupColumnNames(modelBuilder);
            SetupUtcDateTimes(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MicroserviceDbContext).Assembly);
            if (!Database.IsNpgsql())
            {
            }

            modelBuilder.UseSerialColumns();
            modelBuilder.Seed();
        }

        public override int SaveChanges()
        {
            SetupUpdatableDates();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            SetupUpdatableDates();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            SetupUpdatableDates();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            SetupUpdatableDates();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private static void SetupUtcDateTimes(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var properties = entityType
                    .GetProperties()
                    .ToList();

#pragma warning disable EF1001
                var entityTypeBuilder = new EntityTypeBuilder(entityType);
#pragma warning restore EF1001
                foreach (var property in properties)
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        entityTypeBuilder
                            .Property<DateTime>(property.Name)
                            .HasConversion(
                                v => v.ToUniversalTime(),
                                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        entityTypeBuilder
                            .Property<DateTime?>(property.Name)
                            .HasConversion(
                                v => v.HasValue ? v.Value.ToUniversalTime() : v,
                                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);
                    }
                }
            }
        }

        private static void SetupColumnNames(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // Replace table names
                entity.SetTableName(entity.GetTableName().ToUnderscoreCase());

                // Replace column names
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(property.Name.ToUnderscoreCase());
                }

                foreach (var key in entity.GetKeys())
                {
                    key.SetName(key.GetName().ToUnderscoreCase());
                }

                foreach (var key in entity.GetForeignKeys())
                {
                    key.SetConstraintName(key.GetConstraintName().ToUnderscoreCase());
                }

                foreach (var index in entity.GetIndexes())
                {
                    index.SetDatabaseName(index.GetDatabaseName().ToUnderscoreCase());
                }
            }
        }


        private void SetupUpdatableDates()
        {
            var entities = ChangeTracker
                .Entries()
                .Where(x => x.Entity is UpdatableEntity &&
                            (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                    ((UpdatableEntity)entity.Entity).CreatedAt = SystemTime.DateTime.UtcNow;
            }
        }
    }
}