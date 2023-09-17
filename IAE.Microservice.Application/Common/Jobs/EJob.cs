namespace IAE.Microservice.Application.Common.Jobs
{
    public static class EJob
    {
        public static bool DisableManualStart = false;
        public static bool DisableScheduledStart = false;


        public enum Types
        {
            Recurring = 1,
            Background = 2
        }
    }
}