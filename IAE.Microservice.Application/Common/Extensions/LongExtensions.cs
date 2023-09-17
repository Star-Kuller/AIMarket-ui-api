using System;

namespace IAE.Microservice.Application.Common.Extensions
{
    public static class LongExtensions
    {
        public static string ToFormattedBytes(this long sizeInBytes)
        {
            const int unit = 1024;
            if (sizeInBytes < unit)
            {
                return $"{sizeInBytes} B";
            }

            var exp = (int)(Math.Log(sizeInBytes) / Math.Log(unit));
            return $"{sizeInBytes / Math.Pow(unit, exp):###} {"KMGTPE"[exp - 1]}B";
        }
    }
}