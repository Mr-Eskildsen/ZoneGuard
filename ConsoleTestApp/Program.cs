using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ZoneGuard.Shared.Daemon;

namespace ZoneGuard.ConsoleTestApp
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            ZoneGuardTestService ts;

            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddEnvironmentVariables();

        if (args != null)
        {
            config.AddCommandLine(args);
        }
        //config.AddJsonFile("appsettings.json");
    })


    .ConfigureServices((hostContext, services) =>
    {
        /*
        var configBuilder = new ConfigurationBuilder();
        configBuilder
            //.SetBasePath(env.ContentRootPath)
            .AddJsonFile("mqtt_settings.json", optional: true, reloadOnChange: true)
            //.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();
        */

        services.AddOptions();
        
        services.Configure<ServiceConfig>(hostContext.Configuration.GetSection("Daemon"));
        services.AddHostedService<ZoneGuardTestService>();
    })
    .ConfigureLogging((hostingContext, logging) => {


        logging.SetMinimumLevel(LogLevel.Debug);
        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
        logging.AddConsole();
    });

            await builder.RunConsoleAsync();
        }
    }
}
