using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZoneGuard.Core.Alarm;
using ZoneGuard.Core.Thing.Alarm;
using ZoneGuard.Core.Thing.Sensor;
using ZoneGuard.DAL.Data;
using ZoneGuard.DAL.Models.Config;
using ZoneGuard.Shared.Config;
using ZoneGuard.Shared.Daemon;
using ZoneGuard.Shared.Interface;
using ZoneGuard.Shared.Message;
using ZoneGuard.Shared.Message.Control;
using ZoneGuard.Shared.Thing.Sensor;
using ZoneGuard.Shared.Thing.Service;

namespace ZoneGuard.Core
{
    public class ZoneGuardAlarmManagerService : CoreDaemonService
    {
        private AlarmZoneManager alarmManager  = null;
        //private Dictionary<String, ServiceCore> dictServices;
        private ILoggerFactory loggerFactory;

        public ZoneGuardAlarmManagerService(IConfiguration configuration, IHostingEnvironment environment, ILogger<ZoneGuardAlarmManagerService> logger, IOptions<ServiceConfig> config, IApplicationLifetime appLifetime) 
            : base(configuration, environment, logger, config, appLifetime)
        {
            this.loggerFactory = new LoggerFactory().AddConsole();
                                     //.AddDebug();

        }

        public override ILoggerFactory getLoggerFactory()
        {
            return loggerFactory;
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

        protected override void OnStarted()
        {
            Logger.LogDebug("OnStarted method called.");

            OnSetupAlarmManager();

            LoadFromDatabase();

        }





        protected void OnSetupAlarmManager()
        {
            alarmManager = new MQTTAlarmZoneMgr((ServiceMQTT)getServiceByName("MQTT"));
        }

        protected override void OnSetupMessageQueue()
        {
            ServiceMQ serviceMQ = (ServiceMQ)getServiceByName("MQ");
            serviceMQ.CreateServerControlQueue(callbackControlMessageHandler);
            serviceMQ.CreateStateQueue(callbackStateMessageHandler);
        }

        protected override void OnInitializeDatabase()
        {
            ZoneGuardConfigContextFactory factory = new ZoneGuardConfigContextFactory();

            using (ZoneGuardConfigContext context = factory.CreateDbContext())
            {
                context.Database.EnsureCreated();
            }
        }




        protected async override void OnInitializeServices()
        {
            ZoneGuardConfigContextFactory factory = new ZoneGuardConfigContextFactory();

            using (ZoneGuardConfigContext context = factory.CreateDbContext())
            {

                /***************************************************************
                 * 
                 *  Load Services
                 * 
                 ***************************************************************/
                List<ThingDAL> services = await context.Thing
                                                            .Where<ThingDAL>(t => t.ThingType == ThingType.Service)
                                                            .Include(p => p.Parameters)
                                                            .ToListAsync<ThingDAL>();
                foreach (ThingDAL serviceDAL in services)
                {
                    Console.WriteLine(serviceDAL.Name);
                    ConfigService configService = ZoneGuardConfigContextFactory.CreateThingFromDAL<ConfigService>(serviceDAL);

                    Console.WriteLine(configService.toJSON());

#if warning
             
#endif
                    //TODO:: MUST 
                    //TODOO:: MIGRATE                    addSensor(new SensorProxy(configSensor, this), false);
                    ServiceCore service = null;
                    String s = configService.ConfigClass;
                    if (configService.ConfigClass == "ConfigServiceMQTT")
                    {
                        service = new ServiceMQTT((ConfigServiceMQTT)configService, this);
                    }
                    else if (configService.ConfigClass == "ConfigServiceMQ")
                    {
                        service = new ServiceMQ((ConfigServiceMQ)configService, this);
                    }
                    addService(service);
                }
            }
            }





        private async void LoadFromDatabase()
        {
            ZoneGuardConfigContextFactory factory = new ZoneGuardConfigContextFactory();

            using (ZoneGuardConfigContext context = factory.CreateDbContext())
            {

                /***************************************************************
                 * 
                 *  Load Sensors
                 * 
                 ***************************************************************/
                List<ThingDAL> things = await context.Thing
                                                            .Where<ThingDAL>(t => t.ThingType == ThingType.Sensor)
                                                            .Include(p => p.Parameters)
                                                            .ToListAsync<ThingDAL>();
                foreach (ThingDAL thing in things)
                {
                    Console.WriteLine(thing.Name);
                    ConfigSensor configSensor = ZoneGuardConfigContextFactory.CreateThingFromDAL<ConfigSensor>(thing);

                    Console.WriteLine(configSensor.toJSON());

                    //TODO:: More Dynamic
                    addMQTTSensor(new SensorMQTT(configSensor, this), true);

                    addProxySensor(new SensorProxy(configSensor, this), false);

                }



                /***************************************************************
                 * 
                 *  Load AlarmZones
                 * 
                 ***************************************************************/
                
                List<AlarmZoneDAL> alarmZones = await context.AlarmZone
                                                                    .Include(az => az.Sensors)
                                                                    .ThenInclude(pa => pa.Thing)
                                                                    .ToListAsync<AlarmZoneDAL>();


                foreach (AlarmZoneDAL zone in alarmZones)
                {
                    ConfigAlarmZone configAlarmZone = ZoneGuardConfigContextFactory.CreateConfigAlarmZoneFromDAL(zone);
                    
                    AlarmZone alarmZone = new AlarmZone(configAlarmZone, this);
                    alarmManager.addAlarmZone(alarmZone);
                    /*
                                    //if (zone.Enabled == 1)
                                    {
                                        Console.WriteLine("Creating Alarm Zone {0}", zone.Name);
                                        foreach (AlarmZoneThingDAL sensor in zone.Sensors)
                                        {
                                            SensorCore sensorProxy = getSensorByName(sensor.Thing.Name);
                                            SensorLink sensorLink = new SensorLink(sensorProxy, alarmZone, this);


                                            //ConfigLink configLink = new ConfigLink();

                                            //SensorProxy sensorProxy = new SensorProxy(configSensor, this);  
                                            //HEST
                                            //alarmZone.addSensor(sensorProxy);
                                            //if (sensor.Enabled == 1)
                                            //{
                                             //   Console.WriteLine("   Adding Sensor '{0}'", sensor.Thing.Name);
                                           // }
                                            
            }
        }*/
                } 
            }
        }



        void callbackControlMessageHandler(object sender, BasicDeliverEventArgs eventArgs)
        {

            var body = eventArgs.Body;
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(" Control Message Request Arrived: {0}", message);
            MessageControl mc = MessageCore.fromJSON<MessageControl>(message);

            if (mc.Request == ControlRequest.REQUEST_CONFIG)
            {
                foreach (KeyValuePair<String, SensorCore> kvp in dictSensors)
                {
                    SensorCore sc = kvp.Value;
                    if (sc.NodeId == mc.Id)
                    {

                        //Publish sensor config to Client
                        PublishClientConfig(sc.getConfig<ConfigSensor>());

                    }
                }

            }



        }

        void callbackStateMessageHandler(object sender, BasicDeliverEventArgs eventArgs)
        {

            var body = eventArgs.Body;
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(" State Message Arrived: {0}", message);
            SensorStateMessage ssm = MessageCore.fromJSON<SensorStateMessage>(message);
            SensorCore sensor = getSensorByName(ssm.Id);


            ((SensorProxy)sensor).setState(ssm.Triggered);

        }

    }
    
}
