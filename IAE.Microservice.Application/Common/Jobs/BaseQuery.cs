using Hangfire;
using Newtonsoft.Json;

namespace IAE.Microservice.Application.Common.Jobs
{
    public class BaseQuery
    {
        [JsonIgnore]
        public IJobCancellationToken Token { get; set; } = JobCancellationToken.Null;

        [JsonIgnore]
        public EJob.Types JobType { get; set; } = EJob.Types.Recurring;

        public RecurringJobOptions RecurringOptions { get; set; } = new RecurringJobOptions();
        public string StartCron { get; set; } = Cron.Minutely();
    }
}