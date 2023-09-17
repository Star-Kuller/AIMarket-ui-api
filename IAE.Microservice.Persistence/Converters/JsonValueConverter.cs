using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace IAE.Microservice.Persistence.Converters
{
    public class JsonValueConverter<T> : ValueConverter<T, string>
    {
        private static readonly JsonSerializerSettings _defaultSerializerSettings =
            new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

        public JsonValueConverter(ConverterMappingHints mappingHints = null)
                : base(model => JsonConvert.SerializeObject(model, _defaultSerializerSettings),
                       value => JsonConvert.DeserializeObject<T>(value, _defaultSerializerSettings),
                       mappingHints)
        {
        }
    }
}
