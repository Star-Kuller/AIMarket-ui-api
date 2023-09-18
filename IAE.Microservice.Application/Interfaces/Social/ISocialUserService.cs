using System.Threading;
using System.Threading.Tasks;
using IAE.Microservice.Application.Common.Operation;
using IAE.Microservice.Application.Features.Users.Profiles;

namespace IAE.Microservice.Application.Interfaces.Social
{
    public interface ISocialUserService
    {
        Task<OperationResult<Update.Response>> SendCreateOrUpdateAsync(
            Update.Command query, CancellationToken token);
    }
}