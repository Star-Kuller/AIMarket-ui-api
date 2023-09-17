using IAE.Microservice.Domain.Entities.Users;

namespace IAE.Microservice.Domain.Extensions
{
    public static class LanguageExtensions
    {
        public static string ToName(this Language language, string name, string nameRu)
        {
            switch (language)
            {
                case Language.English:
                    return name;
                case Language.Russian:
                    return string.IsNullOrEmpty(nameRu) ? name : nameRu;
                default:
                    return name;
            }
        }
    }
}