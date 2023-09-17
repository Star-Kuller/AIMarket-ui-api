using IAE.Microservice.Api.Attributes;

namespace IAE.Microservice.Api.Controllers
{
    [ValidateRequest]
    public abstract class BaseValidatedController : BaseController
    {
    }
}