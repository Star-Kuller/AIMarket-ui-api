using System.Net;
using AutoMapper;

namespace IAE.Microservice.Infrastructure.Social.Endpoints.Chats
{
    public class ChatService :
        SocialServiceBase<User, ChatVm.CreateRequest, ChatVm.CreateResponse,
            ChatVm.EditRequest, ChatVm.EditResponse>,
        ISocialChatService
    {
        private readonly IBidderClient _bidderClient;
        private readonly IMapper _mapper;

        public ChatService(IBidderClient bidderClient, IMapper mapper) : base(mapper)
        {
            _bidderClient = bidderClient;
            _mapper = mapper;
        }

        private protected override async Task<OperationApiResult<ChatVm.CreateResponse>> SendCreateAsync(
            User entity, ChatVm.CreateRequest request, CancellationToken token)
        {
            return await _bidderClient.CreateAdvertiserAsync(request, entity?.Manager?.BidderId, token);
        }

        private protected override async Task<OperationApiResult<ChatVm.EditResponse>> SendEditAsync(
            User entity, ChatVm.EditRequest request, CancellationToken token)
        {
            return await _bidderClient.EditAdvertiserAsync(request, entity.BidderId, token);
        }

        public async Task<OperationResult<BalanceGet.Model>> GetBalanceAsync(User entity,
            CancellationToken cancellationToken)
        {
            var response =
                await _bidderClient.GetAdvertiserBalanceAsync(entity.BidderId, cancellationToken);
            return response.StatusCode != HttpStatusCode.OK
                ? OperationResult<BalanceGet.Model>.Failed(null, response.Errors.ToArray())
                : OperationResult<BalanceGet.Model>.Success(_mapper.Map<BalanceGet.Model>(response.Data));
        }

        public async Task<OperationResult<BalanceAdd.Model>> AddBalanceAsync(User entity,
            BidderAdvertiserIVm.IAddBalanceRequest request, CancellationToken cancellationToken)
        {
            var response =
                await _bidderClient.AddAdvertiserBalanceAsync(entity.BidderId, request.Amount, cancellationToken);
            return response.StatusCode != HttpStatusCode.OK
                ? OperationResult<BalanceAdd.Model>.Failed(null, response.Errors.ToArray())
                : OperationResult<BalanceAdd.Model>.Success(_mapper.Map<BalanceAdd.Model>(response.Data));
        }

        public async Task<OperationResult<BalanceListById.Result>> ListBalanceByIdAsync(BalanceListById.Query query,
            CancellationToken cancellationToken)
        {
            var request = _mapper.Map<ChatVm.ListBalanceRequest>(query);
            var response =
                await _bidderClient.ListAdvertiserBalanceAsync(request, cancellationToken);
            return response.StatusCode != HttpStatusCode.OK
                ? OperationResult<BalanceListById.Result>.Failed(null, response.Errors.ToArray())
                : OperationResult<BalanceListById.Result>.Success(_mapper.Map<BalanceListById.Result>(response.Data));
        }

        public virtual async Task<OperationResult<User>> CreateAsync(User entity, string password,
            CancellationToken cancellationToken)
        {
            var request = _mapper.Map<ChatVm.CreateRequest>(entity);
            request.Password = password;
            var response = await SendCreateAsync(entity, request, cancellationToken);
            if (response.StatusCode != HttpStatusCode.Created)
            {
                return OperationResult<User>.Failed(entity, response.Errors.ToArray());
            }

            _mapper.Map(response.Data, entity);
            return OperationResult<User>.Success(entity);
        }

        public async Task<OperationResult<string>> ChangeStatusAsync(User entity, CancellationToken cancellationToken)
        {
            var isActivated = entity.Status == Status.Enabled;
            return await _bidderClient.ChangeStatusAdvertiserAsync(entity.BidderId, isActivated, cancellationToken);
        }
        
        public async Task<OperationResult<string>> DeleteAsync(User entity, CancellationToken cancellationToken)
        {
            return await _bidderClient.DeleteAdvertiserAsync(entity.BidderId, cancellationToken);
        }
    }
}