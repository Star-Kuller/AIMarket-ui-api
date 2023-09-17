using System.Net;
using AutoMapper;
using IAE.Microservice.Application.Common.Operation;

namespace IAE.Microservice.Infrastructure.Social
{
    public abstract class SocialServiceBase<TEntity, TCreateRequest, TCreateResponse, TEditRequest, TEditResponse>
    {
        private readonly IMapper _mapper;

        protected SocialServiceBase(IMapper mapper)
        {
            _mapper = mapper;
        }

        public virtual async Task<OperationResult<TEntity>> CreateAsync(TEntity entity,
            CancellationToken cancellationToken)
        {
            var request = _mapper.Map<TCreateRequest>(entity);
            var response = await SendCreateAsync(entity, request, cancellationToken);
            if (response.StatusCode != HttpStatusCode.Created)
            {
                return OperationResult<TEntity>.Failed(entity, response.Errors.ToArray());
            }

            _mapper.Map(response.Data, entity);
            return OperationResult<TEntity>.Success(entity);
        }

        public virtual async Task<OperationResult<TEntity>> EditAsync(TEntity entity,
            CancellationToken cancellationToken)
        {
            var request = _mapper.Map<TEditRequest>(entity);
            var response = await SendEditAsync(entity, request, cancellationToken);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return OperationResult<TEntity>.Failed(entity, response.Errors.ToArray());
            }

            _mapper.Map(response.Data, entity);
            return OperationResult<TEntity>.Success(entity);
        }

        private protected abstract Task<OperationApiResult<TCreateResponse>> SendCreateAsync(TEntity entity,
            TCreateRequest request, CancellationToken token);

        private protected abstract Task<OperationApiResult<TEditResponse>> SendEditAsync(TEntity entity,
            TEditRequest request, CancellationToken token);
    }
}