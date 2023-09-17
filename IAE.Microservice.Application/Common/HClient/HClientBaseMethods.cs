using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IAE.Microservice.Application.Common.Operation;

namespace IAE.Microservice.Application.Common.HClient
{
    public abstract partial class HClientBase
    {
        protected async Task<OperationApiResult<TOut>> PostAsync<TIn, TOut>(TIn content, string path,
            CancellationToken token = default)
            where TIn : class
            where TOut : class
        {
            var profile = new HClientProfile<TIn, TOut>
            {
                Content = content,
                PathAndQuery = path,
                Method = HttpMethod.Post
            };
            return await SendAsync<TIn, TOut>(profile, token);
        }

        protected async Task<OperationApiResult<TOut>> PutAsync<TIn, TOut>(TIn content, string path,
            CancellationToken token = default)
            where TIn : class
            where TOut : class
        {
            var profile = new HClientProfile<TIn, TOut>
            {
                Content = content,
                PathAndQuery = path,
                Method = HttpMethod.Put
            };
            return await SendAsync<TIn, TOut>(profile, token);
        }

        protected async Task<OperationApiResult<TOut>> GetAsync<TOut>(string path, CancellationToken token = default)
            where TOut : class
        {
            var profile = new HClientProfile<string, TOut>
            {
                Content = null,
                PathAndQuery = path,
                Method = HttpMethod.Get
            };
            return await SendAsync<string, TOut>(profile, token);
        }

        protected async Task<OperationApiResult<TOut>> GetAsync<TIn, TOut>(TIn query, string path,
            CancellationToken token = default)
            where TIn : HClientQuery
            where TOut : class
        {
            var profile = new HClientProfile<string, TOut>
            {
                Content = null,
                PathAndQuery = query.ToPathAndQuery(path),
                Method = HttpMethod.Get
            };
            return await SendAsync<string, TOut>(profile, token);
        }

        protected async Task<OperationApiResult<TOut>> DeleteAsync<TIn, TOut>(TIn content, string path,
            CancellationToken token = default)
            where TIn : class
            where TOut : class
        {
            var profile = new HClientProfile<TIn, TOut>
            {
                Content = content,
                PathAndQuery = path,
                Method = HttpMethod.Delete
            };
            return await SendAsync<TIn, TOut>(profile, token);
        }

        protected async Task<OperationApiResult<TOut>> DeleteAsync<TOut>(string path, CancellationToken token = default)
            where TOut : class
        {
            var profile = new HClientProfile<string, TOut>
            {
                Content = null,
                PathAndQuery = path,
                Method = HttpMethod.Delete
            };
            return await SendAsync<string, TOut>(profile, token);
        }

        protected static string SetupPath(params string[] segments) => string.Join("/", segments);
    }
}