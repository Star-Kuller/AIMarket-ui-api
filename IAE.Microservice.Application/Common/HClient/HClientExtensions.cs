using System;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace IAE.Microservice.Application.Common.HClient
{
    public static class HClientExtensions
    {
        public static void Configure(this IHttpClientBuilder builder, HClientConfig config,
            Action<HttpRequestHeaders> setupDefaultRequestHeaders = null)
        {
            builder.ConfigureHttpClient(c =>
            {
                var uriString = config.BaseUri;
                if (!uriString[uriString.Length - 1].Equals('/'))
                {
                    uriString += '/';
                }

                c.BaseAddress = new Uri(uriString);
                setupDefaultRequestHeaders?.Invoke(c.DefaultRequestHeaders);
            });
        }
    }
}