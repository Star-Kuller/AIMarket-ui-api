using System.Net;
using AutoMapper;
using IAE.Microservice.Application.Common.Operation;
using IAE.Microservice.Application.Interfaces.Social;
using IAE.TradingDesk.Infrastructure.Bidder.Client;

namespace IAE.Microservice.Infrastructure.Social.Endpoints
{
    public class GenericService : ISocialGenericService
    {
        private readonly ISocialClient _socialClient;
        private readonly IMapper _mapper;

        public GenericService(ISocialClient socialClient, IMapper mapper)
        {
            _socialClient = socialClient;
            _mapper = mapper;
        }

        public async Task<OperationResult<TResult>> DoAsync<TQuery, TResult, TRequest, TResponse>(
            TQuery query, Func<TRequest, CancellationToken, Task<OperationApiResult<TResponse>>> clientFunc,
            CancellationToken cancellationToken, HttpStatusCode successfulCode = HttpStatusCode.OK)
            where TQuery : class, new()
            where TResult : class, new()
        {
            var request = _mapper.Map<TRequest>(query);
            var response = await clientFunc(request, cancellationToken);
            if (response.StatusCode != successfulCode)
            {
                return OperationResult<TResult>.Failed(null, response.Errors.ToArray());
            }

            var result = _mapper.Map<TResult>(response.Data);
            return OperationResult<TResult>.Success(result);
        }
    }
}