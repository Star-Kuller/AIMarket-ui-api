using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace IAE.Microservice.Application.Common.HClient
{
    public class HClientProfile<TIn, TOut>
    {
        public TIn Content { get; set; }

        public HttpMethod Method { get; set; }

        public string PathAndQuery { get; set; }

        public Uri Url { get; set; } = null;

        public string MediaType { get; set; } = "application/json";

        public Encoding Encoding { get; set; } = Encoding.UTF8;

        public HttpCompletionOption CompletionOption { get; set; } = HttpCompletionOption.ResponseContentRead;

        public Action<HttpRequestHeaders> SetRequestHeaders { get; set; } = null;

        public Func<HttpContent, Task<TOut>> ResponseHandler { get; set; } = null;
    }
}