using System.Globalization;
using System.Linq;

namespace IAE.Microservice.Persistence.Extensions
{
    public static class StringExtensions
    {
        private static readonly Inflector.Inflector Inflector = new Inflector.Inflector(new CultureInfo("en-US"));

        public static string ToUnderscoreCase(this string str)
        {
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }

        public static string ToPluralizeUnderscoreCase(this string str)
        {
            return str.Pluralize().ToUnderscoreCase();
        }

        public static string Pluralize(this string value)
        {
            return Inflector.Pluralize(value);
        }
    }
}
