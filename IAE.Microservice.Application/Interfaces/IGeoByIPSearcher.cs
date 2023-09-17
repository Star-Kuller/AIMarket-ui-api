namespace IAE.Microservice.Application.Interfaces
{
    public class GeoInfo
    {
        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
    }

    public enum GeoByIPSearcherLocale
    {
        En = 1,
        Ru = 2
    }

    public interface IGeoByIPSearcher
    {
        GeoInfo Search(string ipAddress, GeoByIPSearcherLocale locale);
    }
}
