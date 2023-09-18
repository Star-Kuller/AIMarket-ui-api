using System.Net.Http;
using IAE.Microservice.Application.Common.HClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace IAE.Microservice.Infrastructure.Social.Client
{
    public partial class SocialClient : HClientBase, ISocialClient
    {
        private static string GetPath(params string[] segments) => SetupPath(segments);
        
        private const string ChatPath = "chat";
        private const string MessagePath = "message"; 
        private const string UserPath = "user";
        public SocialClient(HttpClient httpClient) : base(httpClient)
        {
        }

        protected override JsonSerializerSettings JsonSettings => new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include,
            DefaultValueHandling = DefaultValueHandling.Include,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
    }
}