using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using ZoneGuard.Shared.Daemon;
using ZoneGuard.Shared.Interface;

namespace ZoneGuard.Core
{
    public class ZoneGuardAlarmManagerService : CoreDaemonService
    {
        public ZoneGuardAlarmManagerService(IConfiguration configuration, IHostingEnvironment environment, ILogger<ZoneGuardAlarmManagerService> logger, IOptions<ServiceConfig> config, IApplicationLifetime appLifetime) 
            : base(configuration, environment, logger, config, appLifetime)
        {
        }

    

        protected override void OnStopping()
        {
            Logger.LogDebug("OnStopping method called.");

            // On-stopping code goes here  
        }

        protected override void OnStopped()
        {
            Logger.LogDebug("OnStopped method called.");

            // Post-stopped code goes here  
        }

        protected override void OnInitializing()
        {
            Logger.LogDebug("OnInitializing method called.");
        }

        protected override void OnInitialized()
        {
            Logger.LogDebug("OnInitialized method called.");
        }
    }
}
