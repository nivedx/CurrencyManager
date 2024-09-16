using CurrencyManager.Models.SystemModels.Interfaces;
using System.Diagnostics;

namespace CurrencyManager.Models.SystemModels
{
    public class TelemetryHandler<T> : DelegatingHandler
    {
        private readonly ILoggerFacade _logger;

        private readonly TelemetryService _telemetry;

        public TelemetryHandler(LoggerFacade<T> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            using (TelemetryService.Of("proxy_call:" + request.RequestUri!.AbsolutePath))
            {
                return await base.SendAsync(request, cancellationToken);
            }
        }
    }

    public class TelemetryService
    {
        private static LoggerFacade<TelemetryService> _logger;

        public static TelemetryScope Of(string scopeName)
        {
            if (_logger == null)
            {
                _logger = LoggerFacadeFactory.CreateLoggerFacade<TelemetryService>();
            }

            return new TelemetryScope(scopeName, _logger);
        }
    }

    public class TelemetryScope : IDisposable
    {
        private readonly string _scopeName;

        private readonly Stopwatch _watch;

        private readonly LoggerFacade<TelemetryService> _logger;

        public string Name => _scopeName;

        public TelemetryScope(string scopeName, LoggerFacade<TelemetryService> logger)
        {
            _scopeName = scopeName;
            _watch = new Stopwatch();
            _watch.Start();
            _logger = logger;
            _logger.Debug("Entering {0}", scopeName);
        }

        public void Dispose()
        {
            _watch.Stop();
            long elapsedMilliseconds = _watch.ElapsedMilliseconds;
            TelemetrySampler.Instance.Sample(this, elapsedMilliseconds);
            _logger.Debug("Exiting {0} in {1}ms", _scopeName, elapsedMilliseconds);
        }
    }

    public class TelemetrySampler
    {
        private static readonly Lazy<TelemetrySampler> _sampler = new Lazy<TelemetrySampler>(() => new TelemetrySampler());

        private readonly Dictionary<string, TelemetryStats> statsMap = new Dictionary<string, TelemetryStats>();

        public static TelemetrySampler Instance => _sampler.Value;

        private TelemetrySampler()
        {
        }

        public void Sample(TelemetryScope scope, long time)
        {
            TelemetryStats telemetryStats = (statsMap.ContainsKey(scope.Name) ? statsMap[scope.Name] : new TelemetryStats(scope.Name));
            telemetryStats.AddSample(time);
        }
    }

    public class TelemetryStats
    {
        public string Scope { get; private set; }

        public long Min { get; private set; }

        public long Max { get; private set; }

        public long Avge { get; private set; }

        public long Count { get; private set; }

        public TelemetryStats(string name)
        {
            Scope = name;
            Min = long.MaxValue;
            Max = long.MinValue;
            Count = 0L;
        }

        internal void AddSample(long time)
        {
            Min = Math.Min(Min, time);
            Max = Math.Max(Max, time);
            Count++;
            Avge = (Min + Max) / 2;
        }
    }
}
