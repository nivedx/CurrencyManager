using CurrencyManager.Models.ProxyModels;

namespace CurrencyManager.Proxies.CurrencyExchangeProxy
{
    public interface ICurrencyExchangeProxy
    {
        Task<ExchangeRate> GetLatestRatesForBaseCurrency(string baseCurrency);
        Task<ExchangeRate> ConvertCurrency(string from, string to);
        Task<HistoricalExchangeRate> HistoricalRates(string from, string to, string baseCurrency);
    }
}
