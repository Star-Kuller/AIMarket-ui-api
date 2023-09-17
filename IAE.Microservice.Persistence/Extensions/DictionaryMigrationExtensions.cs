using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace IAE.Microservice.Persistence.Extensions
{
    public static class DictionaryMigrationExtensions
    {
        [Obsolete(
            "This method was used to remove seeded dictionaries data from migrations." +
            "It is now obsolete and its use is strictly prohibited.", true)]
        public static async Task<bool> IsRemoveDictionariesDataFromMigrationsAsync(
            this IHostingEnvironment webHostEnvironment)
        {
            IList<string> filePaths = null;

            var path = Path.Combine(webHostEnvironment.ContentRootPath,
                "..", "IAE.Microservice.Persistence", "Migrations");
            if (Directory.Exists(path))
            {
                filePaths = Directory.EnumerateFiles(path, "*_*.*", SearchOption.TopDirectoryOnly).ToList();
            }

            if (filePaths == null)
            {
                return false;
            }

            try
            {
                // Migration files
                await RemoveDictionariesDataFromMigrationsAsync(filePaths.Where(x => !x.Contains("Designer")),
                    new[] { "InsertData(", "UpdateData(", "DeleteData(" },
                    new[] { ");" },
                    line => line.Contains("table:", StringComparison.Ordinal) && new[]
                    {
                        "geos", "carriers", "browsers", "languages", "categories", "operating_systems", "manufacturers",
                        "positions", "devices", "countries", "timezones", "zip_codes", "designated_market_areas"
                    }.All(x => !line.Contains(x)),
                    new Dictionary<string, string>
                    {
                        {
                            "migrationBuilder.Sql(\"update users set country_id = 183,timezone_id = 234 where country_id is null OR timezone_id is null;\");",
                            "migrationBuilder.Sql(\"TRUNCATE users RESTART IDENTITY CASCADE;\");"
                        },
                        {
                            "migrationBuilder.Sql(\"update users set currency = 1 where currency is null;\");",
                            null
                        }
                    }, true);

                // Migration-Designer files
                await RemoveDictionariesDataFromMigrationsAsync(filePaths.Where(x => x.Contains("Designer")),
                    new[] { "HasData(" },
                    new[] { ");" },
                    line => new[]
                        {
                            "Name = \"Administrator\"", "FirstName = \"Admin\"", "UserId = -1L", "StatColumnId = 3L",
                            "Code = \"hour\""
                        }
                        .Any(x => line.Contains(x, StringComparison.Ordinal)), null, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
        }


        #region Help private methods

        private static async Task RemoveDictionariesDataFromMigrationsAsync(IEnumerable<string> migrationFilePaths,
            string[] startKeywords, string[] endKeywords, Func<string, bool> isPrematurelyEndByLine,
            Dictionary<string, string> replaceLineByLine = null, bool isCreateFilesWithRemovedLine = false)
        {
            foreach (var migrationFilePath in migrationFilePaths)
            {
                var lines = (await File.ReadAllLinesAsync(migrationFilePath)).ToList();
                var startAndEndIndexesToRemoveLines = new Dictionary<int, int?>();
                var isSearchStartIndex = true;

                for (var i = 0; i < lines.Count; i++)
                {
                    var line = lines[i];
                    if (replaceLineByLine != null && replaceLineByLine.Any())
                    {
                        foreach (var (key, value) in replaceLineByLine)
                        {
                            if (!line.Contains(key))
                            {
                                continue;
                            }

                            if (value == null)
                            {
                                lines[i] = string.Empty;
                                continue;
                            }

                            lines[i] = line.Replace(key, value);
                        }
                    }

                    if (isSearchStartIndex)
                    {
                        if (!startKeywords.Any(x => line.Contains(x)))
                        {
                            continue;
                        }

                        startAndEndIndexesToRemoveLines.Add(i, null);
                        isSearchStartIndex = false;
                    }
                    else
                    {
                        if (isPrematurelyEndByLine.Invoke(line))
                        {
                            startAndEndIndexesToRemoveLines.Remove(startAndEndIndexesToRemoveLines.Last().Key);
                            isSearchStartIndex = true;
                            continue;
                        }

                        if (!endKeywords.Any(x => line.Contains(x)))
                        {
                            continue;
                        }

                        startAndEndIndexesToRemoveLines[startAndEndIndexesToRemoveLines.Last().Key] = i;
                        isSearchStartIndex = true;
                    }
                }

                var removedLines = new List<string>();
                foreach (var (startIndex, endIndex) in startAndEndIndexesToRemoveLines.Reverse())
                {
                    if (!endIndex.HasValue)
                    {
                        continue;
                    }

                    if (isCreateFilesWithRemovedLine)
                    {
                        removedLines.AddRange(lines.GetRange(startIndex, endIndex.Value - startIndex + 1));
                    }

                    lines.RemoveRange(startIndex, endIndex.Value - startIndex + 1);
                    if (lines[startIndex + 1].All(char.IsWhiteSpace))
                    {
                        lines.RemoveAt(startIndex + 1);
                    }
                }

                if (removedLines.Any())
                {
                    var removedMigrationDirectoryPath = Path.Combine(Path.GetDirectoryName(migrationFilePath),
                        "Removed");
                    if (!Directory.Exists(removedMigrationDirectoryPath))
                    {
                        Directory.CreateDirectory(removedMigrationDirectoryPath);
                    }

                    var removedMigrationFilePath = Path.Combine(removedMigrationDirectoryPath,
                        Path.GetFileName(migrationFilePath));
                    await File.WriteAllLinesAsync(removedMigrationFilePath, removedLines);
                }

                await File.WriteAllLinesAsync(migrationFilePath, lines);
            }
        }

        #endregion
    }
}