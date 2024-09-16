namespace CurrencyManager.Proxies.CacheProxy
{
    using CurrencyManager.Models.ProxyModels;
    using CurrencyManager.Models.SystemModels;
    using Microsoft.Extensions.Caching.Memory;

    public class CacheProxy: ICacheProxy
    {
        private readonly IMemoryCache _memoryCache;
        private readonly LoggerFacade<CacheProxy> logger;

        public CacheProxy(LoggerFacade<CacheProxy> logger, IMemoryCache memoryCache)
        {
            this.logger = logger;
            _memoryCache = memoryCache;
        }

        public async Task<ExchangeRate> GetCacheValue(string key)
        {
            ExchangeRate value = null;

            try
            {                
                _memoryCache.TryGetValue(key, out value);                
            }
            catch(Exception ex)
            {
                logger.Error(ex);
            }

            return value;
        }

        public async Task<HistoricalExchangeRate> GetCacheValueForHistoricalRates(string key)
        {
            HistoricalExchangeRate value = null;

            try
            {
                _memoryCache.TryGetValue(key, out value);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return value;
        }

        public async Task<bool> SetCacheValue(string key, ExchangeRate value)
        {
            try
            {
                _memoryCache.Set(key, value,
                     new MemoryCacheEntryOptions()
                     .SetAbsoluteExpiration(TimeSpan.FromHours(24)));
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }

        public async Task<bool> SetCacheValueForHistoricalRates(string key, HistoricalExchangeRate value)
        {
            try
            {
                _memoryCache.Set(key, value,
                     new MemoryCacheEntryOptions()
                     .SetAbsoluteExpiration(TimeSpan.FromHours(24)));
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }
    }
}
