namespace IAE.Microservice.Application.Common.HClient
{
    public class HClientConfig
    {
        public string BaseUri { get; set; }

        public string AdminToken { get; set; }

        public bool IsCorrect => !string.IsNullOrWhiteSpace(BaseUri) && !string.IsNullOrWhiteSpace(AdminToken);
    }
}