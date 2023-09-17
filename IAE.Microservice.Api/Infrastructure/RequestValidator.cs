using IAE.Microservice.Application.Common.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.Net;
using System.Threading.Tasks;

namespace IAE.Microservice.Api.Infrastructure
{
    public abstract class RequestValidator
    {
        public class Middleware : IMiddleware
        {
            public async Task InvokeAsync(HttpContext context, RequestDelegate next)
            {
                var jsonError = ValidateContentLength(context);
                if (jsonError != null)
                {
                    await CompleteAsync(context, jsonError);
                    return;
                }

                await next(context);
            }

            private static async Task CompleteAsync(HttpContext context, string jsonError)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync(jsonError);
            }

            private static string ValidateContentLength(HttpContext context)
            {
                var maxSize = context.Features.Get<IHttpMaxRequestBodySizeFeature>().MaxRequestBodySize;
                var size = context.Request.ContentLength;
                if (maxSize < size)
                {
                    return $"{{\"request\": [\"Request body size ({size.Value.ToFormattedBytes()}) cannot be greater " +
                           $"than max supported size ({maxSize.Value.ToFormattedBytes()}) by application. " +
                           "If request contains file, check its size\"]}";
                }

                return null;
            }
        }
    }
}