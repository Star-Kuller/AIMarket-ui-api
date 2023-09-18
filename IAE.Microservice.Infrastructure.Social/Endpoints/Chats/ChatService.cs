using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IAE.Microservice.Application.Common.Operation;
using IAE.Microservice.Application.Features.Chats;
using IAE.Microservice.Application.Interfaces.Social;
using IAE.Microservice.Infrastructure.Social.Client;

namespace IAE.Microservice.Infrastructure.Social.Endpoints.Chats
{
    public class ChatService : ISocialChatService
    {
        private readonly ISocialClient _socialClient;
        private readonly IMapper _mapper;

        public ChatService(ISocialClient socialClient, IMapper mapper)
        {
            _socialClient = socialClient;
            _mapper = mapper;
        }

        public async Task<OperationResult<Create.Response>> SendCreateOrUpdateAsync(
            Create.Command query, CancellationToken token)
        {
            var request = _mapper.Map<ChatVm.CreateOrUpdateRequest>(query);
            var response = await _socialClient.CreateOrUpdateChatAsync(request, token);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return OperationResult<Create.Response>.Failed(null, response.Errors.ToArray());
            }
            
            var result = _mapper.Map<Create.Response>(response.Data);
            return OperationResult<Create.Response>.Success(result);
        }
    }
}