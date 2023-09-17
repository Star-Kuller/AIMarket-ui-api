namespace IAE.Microservice.Infrastructure
{
    public class SmtpManagement
    {
        public string Server { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
    }
}
