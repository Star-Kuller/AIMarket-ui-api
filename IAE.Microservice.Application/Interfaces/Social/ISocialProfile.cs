namespace IAE.Microservice.Application.Interfaces.Social
{
    public interface ISocialProfile<TQuery>
    {
        TQuery Query { get; }
    }
    
}