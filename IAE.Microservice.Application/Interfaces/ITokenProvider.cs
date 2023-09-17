using IAE.Microservice.Application.Tokens.Models;

namespace IAE.Microservice.Application.Interfaces
{
    public interface ITokenProvider
    {
        string GetToken(UserClaims userClaims);
    }
}
