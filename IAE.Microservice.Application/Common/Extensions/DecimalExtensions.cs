using System;
using System.Globalization;

namespace IAE.Microservice.Application.Common.Extensions
{
    public static class DecimalExtensions
    {
        public static string ToFormattedString(this decimal number)
        {
            var intPartLength = Math.Round(number).ToString(CultureInfo.CurrentCulture).Length;

            var formatTemplate = string.Empty;

            var digitCount = intPartLength / 3;
            for (var i = 1; i <= digitCount; i++)
                formatTemplate += " 000";

            formatTemplate = new string('0', intPartLength % 3) + formatTemplate + ".00";

            return number.ToString(formatTemplate, CultureInfo.GetCultureInfo("ru"))
                .Replace(",00", "").Trim();
        }
    }
}