using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IAE.Microservice.Application.Common.Operation;
using IAE.Microservice.Application.Features.Users.Profiles;
using IAE.Microservice.Application.Interfaces.Social;
using IAE.Microservice.Infrastructure.Social.Client;

namespace IAE.Microservice.Infrastructure.Social.Endpoints.Users
{
    public class UserService : ISocialUserService
    {
        private readonly ISocialClient _socialClient;
        private readonly IMapper _mapper;

        public UserService(ISocialClient socialClient, IMapper mapper)
        {
            _socialClient = socialClient;
            _mapper = mapper;
        }

        public async Task<OperationResult<Update.Response>> SendCreateOrUpdateAsync(
            Update.Command query, CancellationToken token)
        {
            var request = _mapper.Map<UserVm.CreateOrUpdateRequest>(query);
            var response = await _socialClient.CreateOrUpdateUserAsync(request, token);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return OperationResult<Update.Response>.Failed(null, response.Errors.ToArray());
            }
            
            var result = _mapper.Map<Update.Response>(response.Data);
            return OperationResult<Update.Response>.Success(result);
        }
    }
}