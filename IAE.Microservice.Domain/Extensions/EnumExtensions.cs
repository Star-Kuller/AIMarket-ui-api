using System;
using System.ComponentModel;
using System.Linq;

namespace IAE.Microservice.Domain.Extensions
{
    public static class EnumExtensions
    {
        public static string Description<T>(this T value)
        {
            var memInfo = value.GetType().GetMember(value.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                var attributes = (DescriptionAttribute[])memInfo[0]
                    .GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes.Length > 0)
                    return attributes[0].Description;
            }

            return value.ToString();
        }

        public static TAttribute GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name)
                .GetCustomAttributes(false)
                .OfType<TAttribute>()
                .SingleOrDefault();
        }
    }
}
