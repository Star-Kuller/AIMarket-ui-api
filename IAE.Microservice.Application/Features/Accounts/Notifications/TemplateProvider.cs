using IAE.Microservice.Domain.Entities.Users;
using System.IO;
using System.Reflection;
using System.Text;

namespace IAE.Microservice.Application.Features.Accounts.Notifications
{
    public class TemplateProvider
    {
        public static string GetCreateTemplate(Language language) => GetTemplate("create", language);
        public static string GetChangePasswordTemplate(Language language) => GetTemplate("change_password", language);
        public static string GetRecoveryPasswordTemplate(Language language) => GetTemplate("recovery_password", language);
        public static string GetRegisterTemplate(Language language) => GetTemplate("register", language);

        private static string GetTemplate(string resourceFileWithoutExt, Language language)
        {
            var lang = language == Language.Russian ? "ru" : "en";
            var resourceName = $"IAE.Microservice.Application.Features.Accounts.Notifications.Templates.{resourceFileWithoutExt}_{lang}.html";
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
