using System.Threading;
using System.Threading.Tasks;
using IAE.Microservice.Application.Common.Operation;

namespace IAE.Microservice.Application.Interfaces.Bidder
{
    public interface IBidderService<TEntity>
    {
        Task<OperationResult<TEntity>> CreateAsync(TEntity entity, CancellationToken cancellationToken);

        Task<OperationResult<TEntity>> EditAsync(TEntity entity, CancellationToken cancellationToken);
    }
}