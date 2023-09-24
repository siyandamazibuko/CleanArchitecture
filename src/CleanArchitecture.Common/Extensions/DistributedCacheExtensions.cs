using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;


namespace CleanArchitecture.Common.Extensions
{
    public static class DistributedCacheExtensions
    {
        public static readonly DistributedCacheEntryOptions DefaultDistributedCacheEntryOptions
            = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12),
                SlidingExpiration = TimeSpan.FromHours(12),
            };

        public static async Task<TObject> GetOrSetValueAsync<TObject>(this IDistributedCache cache, string key, Func<Task<TObject>> factory, DistributedCacheEntryOptions options = null)
            where TObject : class
        {
            var result = await cache.GetValueAsync<TObject>(key);
            if (result != null)
            {
                return result;
            }

            result = await factory();

            await cache.SetValueAsync(key, result, options);

            return result;
        }

        private static async Task<TObject> GetValueAsync<TObject>(this IDistributedCache cache, string key)
            where TObject : class
        {
            var data = await cache.GetStringAsync(key);
            if (data == null)
            {
                return default;
            }

            return JsonConvert.DeserializeObject<TObject>(data);
        }

        private static async Task SetValueAsync<TObject>(this IDistributedCache cache, string key, TObject value, DistributedCacheEntryOptions options = null)
            where TObject : class
        {
            var data = JsonConvert.SerializeObject(value);

            await cache.SetStringAsync(key, data, options ?? DefaultDistributedCacheEntryOptions);
        }
    }
}
