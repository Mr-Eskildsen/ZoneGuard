﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;


using ZoneGuard.Shared.Interface;
using ZoneGuard.Shared.Thing.Service;
using ZoneGuard.Shared.Thing.Sensor;
using ZoneGuard.Shared.Config;
using ZoneGuard.Shared.Message;

// https://www.blaize.net/2018/08/creating-a-daemon-with-net-core-part-1/
// https://stackify.com/serilog-tutorial-net-logging/
namespace ZoneGuard.Shared.Daemon
{
    public abstract class CoreDaemonService : IHostedService, IDisposable, IZoneGuardDaemon, IManager
    {
        private readonly IOptions<ServiceConfig> _config;
        private readonly ILogger _logger;
        private IApplicationLifetime appLifetime;
        //private ILogger<HomeMgrLifetimeHostedService> logger;
        private IHostingEnvironment environment;
        private IConfiguration configuration;

        public static string SERVICE_ID_MQTT = "MQTT";
        public static string SERVICE_ID_MQ = "MQ";

        public Dictionary<String, SensorCore> dictSensors;
        public Dictionary<String, SensorCore> dictMqttSensors;
        private Dictionary<String, ServiceCore> dictServices;

        protected ServiceMQ ServiceMQ { get { return (ServiceMQ)getServiceByName(SERVICE_ID_MQ); } }
        protected ILogger Logger { get { return _logger; } }

        public CoreDaemonService(IConfiguration configuration, IHostingEnvironment environment, ILogger<CoreDaemonService> logger, IOptions<ServiceConfig> config, IApplicationLifetime appLifetime)
        {
            _config = config;
            _logger = logger;
            
            this.appLifetime = appLifetime;
            this.environment = environment;
            
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting service: " + _config.Value.Name);

            
            this.appLifetime.ApplicationStarted.Register(OnCoreStarted);
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

            foreach(SensorCore sensor in dictMqttSensors.Values)
            {
                sensor.Dispose();
            }
            dictMqttSensors.Clear();

            foreach (SensorCore sensor in dictSensors.Values)
            {
                sensor.Dispose();
            }
            dictSensors.Clear();

            foreach (ServiceCore service in dictServices.Values)
            {
                service.Dispose();
            }
            dictServices.Clear();
        }

        protected void OnCoreStarted()
        {
            _logger.LogDebug("OnStarted method called.");

            // Instantiating dictionaries
            dictMqttSensors = new Dictionary<String, SensorCore>();
            dictSensors = new Dictionary<String, SensorCore>();
            dictServices = new Dictionary<String, ServiceCore>();

            OnInitializeDatabase();

            OnInitializeServices();


            OnSetupMessageQueue();
            
            OnInitializing();

            OnStarted();
        }


        protected virtual void OnStopping()             {}
        protected virtual void OnStopped()              {}
        protected virtual void OnStarted()              { } 
        protected abstract void OnInitializeDatabase();
        protected abstract void OnInitializeServices();
        protected abstract void OnInitializing();
        
        protected abstract void OnSetupMessageQueue(/*ServiceMQ serviceMQ*/);



        protected void addMQTTSensor(SensorCore sensor, bool replace)
        {
            if ((replace) && (dictMqttSensors.ContainsKey(sensor.Id)))
                dictMqttSensors.Remove(sensor.Id);

            _logger.LogDebug("Adding MQTT sensor Id='{0}', NodeId='{1}', Activated='{2}'", sensor.Id, sensor.NodeId, sensor.Activated);
            dictMqttSensors.Add(sensor.Id, sensor);
        }


        protected void addProxySensor(SensorCore sensor, bool replace)
        {
            if ((replace) && (dictSensors.ContainsKey(sensor.Id)))
                dictSensors.Remove(sensor.Id);

            //_logger.LogDebug("Adding Proxy sensor Id='{0}', NodeId='{1}', Activated='{2}'", sensor.Id, sensor.NodeId, sensor.Activated);
            dictSensors.Add(sensor.Id, sensor);
        }

        protected void addService(ServiceCore service)
        {
            dictServices.Add(service.Id, service);
        }

        public SensorCore getSensorByName(String name)
        {
            return dictSensors[name];
        }


        public ServiceCore getServiceByName(String name)
        {
            if (!dictServices.ContainsKey(name))
                return null;
            return dictServices[name];
        }

        public abstract ILoggerFactory getLoggerFactory();



        public void PublishClientConfig(ConfigSensor cs)
        {
            //TODO:: MIGRATE
            //ServiceMQ.Publish(ServiceMQ.EXCHANGE_CONTROL, cs.NodeId.ToString(), cs.toJSON());
        }


        public void PublishSensorState(SensorStateMessage ssm)
        {
            ServiceMQ.Publish(ServiceMQ.EXCHANGE_STATE, "", ssm.toJSON());

        }


    }
}
