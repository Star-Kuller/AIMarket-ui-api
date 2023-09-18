using System.Threading;
using System.Threading.Tasks;
using IAE.Microservice.Application.Common.Operation;
using IAE.Microservice.Infrastructure.Social.Endpoints.Users;

namespace IAE.Microservice.Infrastructure.Social.Client
{
    public partial class SocialClient
    {
        public async Task<OperationApiResult<UserVm.CreateOrUpdateResponse>> CreateOrUpdateUserAsync(
            UserVm.CreateOrUpdateRequest request, CancellationToken token = default)
        {
            var path = SocialClient.GetPath(SocialClient.UserPath);
            return await PostAsync<UserVm.CreateOrUpdateRequest, UserVm.CreateOrUpdateResponse>(request, path, token);
        }

        public async Task<OperationApiResult<UserVm.GetResponse>> GetAsync(
            UserVm.GetRequest request, CancellationToken token = default)
        {
            var path = SocialClient.GetPath(SocialClient.UserPath);
            return await GetAsync<UserVm.GetRequest, UserVm.GetResponse>(request, path, token);
        }
    }
}