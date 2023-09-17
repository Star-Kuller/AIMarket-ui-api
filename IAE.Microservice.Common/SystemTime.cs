using System;

namespace IAE.Microservice.Common
{
    public class SystemTime
    {
        private static Lazy<SystemTime> _lazyInstance =
            new Lazy<SystemTime>(() => new SystemTime());

        private SystemTime() { }

        public static SystemTime DateTime => _lazyInstance.Value;


        private Func<DateTime> _defaultCurrentFunction = () => System.DateTime.UtcNow;

        public DateTime UtcNow => SystemTimeContext.Current == null
                ? _defaultCurrentFunction.Invoke()
                : SystemTimeContext.Current.ContextDateTimeUtcNow;
    }
}
