using System;

namespace IAE.Microservice.Application.Common.Extensions
{
    public static class StringExtensions
    {
        public static string ToFormattedUri(this string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url) || !Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri))
                {
                    return url;
                }

                if (uri.PathAndQuery == "/")
                {
                    if (!url.EndsWith("/"))
                    {
                        url += "/";
                    }

                    return url;
                }

                if (url.Contains("/?") || !uri.PathAndQuery.StartsWith("/?"))
                {
                    return url;
                }

                var i = url.IndexOf('?');
                return url.Insert(i, "/");
            }
            catch
            {
                return url;
            }
        }
    }
}