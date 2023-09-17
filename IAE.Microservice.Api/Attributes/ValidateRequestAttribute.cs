using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IAE.Microservice.Api.Attributes
{
    public class ValidateRequestAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext cxt)
        {
            if (cxt.ModelState == null || cxt.ModelState.IsValid) return;
            cxt.Result = new BadRequestObjectResult(new SerializableError(cxt.ModelState));
        }
    }
}