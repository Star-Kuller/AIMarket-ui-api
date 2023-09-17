using IAE.Microservice.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IAE.Microservice.Persistence.Extensions
{
    public static class PropertyBuilderExtensions
    {
        public static PropertyBuilder<T> HasJsonValueConversion<T>(this PropertyBuilder<T> propertyBuilder) where T : class
        {
            propertyBuilder
              .HasConversion(new JsonValueConverter<T>())
              .Metadata.SetValueComparer(new JsonValueComparer<T>());

            return propertyBuilder;
        }
    }
}
