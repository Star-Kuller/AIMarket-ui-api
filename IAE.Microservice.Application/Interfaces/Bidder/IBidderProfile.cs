namespace IAE.Microservice.Application.Interfaces.Bidder
{
    public interface IBidderProfile<TQuery>
    {
        TQuery Query { get; }
    }
    
}