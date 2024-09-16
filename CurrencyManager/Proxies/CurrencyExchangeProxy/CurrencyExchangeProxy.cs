using CurrencyManager.Helpers;
using CurrencyManager.Models.ProxyModels;
using CurrencyManager.Models.SystemModels;
using CurrencyManager.Proxies.CacheProxy;
using Newtonsoft.Json;

namespace CurrencyManager.Proxies.CurrencyExchangeProxy
{
    public class CurrencyExchangeProxy: ICurrencyExchangeProxy
    {
        private readonly HttpClient httpClient;

        private readonly LoggerFacade<CurrencyExchangeProxy> logger;

        private readonly ServiceConfig serviceConfig;

        private readonly ICacheProxy cacheProxy;

        public CurrencyExchangeProxy(HttpClient client,
            ServiceConfig serviceConfig,
            LoggerFacade<CurrencyExchangeProxy> logger,
            ICacheProxy cacheProxy)
        {
            this.logger = logger;
            this.httpClient = client;
            this.serviceConfig = serviceConfig;
            this.cacheProxy = cacheProxy;
        }

        public async Task<ExchangeRate> GetLatestRatesForBaseCurrency(string baseCurrency)
        {
            this.logger.Info("Start of GetLatestRatesForBaseCurrency()");

            ExchangeRate exchangeRate = await this.cacheProxy.GetCacheValue("Latest_"+baseCurrency);

            if (exchangeRate is null)
            {
                string uri = this.serviceConfig.ConstructUri(
                    "FrankfurterApi",
                    "LatestRates",
                    new
                    { });

                var response = await this.httpClient.GetAsync(uri+"?from="+ baseCurrency);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    exchangeRate = JsonConvert.DeserializeObject<ExchangeRate>(jsonString);
                    if (exchangeRate is not null)
                    {
                        await this.cacheProxy.SetCacheValue("Latest_"+baseCurrency,exchangeRate);
                    }
                    else
                    {
                        this.logger.Error("GetLatestRatesForBaseCurrency api returned empty responce");
                    }
                }
                else
                {
                    this.logger.Error("GetLatestRatesForBaseCurrency api call has failed");
                }
            }

            this.logger.Info("End of GetLatestRatesForBaseCurrency()");
            return exchangeRate;
        }

        public async Task<ExchangeRate> ConvertCurrency(string from,string to)
        {
            this.logger.Info("Start of ConvertCurrency()");
            ExchangeRate exchangeRate = await this.cacheProxy.GetCacheValue("Latest_from=" + from +"&to="+to);
            if (exchangeRate is null)
            {
                string uri = this.serviceConfig.ConstructUri(
                    "FrankfurterApi",
                    "LatestRates",
                    new
                    { });

                var response = await this.httpClient.GetAsync(uri + "?from=" + from + ((!string.IsNullOrEmpty(to))?"&to="+to:""));

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    exchangeRate = JsonConvert.DeserializeObject<ExchangeRate>(jsonString);
                    if (exchangeRate is not null)
                    {
                        exchangeRate.Rates.RemoveAll(new List<string>() { "TRY", "PLN", "THB", "MXN" });
                        await this.cacheProxy.SetCacheValue("Latest_from=" + from + "&to=" + to, exchangeRate);
                    }
                    else
                    {
                        this.logger.Error("ConvertCurrency api returned empty responce");
                    }
                }
                else
                {
                    this.logger.Error("ConvertCurrency api call has failed");
                }
            }

            this.logger.Info("End of ConvertCurrency()");
            return exchangeRate;
        }

        public async Task<HistoricalExchangeRate> HistoricalRates(string from, string to,string baseCurrency)
        {
            this.logger.Info("Start of HistoricalRates()");

            HistoricalExchangeRate exchangeRate = await this.cacheProxy.GetCacheValueForHistoricalRates("Historical_" + from + (string.IsNullOrEmpty(to) ? ".." : ".." + to) + "?from=" + baseCurrency);

            if (exchangeRate is null)
            {
                string uri = this.serviceConfig.ConstructUri(
                    "FrankfurterApi",
                    "HistoricalRates",
                    new
                    { });

                var response = await this.httpClient.GetAsync(uri + from + (string.IsNullOrEmpty(to)?"..":".."+to)+"?from="+baseCurrency);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    exchangeRate = JsonConvert.DeserializeObject<HistoricalExchangeRate>(jsonString);
                    if (exchangeRate is not null)
                    {
                        await this.cacheProxy.SetCacheValueForHistoricalRates("Historical_" + from + (string.IsNullOrEmpty(to) ? ".." : ".." + to) + "?from=" + baseCurrency, exchangeRate);
                    }
                    else
                    {
                        this.logger.Error("HistoricalRates api returned empty responce");
                    }
                }
                else
                {
                    this.logger.Error("HistoricalRates api call has failed");
                }
            }

            this.logger.Info("End of HistoricalRates()");
            return exchangeRate;

        }
    }
}
