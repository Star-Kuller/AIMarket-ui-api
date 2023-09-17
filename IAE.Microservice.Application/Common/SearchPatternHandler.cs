using System.Linq;

namespace IAE.Microservice.Application.Common
{
    public static class SearchPatternHandler
    {
        public const string EscapeCharacter = "/";

        private static readonly string[] SpecialSymbols = { "_", "[", "]", "^" };

        public static string Handle(string source)
        {
            source = SpecialSymbols.Aggregate(source, (current, el) =>
                current.Replace(el, EscapeCharacter + el));
            return $"%{source}%";
        }
    }
}