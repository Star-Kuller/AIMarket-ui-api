using IAE.Microservice.Application.Interfaces;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Exceptions;
using MaxMind.GeoIP2.Model;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace IAE.Microservice.Infrastructure
{
    public class MaxmindGeoByIPSearcher : IGeoByIPSearcher, IDisposable
    {
        private bool _disposed;
        private Stream _stream;
        private DatabaseReader _reader;

        public MaxmindGeoByIPSearcher()
        {
            _stream = GetDbStream();
            _reader = new DatabaseReader(_stream, new[] { "en", "ru" });
        }

        public GeoInfo Search(string ipAddress, GeoByIPSearcherLocale locale)
        {
            try
            {
                var geo = _reader.City(ipAddress);
                var lang = locale.ToString().ToLower();
                return new GeoInfo
                {
                    Country = GetNameByLocale(geo.Country, lang),
                    Region = geo.Subdivisions.Any() ? GetNameByLocale(geo.Subdivisions[0], lang) : string.Empty,
                    City = GetNameByLocale(geo.City, lang)
                };
            }
            catch (AddressNotFoundException)
            {
                return new GeoInfo();
            }
        }

        private string GetNameByLocale(NamedEntity entity, string locale)
        {
            return entity.Names.ContainsKey(locale) ? entity.Names[locale] : entity.Name;
        }

        private static Stream GetDbStream()
        {
            var resourceName = $"IAE.Microservice.Infrastructure.Data.geocity.mmdb";
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream(resourceName);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _reader.Dispose();
                _stream.Dispose();
            }

            _disposed = true;
        }
    }
}
