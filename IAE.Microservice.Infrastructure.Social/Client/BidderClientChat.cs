using System.Threading;
using System.Threading.Tasks;
using IAE.Microservice.Application.Common.Operation;
using IAE.Microservice.Infrastructure.Social.Endpoints.Chats;

namespace IAE.Microservice.Infrastructure.Social.Client
{
    public partial class SocialClient
    {
        public async Task<OperationApiResult<ChatVm.CreateOrUpdateResponse>> CreateOrUpdateChatAsync(
            ChatVm.CreateOrUpdateRequest request, CancellationToken token = default)
        {
            var path = SocialClient.GetPath(SocialClient.ChatPath);
            return await PostAsync<ChatVm.CreateOrUpdateRequest, ChatVm.CreateOrUpdateResponse>(request, path, token);
        }
    }
}