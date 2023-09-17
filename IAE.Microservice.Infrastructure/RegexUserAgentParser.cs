using IAE.Microservice.Application.Interfaces;
using System.IO;
using System.Reflection;
using System.Text;
using UAParser.FormFactor;

namespace IAE.Microservice.Infrastructure
{
    public class RegexUserAgentParser : IUserAgentParser
    {
        private readonly IUAParser _parser;

        public RegexUserAgentParser()
        {
            _parser = Parser.FromYaml(GetRegexesYaml());
        }

        public ClientInfo Parse(string userAgent)
        {
            if (string.IsNullOrWhiteSpace(userAgent)) return new ClientInfo();

            var client = _parser.Parse(userAgent);
            return new ClientInfo
            {
                OperatingSystem = client.OS.ToString(),
                Browser = client.UA.ToString()
            };
        }

        private static string GetRegexesYaml()
        {
            var resourceName = $"IAE.Microservice.Infrastructure.Data.regexes.yaml";
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
