using CurrencyManager.Models.SystemModels;
using NLog.Web;
using System.Reflection.Metadata;

namespace CurrencyManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("Initializing Currency manager logging");
                var host = CreateHostBuilder(args).Build();
                var loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
                LoggerFacadeFactory.Initialize(loggerFactory);
                host.Run();
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
             .ConfigureAppConfiguration((hostingContext, config) =>
             {
                 config.AddJsonFile(
                     "service-config.json",
                     optional: false,
                     reloadOnChange: true);
             })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(LogLevel.Trace);
            })
            .UseNLog();
    }
}