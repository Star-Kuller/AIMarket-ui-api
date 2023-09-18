using System.Threading;
using System.Threading.Tasks;
using IAE.Microservice.Application.Common.Operation;
using IAE.Microservice.Application.Features.Chats;
using IAE.Microservice.Application.Features.Users.Profiles;

namespace IAE.Microservice.Application.Interfaces.Social
{
    public interface ISocialChatService
    {
        Task<OperationResult<Create.Response>> SendCreateOrUpdateAsync(
            Create.Command query, CancellationToken token);
    }
}