using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace IAE.Microservice.Application.Common.CacheProviders
{
    public class MemoryCacheProvider : ICacheProvider
    {
        private static readonly SemaphoreSlim GetSemaphore = new SemaphoreSlim(1, 1);

        private readonly IMemoryCache _cache;

        public MemoryCacheProvider(IMemoryCache cache)
        {
            _cache = cache;
        }

        private T GetFromCache<T>(string key) where T : class
        {
            return _cache.Get<T>(key);
        }

        private void SetToCache<T>(string key, T value) where T : class
        {
            _cache.Set(key, value);
        }

        private void SetToCache<T>(string key, T value, DateTimeOffset duration) where T : class
        {
            _cache.Set(key, value, duration);
        }

        private void ClearCache(string key)
        {
            _cache.Remove(key);
        }

        public async Task<T> GetFromOrSetToCacheAsync<T>(string key, Func<Task<T>> func,
            DateTimeOffset duration = default)
            where T : class
        {
            var data = GetFromCache<T>(key);
            if (data != null) return data;

            var semaphore = GetSemaphore;
            try
            {
                await semaphore.WaitAsync();
                // Recheck to make sure it didn't populate before entering semaphore
                data = GetFromCache<T>(key);
                if (data != null) return data;

                data = await func();
                if (data == null) return null;
                if (duration == default)
                {
                    SetToCache(key, data);
                }
                else
                {
                    SetToCache(key, data, duration);
                }
            }
            finally
            {
                semaphore.Release();
            }

            return data;
        }
    }
}