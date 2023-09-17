using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using IAE.Microservice.Persistence;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace IAE.Microservice.Application.Tests.Infrastructure
{
    public class TradingDeskContextFactory
    {
        public static TradingDeskDbContext Create()
        {
            return CreateInNpgsqlDatabase(TestsConfiguration.GetConnectionString());
        }

        public static void Destroy(TradingDeskDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }


        private static class TestsConfiguration
        {
            public static string GetConnectionString()
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                return configuration.GetConnectionString("TradingDeskTest");
            }
        }

        private static TradingDeskDbContext CreateInNpgsqlDatabase(string connectionString)
        {
            var databaseName = $"TD-TEST-{NUnit.Framework.TestContext.CurrentContext.Test.MethodName}-" +
                               $"{DateTime.UtcNow.Ticks}";
            var templateDatabaseName = TradingDeskContextTemplate.GetInstance(connectionString).DatabaseName;
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString)
            {
                Database = templateDatabaseName
            };

            using (var tmplConnection = new NpgsqlConnection(connectionStringBuilder.ConnectionString))
            {
                tmplConnection.Open();
                var raw =
                    $"CREATE DATABASE \"{databaseName}\" WITH TEMPLATE \"{templateDatabaseName}\"";
                using (var cmd = new NpgsqlCommand(raw, tmplConnection))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            connectionStringBuilder.Database = databaseName;

            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseNpgsql(connectionStringBuilder.ConnectionString);
            optionsBuilder.EnableSensitiveDataLogging();

            return new TradingDeskDbContext(optionsBuilder.Options);
        }
    }
}