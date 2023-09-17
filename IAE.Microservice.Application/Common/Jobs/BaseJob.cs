using Hangfire;
using IAE.Microservice.Application.Common.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using IAE.Microservice.Application.Common.Operation;

namespace IAE.Microservice.Application.Common.Jobs
{
    public abstract class BaseJob<TI, TQ, TResult> : IJob<TQ>
        where TI : IJob<TQ>
        where TQ : BaseQuery, new()
    {
        private static string JobId => typeof(TI).Name;

        protected readonly IJobConfig Config;
        protected readonly ILogger<TI> Logger;

        protected BaseJob(IJobConfig config, ILogger<TI> logger)
        {
            Config = config;
            Logger = logger;
        }

        protected abstract Func<TQ, CancellationToken, Task<OperationResult<TResult>>> CallbackAsync { get; }

        public virtual async Task RunAsync(TQ query, EJob.Types jobType, bool isJobDisabled = false,
            CancellationToken token = default)
        {
            if (isJobDisabled) return;
            query.JobType = jobType;
            query.Token?.ThrowIfCancellationRequested();
            Logger.LogInfoWithTime(Messages.StartsJob);
            var result = await CallbackAsync(query, token);
            if (!result.Succeeded)
            {
                if (query.JobType == EJob.Types.Recurring) UpdateRecurring(query, Config.NotCompletedJobCronTime);
                Logger.LogWarnWithTime(result.ToString());
                return;
            }

            if (query.JobType == EJob.Types.Recurring) UpdateRecurring(query, Config.CompletedJobCronTime);
            Logger.LogInfoWithTime(Messages.CompletedJob);
        }

        public static void RunBackground(TQ query = default)
        {
            query = query ?? new TQ();
            query.JobType = EJob.Types.Background;
            BackgroundJob.Enqueue(MethodCall(query, EJob.DisableManualStart));
        }

        public static void AddRecurring(TQ query = default)
        {
            query = query ?? new TQ();
            query.JobType = EJob.Types.Recurring;
            UpdateRecurring(query, query.StartCron);
        }

        private static void UpdateRecurring(TQ query, string upJobCronTime)
        {
            RecurringJob.RemoveIfExists(JobId);
            RecurringJob.AddOrUpdate(JobId, MethodCall(query, EJob.DisableScheduledStart), upJobCronTime,
                query.RecurringOptions);
        }

        private static Expression<Func<TI, Task>> MethodCall(TQ query, bool isJobDisabled = false)
        {
            return job => job.RunAsync(query, query.JobType, isJobDisabled, CancellationToken.None);
        }
    }
}