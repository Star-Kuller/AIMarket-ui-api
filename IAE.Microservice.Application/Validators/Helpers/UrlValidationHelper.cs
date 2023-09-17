using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace IAE.Microservice.Application.Validators.Helpers
{
    public static class UrlValidationHelper
    {
        private static readonly Regex DomainRegex =
            new Regex(DomainRegexString,
                RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        private static readonly IdnMapping IdnMapping = new IdnMapping();

        private const string DomainRegexString =
            @"^[0-9\p{L}-\.]{0,61}[0-9\p{L}]\.[0-9\p{L}][\p{L}-]*[0-9\p{L}]+$";

        public static bool IsValidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            var result = Uri.TryCreate(url, UriKind.Absolute, out Uri uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

            return !result ? false : IsValidDomain(uri.Host);
        }

        public static bool IsValidDomain(string domain)
        {
            if (string.IsNullOrWhiteSpace(domain))
                return false;

            if (domain.Contains("_"))
                return false;

            if (domain.StartsWith("-"))
                return false;

            domain = ConvertToUnicode(domain);

            return DomainRegex.IsMatch(domain);
        }

        private static string ConvertToUnicode(string input)
        {
            if (!input.Contains("xn--"))
                return input;

            return IdnMapping.GetUnicode(input);
        }
    }
}
