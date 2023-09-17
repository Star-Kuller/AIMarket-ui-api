using System;
using IAE.Microservice.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace IAE.Microservice.Application.Tests.Infrastructure
{
    public sealed class TradingDeskContextTemplate : IDisposable
    {
        private readonly MicroserviceDbContext _context;
        private static TradingDeskContextTemplate _instance;

        private TradingDeskContextTemplate(string connectionString)
        {
            DatabaseName = $"TD-TEST-TEMPLATE-{DateTime.UtcNow.Ticks}";
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString)
            {
                Database = DatabaseName
            };

            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseNpgsql(connectionStringBuilder.ConnectionString);

            _context = new MicroserviceDbContext(optionsBuilder.Options);
            _context.Database.EnsureCreated();

            AppDomain.CurrentDomain.ProcessExit += Destroy;
        }

        public string DatabaseName { get; }

        public static TradingDeskContextTemplate GetInstance(string connectionString)
            => _instance ??= new TradingDeskContextTemplate(connectionString);

        private void Destroy(object sender, EventArgs e)
        {
            Dispose();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
        }
    }
}