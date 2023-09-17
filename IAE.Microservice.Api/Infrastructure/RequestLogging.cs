using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IAE.Microservice.Api.Infrastructure
{
    public abstract class RequestLogging
    {
        public class Middleware : IMiddleware
        {
            private readonly ILogger _logger;

            public Middleware(ILoggerFactory loggerFactory)
            {
                _logger = loggerFactory.CreateLogger<RequestLogging>();
            }

            public async Task InvokeAsync(HttpContext context, RequestDelegate next)
            {
                var feature = context.Features.Get<IConnectionLifetimeNotificationFeature>();
                if (feature == null)
                {
                    feature = new ConnectionCloseNotificationLifeTimeFeature(context.Response);
                    context.Features.Set(feature);
                }

                var watch = new Stopwatch();
                try
                {
                    _logger.LogInformation(
                        "RequestLogging ({id}) start: {method} '{url}{query}'",
                        context.TraceIdentifier,
                        context.Request?.Method,
                        context.Request?.Path.Value,
                        context.Request?.QueryString.Value);
                    watch.Start();
                    await next(context);
                }
                finally
                {
                    watch.Stop();
                    _logger.LogInformation(
                        "RequestLogging ({id}) end: {method} '{url}{query}' after {duration}ms => {code}",
                        context.TraceIdentifier,
                        context.Request?.Method,
                        context.Request?.Path.Value,
                        context.Request?.QueryString.Value,
                        watch.ElapsedMilliseconds,
                        context.Response?.StatusCode);
                }
            }


            private class ConnectionCloseNotificationLifeTimeFeature : IConnectionLifetimeNotificationFeature
            {
                private readonly HttpResponse _httpResponse;

                public ConnectionCloseNotificationLifeTimeFeature(HttpResponse httpResponse)
                {
                    _httpResponse = httpResponse;
                }

                public CancellationToken ConnectionClosedRequested { get; set; }

                public void RequestClose()
                {
                    // Set the connection close feature if the response hasn't sent headers as yet
                    if (!_httpResponse.HasStarted)
                    {
                        _httpResponse.Headers[HeaderNames.Connection] = "close";
                    }
                }
            }
        }


        public class HttpClientMessageHandler : DelegatingHandler
        {
            private readonly ILogger _logger;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public HttpClientMessageHandler(ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor)
            {
                _logger = loggerFactory.CreateLogger<RequestLogging>();
                _httpContextAccessor = httpContextAccessor;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                _logger.LogInformation("RequestLogging ({id}) httpClient: {method} '{url}'",
                    _httpContextAccessor?.HttpContext?.TraceIdentifier ?? "unknown",
                    request?.Method,
                    request?.RequestUri.ToString());

                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}