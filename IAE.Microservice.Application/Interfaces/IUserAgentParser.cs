namespace IAE.Microservice.Application.Interfaces
{
    public class ClientInfo
    {
        public string Browser { get; set; }
        public string OperatingSystem { get; set; }
    }

    public interface IUserAgentParser
    {
        ClientInfo Parse(string userAgent);
    }
}
