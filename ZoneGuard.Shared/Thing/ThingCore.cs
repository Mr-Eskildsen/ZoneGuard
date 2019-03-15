using Microsoft.Extensions.Logging;
using System;
using ZoneGuard.Shared.Config;
using ZoneGuard.Shared.Interface;

namespace ZoneGuard.Shared.Thing
{
    public abstract class ThingCore : IDisposable
    {
        private ConfigCore config;
        
        private ILogger logger;
        private IManager manager;

        protected ThingCore(IManager manager) //, ILogger<ApplicationLifetimeHostedService> logger)
        {
            this.manager = manager;
            this.logger = manager.getLoggerFactory().CreateLogger(GetType().FullName);
            this.config = null;
            Activated = true;
            
            Initialize();
        }


        public ThingCore(ConfigCore config, IManager manager) //, ILogger<ApplicationLifetimeHostedService> logger)
        {
            this.manager = manager;
            this.logger = manager.getLoggerFactory().CreateLogger(GetType().FullName);
            this.config = config;
            
            Id = config.Id;

            Activated = true;
            Initialize();
        }

        protected ILogger getLogger() { return logger; }

        public string Id
        {
            get;
            protected set;
        }

        
        public bool Activated
        {
            get;
            set;
        }
        

        public T getConfig<T>() where T : ConfigCore
        {
            return (T)config;
        }

        public void Initialize()
        {
            onInitialize();
        }

        public void Destroy()
        {
            onDestroy();
        }

        protected IManager getManager() { return manager; }
        protected abstract void onInitialize();
        protected abstract void onDestroy();

        public static ThingCore Create(ConfigCore config)
        {
            ThingCore obj = null;

            return obj;

        }

        public virtual void Dispose()
        {
        }
    }
}
