namespace CurrencyManager.Models.SystemModels.Interfaces
{
    public interface ILoggerFacade
    {
        void Trace(string message, params object[] args);

        void Debug(string message, params object[] args);

        void Info(string message, params object[] args);

        void Warn(string message, params object[] args);

        void Error(string message, params object[] args);

        void Error(Exception ex);
    }
}
