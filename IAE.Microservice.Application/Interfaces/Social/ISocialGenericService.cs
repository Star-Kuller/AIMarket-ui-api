using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using IAE.Microservice.Application.Common.Operation;

namespace IAE.Microservice.Application.Interfaces.Social
{
    public interface ISocialGenericService
    {
        Task<OperationResult<TResult>> DoAsync<TQuery, TResult, TRequest, TResponse>(
            TQuery query, Func<TRequest, CancellationToken, Task<OperationApiResult<TResponse>>> clientFunc,
            CancellationToken cancellationToken, HttpStatusCode successfulCode = HttpStatusCode.OK)
            where TQuery : class, new()
            where TResult : class, new();
    }
}