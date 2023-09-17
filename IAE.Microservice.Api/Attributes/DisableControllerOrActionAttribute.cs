using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IAE.Microservice.Api.Attributes
{
    public class DisableControllerOrActionAttribute : ActionFilterAttribute, IApiDescriptionVisibilityProvider
    {
        /// <param name="ignoreApi">If <c>false</c> then no <c>ApiDescription</c> objects will be created
        /// for the associated controller or action.</param>
        public DisableControllerOrActionAttribute(bool ignoreApi = true)
        {
            IgnoreApi = ignoreApi;
        }

        public bool IgnoreApi { get; }

        public override void OnActionExecuting(ActionExecutingContext cxt)
        {
            cxt.Result = new NotFoundResult();
        }
    }
}