using System.Threading;
using System.Threading.Tasks;
using IAE.Microservice.Application.Common.Operation;
using IAE.Microservice.Infrastructure.Social.Endpoints.Chats;
using IAE.Microservice.Infrastructure.Social.Endpoints.Users;

namespace IAE.Microservice.Infrastructure.Social.Client
{
    /// <summary>
    /// all methods are taken from Bidder API documentation (<a href="http://doc.iae.one/dsp">link</a>)
    /// </summary>
    public interface ISocialClient
    {
        #region Users

        Task<OperationApiResult<UserVm.CreateOrUpdateResponse>> CreateOrUpdateUserAsync(
            UserVm.CreateOrUpdateRequest request, CancellationToken token = default);

        Task<OperationApiResult<UserVm.GetResponse>> GetAsync(
            UserVm.GetRequest request, CancellationToken token = default);

        #endregion
        
        #region Chats

        Task<OperationApiResult<ChatVm.CreateOrUpdateResponse>> CreateOrUpdateChatAsync(
            ChatVm.CreateOrUpdateRequest request, CancellationToken token = default);

        #endregion
    }
}