namespace IAE.Microservice.Domain.Entities.Common.Interfaces
{
    public interface IId<out T>
    {
        T Id { get; }
    }

    public interface IId : IId<long>
    {
    }
}