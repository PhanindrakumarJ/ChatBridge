using Microsoft.Extensions.Caching.Memory;

namespace ChatBridge.Chat.API.Services
{
    public class RateLimitNotificationCooldown
    {
        private readonly IMemoryCache _memoryCache;
        private readonly object _lock = new object();

        private const string CacheKey = "RateLimitNotification";
        private readonly TimeSpan _cooldown =
            TimeSpan.FromSeconds(90);

        public RateLimitNotificationCooldown(
            IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public bool TryAcquireNotification()
        {
            lock (_lock)
            {
                if (_memoryCache.TryGetValue(CacheKey, out _))
                {
                    return false;
                }

                _memoryCache.Set(
                    CacheKey,
                    true,
                    _cooldown);

                return true;
            }
        }

        public void ReleaseNotification()
        {
            lock (_lock)
            {
                _memoryCache.Remove(CacheKey);
            }
        }
    }
}