using Microsoft.Extensions.Logging;
using System;

namespace IAE.Microservice.Application.Common.Extensions
{
    public static class LoggerExtensions
    {
        public static void LogInfoWithTime(this ILogger logger, string message)
        {
            logger.LogInformation($"{DateTime.Now}: {message}");
        }

        public static void LogWarnWithTime(this ILogger logger, string message)
        {
            logger.LogWarning($"{DateTime.Now}: {message}");
        }

        public static void LogErrorWithTime(this ILogger logger, string message)
        {
            logger.LogError($"{DateTime.Now}: {message}");
        }

        public static void LogErrorWithTime(this ILogger logger, Exception exception)
        {
            logger.LogError(exception, $"{DateTime.Now}: {exception.Message}");
        }
    }
}
