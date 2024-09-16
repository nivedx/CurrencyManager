namespace CurrencyManager.Models.SystemModels
{
    public class BaseException : Exception
    {
        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public BaseException(string errorCode, string errorMessage)
            : base(errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }
    }
}
