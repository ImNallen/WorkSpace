using Microsoft.Extensions.Caching.Distributed;

namespace Api.Services.Caching;

public static class CacheOptions
{
    public static DistributedCacheEntryOptions DefaultExpiration =>
        new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2) };

    public static DistributedCacheEntryOptions Create(TimeSpan? expiration)
    {
        return expiration is not null
            ? new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration }
            : DefaultExpiration;
    }
}
