using CurrencyManager.Models.ProxyModels;

namespace CurrencyManager.Proxies.CacheProxy
{
    public interface ICacheProxy
    {
        Task<ExchangeRate> GetCacheValue(string key);
        Task<HistoricalExchangeRate> GetCacheValueForHistoricalRates(string key);
        Task<bool> SetCacheValue(string key, ExchangeRate value);
        Task<bool> SetCacheValueForHistoricalRates(string key, HistoricalExchangeRate value);
    }
}
