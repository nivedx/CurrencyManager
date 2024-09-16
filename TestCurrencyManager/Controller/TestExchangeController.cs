using CurrencyManager.Controllers;
using CurrencyManager.Models.ProxyModels;
using CurrencyManager.Models.SystemModels;
using CurrencyManager.Proxies.CurrencyExchangeProxy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestCurrencyManager.Controller
{
    [TestFixture]
    public class TestExchangeController
    {
        private MockRepository mockRepository;
        private Mock<ICurrencyExchangeProxy> mockCurrencyExchangeProxy;
        private Mock<ILogger<ExchangeController>> _logger;

        [SetUp]
        public void SetUp()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.mockCurrencyExchangeProxy = this.mockRepository.Create<ICurrencyExchangeProxy>();
            this._logger = new Mock<ILogger<ExchangeController>>();
            this._logger.Setup(b => b.IsEnabled(It.IsAny<LogLevel>()))
            .Returns(true);
        }

        private ExchangeController CreateController()
        {
            LoggerFacade<ExchangeController> objlogger = new LoggerFacade<ExchangeController>(_logger.Object);
            return new ExchangeController(
                  objlogger,
                this.mockCurrencyExchangeProxy.Object
                );
        }
        #region Data Preparation
        
        private ExchangeRate fetchExchangeRateDtoObject()
        {
            ExchangeRate data = new ExchangeRate()
            {
                Amount = 1,
                Base = "EUR",
                Date = "2020-01-01",
                Rates = new Dictionary<string, double>
                {
                   { "EUR", 1.222 },
                   { "USD", 2.333 }
                }
            };
            return data;
        }
        private HistoricalExchangeRate fetchHistoricalExchangeRateDtoObject()
        {
            HistoricalExchangeRate data = new HistoricalExchangeRate()
            {
                Amount = 1,
                Base = "EUR",
                Start_Date = "2020-01-01",
                End_Date = "2020-01-02",
                Rates = new Dictionary<string, Dictionary<string,double>>
                {
                    { "2020-01-01",new Dictionary<string, double>{ { "EUR", 1.222 }, { "USD", 2.333 } } },
                    { "2020-01-02",new Dictionary<string, double>{ { "EUR", 1.222 }, { "USD", 2.333 } } }
                }
            };
            return data;
        }
        #endregion

        [TestCase("EUR")]
        public async Task FetchLatestRatesForBaseCurrencyTest(string baseCurrency)
        {
            //Arrange
            this.mockCurrencyExchangeProxy.Setup(m => m.GetLatestRatesForBaseCurrency(It.IsAny<string>()))
                .ReturnsAsync(fetchExchangeRateDtoObject);

            //Act
            var paymentController = CreateController();

            var results = await paymentController.FetchLatestRatesForBaseCurrency(baseCurrency);

            //Assert
            var okObjectResults = results as OkObjectResult;
            ClassicAssert.AreEqual(okObjectResults.StatusCode, 200);
            ClassicAssert.IsNotNull(okObjectResults);
        }

        [TestCase("EUR","USD,EUR")]
        public async Task ConvertCurrencyTest(string from,string? to)
        {
            //Arrange
            this.mockCurrencyExchangeProxy.Setup(m => m.ConvertCurrency(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(fetchExchangeRateDtoObject);

            //Act
            var paymentController = CreateController();

            var results = await paymentController.ConvertCurrency(from,to);

            //Assert
            var okObjectResults = results as OkObjectResult;
            ClassicAssert.AreEqual(okObjectResults.StatusCode, 200);
            ClassicAssert.IsNotNull(okObjectResults);
        }

        [TestCase("2020-01-01", "2020-01-02","EUR",1,10)]
        public async Task HistoricalRatesTest(string from, string? to, string baseCurrency, int pageNumber = 1, int pageSize = 10)
        {
            //Arrange
            this.mockCurrencyExchangeProxy.Setup(m => m.HistoricalRates(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(fetchHistoricalExchangeRateDtoObject);

            //Act
            var paymentController = CreateController();

            var results = await paymentController.HistoricalRates(from, to,baseCurrency,pageNumber,pageSize);

            //Assert
            var okObjectResults = results as OkObjectResult;
            ClassicAssert.AreEqual(okObjectResults.StatusCode, 200);
            ClassicAssert.IsNotNull(okObjectResults);
        }
    }
}
