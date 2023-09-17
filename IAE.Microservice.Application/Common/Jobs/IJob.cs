using System.Threading;
using System.Threading.Tasks;

namespace IAE.Microservice.Application.Common.Jobs
{
    public interface IJob<in TQ>
    {
        Task RunAsync(TQ query, EJob.Types jobType, bool isJobDisabled = false, CancellationToken token = default);
    }
}