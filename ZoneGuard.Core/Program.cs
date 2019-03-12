using System.Collections.Generic;
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
        config.AddJsonFile("appsettings.json");
    })

  
    .ConfigureServices((hostContext, services) =>
    {
    
        services.AddOptions();

        services.Configure<ServiceConfig>(hostContext.Configuration.GetSection("Daemon"));
        services.AddHostedService<ZoneGuardAlarmManagerService>();
        /*
        // DETTE VIRKER MEN BØR GØRES Anderledes:
        //https://github.com/simpleinjector/SimpleInjector/issues/596

        //    https://stackoverflow.com/questions/51254053/how-to-inject-a-reference-to-a-specific-ihostedservice-implementation/51314147

        //services.AddOptions();
        services.Configure<ConfigServiceMQTT>(hostContext.Configuration.GetSection("MQTT"));
        services.AddSingleton<IHostedService, ZoneGuardMQTTService>();
        //services.AddHostedService<ZoneGuardMQTTService>();
        */
        //ZoneGuardMQTTService t = (ZoneGuardMQTTService)sc;

        //services.AddHostedService<ZoneGuardMQTTService>();



        //Configuration = var configBuilder.Build();

        //.SetBasePath(Directory.GetCurrentDirectory())
        //          .AddJsonFile("YouAppSettingFile.json")
        /*
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string>
     {
            { "host", "192.168.xxx.xxx"},
            { "port", "1883"},
            { "user", "My User"},
            { "password", "Very Secret"},
            { "qos", "2" }
     });

        
*/

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
