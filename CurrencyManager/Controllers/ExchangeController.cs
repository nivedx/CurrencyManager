using CurrencyManager.Helpers;
using CurrencyManager.Models.SystemModels;
using CurrencyManager.Proxies.CurrencyExchangeProxy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyManager.Controllers
{
    [ApiController]
    [Route("api/exchangerates")]
    public class ExchangeController : ControllerBase
    {
       
        private readonly LoggerFacade<ExchangeController> _logger;

        private readonly ICurrencyExchangeProxy _currencyExchangeProxy;

        public ExchangeController(LoggerFacade<ExchangeController> logger, ICurrencyExchangeProxy currencyExchangeProxy)
        {
            _logger = logger;
            _currencyExchangeProxy = currencyExchangeProxy;
        }

        [HttpGet("fetchlatestratesbybasecurrency/{baseCurrency}")]
        [AllowAnonymous]
        public async Task<IActionResult> FetchLatestRatesForBaseCurrency(string baseCurrency)
        {
            this._logger.Info("Start of FetchLatestRatesForBaseCurrency.");

            if (string.IsNullOrEmpty(baseCurrency))
            {
                baseCurrency = "EUR";
            }
            try
            {
                var result = await this._currencyExchangeProxy.GetLatestRatesForBaseCurrency(baseCurrency);
                if (result == null)
                {
                    return NotFound();
                }
                this._logger.Info("End of FetchLatestRatesForBaseCurrency.");
                return this.Ok(result);
            }
            catch(Exception ex)
            {
                this._logger.Error("Error occured in FetchLatestRatesForBaseCurrency {0}", ex);
                return this.BadRequest("An error has occured. Please try later.");
            }
        }

        [HttpGet("convertcurrency")]
        [AllowAnonymous]
        public async Task<IActionResult> ConvertCurrency(string from, string? to=null)
        {
            this._logger.Info("Start of ConvertCurrency.");

            List<string> restrictedCurrencies = new List<string>() { "TRY", "PLN", "THB", "MXN" };

            if (!string.IsNullOrEmpty(from) && restrictedCurrencies.Any(it => it.Contains(from) || it.Contains(to)))
            {
                return BadRequest("Please try with a different currency");
            }

            if(!string.IsNullOrEmpty(to))
            {
                if (to.Contains(','))
                {
                    if(restrictedCurrencies.Any(it=> to.Split(',').Contains(it)))
                    {
                        return BadRequest("Please try with a different currency");
                    }
                }
            }

            from = (string.IsNullOrEmpty(from)) ? "EUR" : from;            

            try
            {
                var result = await this._currencyExchangeProxy.ConvertCurrency(from,to);
                if (result == null)
                {
                    return NotFound();
                }
                this._logger.Info("End of ConvertCurrency.");
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this._logger.Error("Error occured in ConvertCurrency {0}", ex);
                return this.BadRequest("An error has occured. Please try later.");
            }
        }

        [HttpGet("historicalrates/{fromDate}/{toDate}/{baseCurrency}")]
        [AllowAnonymous]
        public async Task<IActionResult> HistoricalRates(string fromDate, string toDate, string baseCurrency, int pageNumber = 1, int pageSize = 10)
        {
            this._logger.Info("Start of HistoricalRates.");

            try
            {
                string fromDateString = fromDate.IsValidDate();
                string toDateString = toDate.IsValidDate();

                var result = await this._currencyExchangeProxy.HistoricalRates(fromDateString, toDateString,baseCurrency);
                var totalCounts = result.Rates.Count();
                if (result == null)
                {
                    return NotFound();
                }

                result.Rates = result.Rates.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToDictionary(k => k.Key, v => v.Value);

                var pagedData = new
                {
                    TotalRecords = totalCounts,
                    Data = result
                };

                this._logger.Info("End of HistoricalRates.");
                return this.Ok(pagedData);
            }
            catch (Exception ex)
            {
                this._logger.Error("Error occured in HistoricalRates {0}", ex);
                return this.BadRequest("An error has occured. Please try later.");
            }
        }
    }
}