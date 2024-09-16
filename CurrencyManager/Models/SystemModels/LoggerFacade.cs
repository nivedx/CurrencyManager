using CurrencyManager.Models.SystemModels.Interfaces;
using Newtonsoft.Json;
using System.Text;

namespace CurrencyManager.Models.SystemModels
{
    public class LoggerFacade<T> : ILoggerFacade
    {
        private readonly ILogger _logger;

        public LoggerFacade(ILogger<T> logger)
        {
            _logger = logger;
        }

        public void Debug(string message, params object[] args)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(ConstructLogMessage(message, args));
            }
        }

        public void Error(string message, params object[] args)
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError(ConstructLogMessage(message, args));
            }
        }

        public void Info(string message, params object[] args)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(ConstructLogMessage(message, args));
            }
        }

        public void Trace(string message, params object[] args)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace(ConstructLogMessage(message, args));
            }
        }

        public void Warn(string message, params object[] args)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
            {
                _logger.LogWarning(ConstructLogMessage(message, args));
            }
        }

        public void Error(Exception ex)
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                LogRecord logRecord = new LogRecord();
                logRecord.Stack = ex.StackTrace;
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("Exception : ").Append(ex.Message);
                if (ex.InnerException != null)
                {
                    stringBuilder.AppendLine("Inner Exception : ").Append(ex.InnerException!.Message);
                    logRecord.InnerStack = ex.InnerException!.StackTrace;
                }

                logRecord.Message = stringBuilder.ToString();
                _logger.LogError(JsonConvert.SerializeObject(logRecord));
            }
        }

        private string ConstructLogMessage(string message, object[] args)
        {
            LogRecord logRecord = new LogRecord();
            logRecord.Message = string.Format(message, args);
            return JsonConvert.SerializeObject(logRecord);
        }
    }
}
