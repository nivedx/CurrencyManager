using CurrencyManager.Models.SystemModels;
using CurrencyManager.Proxies.CurrencyExchangeProxy;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using Microsoft.AspNetCore.Mvc;
using CurrencyManager.Proxies.CacheProxy;

namespace CurrencyManager.Initializers
{
    public class CurrencyManagerInitializer
    {
        private readonly IServiceCollection services;

        public CurrencyManagerInitializer(IServiceCollection services)
        {
            this.services = services;
        }

        public void Initialize()
        {
            
            this.RegisterCore();
            this.InitializeProxies();
        }

        public void RegisterCore()
        {
            services.AddScoped(typeof(LoggerFacade<>));
            services.AddScoped(typeof(TelemetryService));
            services.AddTransient(typeof(TelemetryHandler<>));
            services.AddMvcCore().ConfigureApiBehaviorOptions(delegate (ApiBehaviorOptions options)
            {
                options.SuppressModelStateInvalidFilter = true;
            });
        }

        public void InitializeProxies()
        {
            var handlerLifetimeInMins = 5;
            this.services.AddHttpClient<ICurrencyExchangeProxy, CurrencyExchangeProxy>()
                    .SetHandlerLifetime(TimeSpan.FromMinutes(handlerLifetimeInMins))
                    .AddHttpMessageHandler<TelemetryHandler<ICurrencyExchangeProxy>>()
                    .AddPolicyHandler(GetTimeoutPolicy())
                    .AddPolicyHandler(GetRetryPolicy())
                    .AddPolicyHandler(GetCircuitBreakerPolicy());

            this.services.AddScoped<ICacheProxy, CacheProxy>();
            this.services.AddMemoryCache();
        }

        public void InitializeServices()
        {

        }

        internal static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
        {
            var timeoutDurationInSec = 10;
            return Policy.TimeoutAsync<HttpResponseMessage>(timeoutDurationInSec);
        }

        internal static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            var retryCount = 3;

            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>()
                .WaitAndRetryAsync(retryCount, retryAttempt =>
                {
                    Console.WriteLine("Retry {0} attempt ", retryAttempt);
                    return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                });
        }

       
        internal static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            var breakAfterAttempts = 5;
            var breakDurationInSec = 5 * 60;

            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(breakAfterAttempts, TimeSpan.FromSeconds(breakDurationInSec));
        }
    }
}
