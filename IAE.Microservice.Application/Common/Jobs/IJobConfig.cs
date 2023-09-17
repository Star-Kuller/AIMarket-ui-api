namespace IAE.Microservice.Application.Common.Jobs
{
    public interface IJobConfig
    {
        string CompletedJobCronTime { get; set; }
        string NotCompletedJobCronTime { get; set; }
    }
}