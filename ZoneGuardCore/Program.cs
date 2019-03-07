using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ZoneGuard.Core;
using ZoneGuard.Shared.Daemon;

namespace ZoneGuardCore
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddEnvironmentVariables();

        if (args != null)
        {
            config.AddCommandLine(args);
        }
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddOptions();
        services.Configure<ServiceConfig>(hostContext.Configuration.GetSection("Daemon"));
        services.AddSingleton<IHostedService, ZoneGuardAlarmManagerService>();

        services.Configure<ServiceConfig>(hostContext.Configuration.GetSection("MQTT"));
        services.AddSingleton<IHostedService, ZoneGuardMQTTService>();
    })
    .ConfigureLogging((hostingContext, logging) => {
//        logging.SetMinimumLevel(LogLevel.Debug);
        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
        logging.AddConsole();
    });

            await builder.RunConsoleAsync();
        }
    }
}
