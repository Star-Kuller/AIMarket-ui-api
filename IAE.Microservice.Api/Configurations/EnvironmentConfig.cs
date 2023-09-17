

namespace IAE.Microservice.Api.Configurations
{
    internal static class EnvironmentConfig
    {
        internal static bool HideExceptions { get; private set; }
        internal static int TtlInSec { get; private set; }

        internal static void Init(bool hideExceptions, int ttlInSec, bool isDmpOff, bool isDmpExApi2)
        {
            HideExceptions = hideExceptions;
            TtlInSec = ttlInSec;
        }
    }
}