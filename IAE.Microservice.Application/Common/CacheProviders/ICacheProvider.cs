using System;
using System.Threading.Tasks;

namespace IAE.Microservice.Application.Common.CacheProviders
{
    public interface ICacheProvider
    {
        Task<T> GetFromOrSetToCacheAsync<T>(string key, Func<Task<T>> func, DateTimeOffset duration = default)
            where T : class;
    }
}