namespace CurrencyManager.Models.SystemModels
{
    public class LoggerFacadeFactory
    {
        private static ILoggerFactory _factory;

        public static void Initialize(ILoggerFactory factory)
        {
            _factory = factory;
        }

        internal static ILogger<T> CreateLogger<T>()
        {
            return _factory.CreateLogger<T>();
        }

        internal static ILogger CreateLogger(string categoryName)
        {
            return _factory.CreateLogger(categoryName);
        }

        public static LoggerFacade<T> CreateLoggerFacade<T>()
        {
            return new LoggerFacade<T>(CreateLogger<T>());
        }
    }
}
