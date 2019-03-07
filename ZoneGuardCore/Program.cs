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
        /*
        var configBuilder = new ConfigurationBuilder();
        configBuilder
            //.SetBasePath(env.ContentRootPath)
            .AddJsonFile("mqtt_settings.json", optional: true, reloadOnChange: true)
            //.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();
        */

        services.AddOptions();
        //services.Configure<ServiceConfig>(hostContext.Configuration.GetSection("Daemon"));
        services.AddHostedService<ZoneGuardAlarmManagerService>();


        
        //services.AddOptions();
        services.Configure<ConfigServiceMQTT>(hostContext.Configuration.GetSection("MQTT"));
        //services.AddSingleton<IHostedService, ZoneGuardMQTTService>();
        //services.AddSingleton(hostContext.Configuration.GetSection("MQTT").Get<MQTTConfig>());
        services.AddHostedService<ZoneGuardMQTTService>();



        //Configuration = var configBuilder.Build();

        //.SetBasePath(Directory.GetCurrentDirectory())
        //          .AddJsonFile("YouAppSettingFile.json")
        /*
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string>
     {
            { "host", "192.168.9.50"},
            { "port", "1883"},
            { "user", "svc-alarm"},
            { "password", "#openHAB@Home"},
            { "qos", "2" }
     });

        
        //services.Configure<MQTTConfig>(  hostContext.Configuration.GetSection("MQTT"));
        IConfiguration c = configBuilder.Build();
        MQTTConfig cfg = new MQTTConfig("192.168.9.50", "1883", "svc-alarm", "Secret!");
        System.Action<MQTTConfig> myCfg = new System.Action<MQTTConfig>(cfg);
        services.Configure<MQTTConfig>(myCfg);
        services.AddSingleton<IHostedService, ZoneGuardMQTTService>();
*/
        /*
                services.AddSingleton<IHostedService, ZoneGuardMQTTService>(serviceProvider =>
                {
                    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                    return MassTransit.Bus.Factory.CreateUsingRabbitMq(cfg =>
                    {
                        var host = cfg.Host(new Uri(configuration.GetConnectionString("RabbitMq")), h =>
                        {
                            h.Username("guest");
                            h.Password("guest");
                        });
                    });
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
