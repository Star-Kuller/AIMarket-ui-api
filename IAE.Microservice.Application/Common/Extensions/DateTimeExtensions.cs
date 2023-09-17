using System;
using System.Globalization;
using IAE.Microservice.Domain.Entities.Users;

namespace IAE.Microservice.Application.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToStringByLanguage(this DateTime date, Language language)
        {
            return language == Language.English
                ? date.ToString("MMM dd,yy", CultureInfo.GetCultureInfo("en"))
                : date.ToString("dd MMM yy", CultureInfo.GetCultureInfo("ru"));
        }
    }
}