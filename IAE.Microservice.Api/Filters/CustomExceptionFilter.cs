using IAE.Microservice.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;
using IAE.Microservice.Api.Configurations;
using Microsoft.Extensions.Logging;

namespace IAE.Microservice.Api.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly bool _hideEx = EnvironmentConfig.HideExceptions;

        private readonly ILogger<CustomExceptionFilterAttribute> _logger;

        public CustomExceptionFilterAttribute(ILogger<CustomExceptionFilterAttribute> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case ValidationException _:
                    OnValidationException(context);
                    break;
                case NotFoundException _:
                    OnNotFoundOrUnexpectedException(context, HttpStatusCode.NotFound);
                    break;
                default:
                    OnNotFoundOrUnexpectedException(context, HttpStatusCode.InternalServerError);
                    break;
            }
        }

        private static void OnValidationException(ExceptionContext context)
        {
            var result = new JsonResult(
                ((ValidationException) context.Exception).Failures);
            Complete(context, HttpStatusCode.BadRequest, result);
        }

        private void OnNotFoundOrUnexpectedException(ExceptionContext context, HttpStatusCode statusCode)
        {
            var message = context.Exception.InnerException?.Message ?? context.Exception.Message;
            var stackTrace = context.Exception.InnerException?.ToString() ?? context.Exception.ToString();
            _logger.LogError($"{DateTime.UtcNow:yyyy-MM-dd hh:mm:ss.fff}: " +
                             $"{(_hideEx ? message : stackTrace)}");
            var result = _hideEx
                ? new JsonResult(new {error = message})
                : new JsonResult(new {error = message, stackTrace});
            Complete(context, statusCode, result);
        }

        private static void Complete(ExceptionContext context, HttpStatusCode code, IActionResult result)
        {
            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.StatusCode = (int) code;
            context.Result = result;
        }
    }
}