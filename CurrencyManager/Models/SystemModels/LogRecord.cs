using System.Diagnostics;

namespace CurrencyManager.Models.SystemModels
{
    public class LogRecord
    {
        public long Id { get; private set; }

        public string Message { get; set; }

        public string Stack { get; set; }

        public string InnerStack { get; set; }

        public LogRecord()
        {
            Id = Stopwatch.GetTimestamp();
        }
    }
}
