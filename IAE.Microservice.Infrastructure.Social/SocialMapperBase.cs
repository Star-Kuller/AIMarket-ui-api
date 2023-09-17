using System.Xml;
using IAE.Microservice.Domain.Entities.Common;

namespace IAE.Microservice.Infrastructure.Social
{
    public abstract class SocialMapperBase
    {
        #region Add

        protected static int ToInt(Status status)
        {
            switch (status)
            {
                case Status.Active:
                    return 1;
                case Status.Banned:
                    return 2;
                case Status.Deleted:
                    return 2;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }

        protected static string ToRfc3339(DateTime? dateTime,
            XmlDateTimeSerializationMode dateTimeOption = XmlDateTimeSerializationMode.Utc)
        {
            return dateTime == null ? null : XmlConvert.ToString(dateTime.Value, dateTimeOption);
        }

        #endregion
    }
}