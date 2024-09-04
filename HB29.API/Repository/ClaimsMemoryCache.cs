using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hb29.API.Repository
{
    public class ClaimsMemoryCache
    {
        private readonly IMemoryCache _memoryCache;

        public ClaimsMemoryCache()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions { SizeLimit = 4096 });
        }

        public void Remove(string upn)
        {
            _memoryCache.Remove(upn);
        }

        public List<Models.Profile> Get(string upn)
        {
            return _memoryCache.Get<List<Models.Profile>>(upn);
        }

        public async Task<List<Models.Profile>> GetOrCreate(string upn, Func<string, Task<List<Models.Profile>>> cacheEntryFactory)
        {
            var cachedValue = await _memoryCache.GetOrCreateAsync(
                upn,
                async cacheEntry =>
                {
                    cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(10);
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);

                    var permissions = await cacheEntryFactory(upn);
                    cacheEntry.SetSize(permissions.Count);
                    
                    return permissions;
                }
            );

            return cachedValue;
        }
    }
}
