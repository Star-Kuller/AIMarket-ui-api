using IAE.Microservice.Application.Common.File;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace IAE.Microservice.Api.Controllers
{
    public class BaseController : ControllerBase
    {
        private IMediator _mediator;

        protected IMediator Mediator => _mediator ?? (_mediator = HttpContext.RequestServices.GetService<IMediator>());

        /// <summary>
        /// Returns a file with the specified <paramref name="result.fileContents" />
        /// as content (<see cref="StatusCodes.Status200OK"/>), the
        /// specified <paramref name="result.contentType" /> as the Content-Type
        /// and the specified <paramref name="result.fileDownloadName" /> as the suggested file name.
        /// This supports range requests (<see cref="StatusCodes.Status206PartialContent"/> or
        /// <see cref="StatusCodes.Status416RangeNotSatisfiable"/> if the range is not satisfiable).
        /// </summary>
        /// <param name="result">The object containing the above parameters.</param>
        /// <returns>The created <see cref="FileContentResult"/> for the response.</returns>
        [NonAction]
        public virtual FileContentResult File(IFileResult result) =>
            File(result.Contents, result.ContentType, result.DownloadName);
    }
}