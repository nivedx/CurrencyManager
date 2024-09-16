namespace CurrencyManager.Models.SystemModels
{
    public class InvalidConfigurationException : BaseException
    {
        public InvalidConfigurationException(string errorCode, string errorMessage)
            : base(errorCode, errorMessage)
        {
        }
    }
}
