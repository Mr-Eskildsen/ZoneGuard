using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZoneGuard.Shared.Interface;

// https://www.blaize.net/2018/08/creating-a-daemon-with-net-core-part-1/
// https://stackify.com/serilog-tutorial-net-logging/
namespace ZoneGuard.Shared.Daemon
{
    public abstract class CoreDaemonService : IHostedService, IDisposable, IZoneGuardDaemon
    {
        private readonly ILogger _logger;

        private readonly IOptions<ServiceConfig> _config;


        private IApplicationLifetime appLifetime;
        //private ILogger<HomeMgrLifetimeHostedService> logger;
        private IHostingEnvironment environment;
        private IConfiguration configuration;

        protected ILogger Logger { get { return _logger; } }

        public CoreDaemonService(IConfiguration configuration, IHostingEnvironment environment, ILogger<CoreDaemonService> logger, IOptions<ServiceConfig> config, IApplicationLifetime appLifetime)
        {
            _logger = logger;
            _config = config;
            
            this.appLifetime = appLifetime;
            this.environment = environment;
            
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting service: " + _config.Value.Name);

            this.appLifetime.ApplicationStarted.Register(OnStarted);
            this.appLifetime.ApplicationStopping.Register(OnStopping);
            this.appLifetime.ApplicationStopped.Register(OnStopped);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Stopping daemon.");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _logger.LogDebug("Disposing....");

        }

        protected void OnStarted()
        {
            _logger.LogDebug("OnStarted method called.");

            // Instantiating dictionaries
            //dictSensors = new Dictionary<String, SensorCore>();
            //dictServices = new Dictionary<String, ServiceCore>();

            OnInitializing();


            OnInitialized();
        }


        protected virtual void OnStopping()             {}
        protected virtual void OnStopped()              {}
        protected virtual void OnInitializing()         {}
        protected virtual void OnInitialized()          {}



    }
}
