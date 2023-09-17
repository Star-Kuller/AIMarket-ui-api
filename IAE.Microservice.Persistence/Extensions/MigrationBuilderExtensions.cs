using System.Collections.Generic;
using System.Linq;
using IAE.Microservice.Domain.Entities.Common;
using IAE.Microservice.Domain.Entities.Common.Interfaces;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IAE.Microservice.Persistence.Extensions
{
    internal static class MigrationBuilderExtensions
    {
        public static void Drop(this MigrationBuilder migrationBuilder, string tableName)
        {
            migrationBuilder.Sql($"TRUNCATE {tableName} RESTART IDENTITY CASCADE;");
        }

        public static void InsertOrUpdate<TEntity>(this MigrationBuilder migrationBuilder,
            IEnumerable<TEntity> entities, string tableName, bool onConflictUpdate = false)
            where TEntity : Entity
        {
            foreach (var entity in entities)
            {
                var entityProps = entity.GetType().GetProperties()
                    .Where(x => x.SetMethod != null &&
                                (x.PropertyType == typeof(string) ||
                                 x.PropertyType == typeof(long) ||
                                 x.PropertyType == typeof(long?) ||
                                 x.PropertyType == typeof(bool)))
                    .Select(x => new KeyValuePair<string, string>(x.Name, x.GetValue(entity)?.ToString()))
                    .ToList();

                var sqlCommand = entityProps.GetInsertOrUpdateSqlCommand(tableName, onConflictUpdate);
                migrationBuilder.Sql(sqlCommand);
            }
        }


        #region Help private methods

        private static string GetInsertOrUpdateSqlCommand(this IList<KeyValuePair<string, string>> namesValues,
            string tableName, bool onConflictUpdate = false)
        {
            string query = null;
            query += "INSERT INTO " + tableName + " ( ";
            foreach (var (name, _) in namesValues)
            {
                query += "\"";
                query += name.ToUnderscoreCase();
                query += "\"";
                query += ", ";
            }

            query = query.Remove(query.Length - 2, 2);
            query += ") VALUES ( ";
            foreach (var (_, value) in namesValues)
            {
                if (value == null)
                {
                    query += "null";
                }
                else
                {
                    query += "'";
                    query += value.Replace("'", "''");
                    query += "'";
                }

                query += ", ";
            }

            query = query.Remove(query.Length - 2, 2);

            if (onConflictUpdate == false)
            {
                query += ") ON CONFLICT DO NOTHING;";
            }
            else
            {
                var idName = nameof(IId.Id).ToUnderscoreCase();
                query += $") ON CONFLICT ({idName}) DO UPDATE SET ";
                foreach (var (name, _) in namesValues)
                {
                    var uName = name.ToUnderscoreCase();
                    if (string.Equals(uName, idName))
                    {
                        continue;
                    }

                    query += $"{uName} = EXCLUDED.{uName}";
                    query += ", ";
                }

                query = query.Remove(query.Length - 2, 2);
                query += ";";
            }

            return query;
        }

        #endregion
    }
}