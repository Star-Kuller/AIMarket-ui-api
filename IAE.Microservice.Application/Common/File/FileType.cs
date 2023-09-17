using System;

namespace IAE.Microservice.Application.Common.File
{
    public enum FileType
    {
        Csv,
        Zip
    }

    public static class FileTypeExtensions
    {
        public static string GetContentType(this FileType type)
        {
            switch (type)
            {
                case FileType.Csv:
                    return "text/csv";
                case FileType.Zip:
                    return "application/zip";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static string GetExtension(this FileType type)
        {
            switch (type)
            {
                case FileType.Csv:
                    return ".csv";
                case FileType.Zip:
                    return ".zip";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}