using CurrencyManager.Helpers;

namespace CurrencyManager.Models.ProxyModels
{
    public class ExchangeRate
    {        
        public double Amount { get; set; }
        public string Base { get; set; }
        public string Date { get; set; }
        public IDictionary<string, double> Rates { get; set; }
    }
}
