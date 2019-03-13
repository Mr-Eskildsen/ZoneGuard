//using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using ZoneGuard.ConsoleTestApp.Database;
using ZoneGuard.DAL.Data;
using ZoneGuard.DAL.Models.Config;
using ZoneGuard.Shared.Config;
using ZoneGuard.Shared.Daemon;
using ZoneGuard.Shared.Thing.Service;

namespace ZoneGuard.ConsoleTestApp
{
    public class ZoneGuardTestService : CoreDaemonService
    {
        private NodeDAL node = null;

        //private Dictionary<String, ServiceCore> dictServices;
        private ILoggerFactory loggerFactory;

        public ZoneGuardTestService(IConfiguration configuration, IHostingEnvironment environment, ILogger<ZoneGuardTestService> logger, IOptions<ServiceConfig> config, IApplicationLifetime appLifetime)
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


        

        protected override void OnInitializeDatabase()
        {
            ensureDatabaseCreated();
            ClearConfigTables();
        }

        protected override void OnSetupMessageQueue(/*ServiceMQ serviceMQ*/)
        {
        }



        protected override void OnInitializeServices()
        {

            
        }

        protected override void OnInitializing()
        {
            Logger.LogDebug("OnInitializing method called.");

        }

        protected override void OnStarted()
        {
            Logger.LogDebug("OnStarted method called.");

            CreateConfiguration_Services();
            CreateConfiguration_Sensors();
            CreateConfiguration_AlarmZones();


        }


        private void ensureDatabaseCreated()
        {
            ZoneGuardConfigContextFactory factory = new ZoneGuardConfigContextFactory();


            using (ZoneGuardConfigContext context = factory.CreateDbContext())
            {
                context.Database.EnsureCreated();
            }
        }
        //https://www.bmwbayer.de

        private void ClearConfigTables()
        {
            ZoneGuardConfigContextFactory factory = new ZoneGuardConfigContextFactory();

            using (ZoneGuardConfigContext ConfigContext = factory.CreateDbContext())
            {
                ConfigContext.Thing.RemoveRange(ConfigContext.Set<ThingDAL>());
                ConfigContext.AlarmZone.RemoveRange(ConfigContext.Set<AlarmZoneDAL>());

                ConfigContext.SaveChanges();
            }

        }

        private void CreateConfiguration_Services()
        {
            ZoneGuardConfigContextFactory factory = new ZoneGuardConfigContextFactory();
            Guid serverNodeGuid = new Guid("5c7a6580-45fc-4402-9ebe-07de9884f5b0");


            using (ZoneGuardConfigContext context = factory.CreateDbContext())
            {
                node = null;

                if (!context.Node.Any())
                {
                    node = new NodeDAL { UniqueIdentifier = serverNodeGuid, Name = "Server" };
                    context.Node.Add(node);

                    context.SaveChanges();
                }
                else
                {
                    node = context.Node.Where<NodeDAL>(n => n.UniqueIdentifier == serverNodeGuid).FirstOrDefault<NodeDAL>();
                }


                ThingDAL thing;
                thing = AddService(context, "MQ", "MQ Client", ThingType.Service, new Dictionary<string, string>{
                                                                                                                    {"category", "service" },
                                                                                                                    { "config_class", "ConfigServiceMQ" },
                                                                                                                    { "thing_class", "ServiceMQ" },
                                                                                                                    { "name", "MQ" },
                                                                                                                    { "host", "192.168.1.110" },
                                                                                                                    { "user", "svc-homemanager" },
                                                                                                                    { "password", "HomeManager01" },
                                                                                                                    { "vhost", "zoneguard" }
                                                                                                                    });


                //ConfigServiceMQTT csMQTT = new ConfigServiceMQTT(paramMQTT);
                //addService(new ServiceMQTT(new ConfigServiceMQTT(paramMQTT), this));

                thing = AddService(context, "MQTT", "MQTT Client", ThingType.Service, new Dictionary<string, string>{
                                                                                                                    {"category", "service" },
                                                                                                                    { "config_class", "ConfigServiceMQTT" },
                                                                                                                    { "thing_class", "ServiceMQTT" },
                                                                                                                    { "name", "MQTT" },
                                                                                                                    { "host", "192.168.1.50" }/*,
                                                                                                                    { "user", "svc-homemanager" },
                                                                                                                    { "password", "HomeManager01" }*/
                                                                                                                    });

            }
        }




        private void CreateConfiguration_Sensors()
        {
            string topicOffset = "test/";
            ThingDAL thing;
            ZoneGuardConfigContextFactory factory = new ZoneGuardConfigContextFactory();

            
            using (ZoneGuardConfigContext context = factory.CreateDbContext())
            {
                
                thing = AddMQTTSensor(context, node, "LivingroomWindow1", "Window in Livingroom", ThingType.Sensor, "window", true, topicOffset);
                thing = AddMQTTSensor(context, node, "LivingroomWindow2", "Window in Livingroom", ThingType.Sensor, "window", true, topicOffset);
                thing = AddMQTTSensor(context, node, "LivingroomWindow3", "Window in Livingroom", ThingType.Sensor, "window", true, topicOffset);
                thing = AddMQTTSensor(context, node, "LivingroomDoor1", "Door in Livingroom", ThingType.Sensor, "door", true, topicOffset);

                thing = AddMQTTSensor(context, node, "OfficeWindow1", "Window in Office", ThingType.Sensor, "window", true, topicOffset);

                thing = AddMQTTSensor(context, node, "HallwayWindow1", "Window in Hallway", ThingType.Sensor, "window", true, topicOffset);
                thing = AddMQTTSensor(context, node, "FrederikWindow1", "Window in Frederiks room", ThingType.Sensor, "window", true, topicOffset);
                thing = AddMQTTSensor(context, node, "CecilieWindow1", "Window in Cecilies room", ThingType.Sensor, "window", true, topicOffset);
                thing = AddMQTTSensor(context, node, "SofieWindow1", "Window in Sofies room", ThingType.Sensor, "window", true, topicOffset);
                thing = AddMQTTSensor(context, node, "BedroomWindow1", "Window in Bedroom", ThingType.Sensor, "window", true, topicOffset);
                thing = AddMQTTSensor(context, node, "BackentranceWindow1", "Window in Backentrance", ThingType.Sensor, "window", true, topicOffset);
                thing = AddMQTTSensor(context, node, "BackentranceDoor1", "Door in Backentrance", ThingType.Sensor, "door", true, topicOffset);

                thing = AddMQTTSensor(context, node, "BathroomWindow1", "Window in Bathroom", ThingType.Sensor, "window", true, topicOffset);
                thing = AddMQTTSensor(context, node, "EntranceDoor1", "Door in Entrance", ThingType.Sensor, "door", true, topicOffset);

                thing = AddMQTTSensor(context, node, "KitchenWindow1", "Window in Kitchen", ThingType.Sensor, "window", true, topicOffset);
                //                thing = AddMQTTSensor(context, "BackentranceWindow1", "Window in Backentrance", ThingType.Sensor, "window", true, topicOffset);

                thing = AddMQTTSensor(context, node, "HallwayPir1", "Pir sensor 1 in Hallway", ThingType.Sensor, "pir", true, topicOffset);
                thing = AddMQTTSensor(context, node, "HallwayPir2", "Pir sensor 2 in Hallway", ThingType.Sensor, "pir", true, topicOffset);

                thing = AddMQTTSensor(context, node, "BackyardPir1", "Pir in Backyard", ThingType.Sensor, "pir", true, topicOffset);
                thing = AddMQTTSensor(context, node, "FrontPir1", "Pir in Front of house", ThingType.Sensor, "pir", true, topicOffset);

                thing = AddMQTTSensor(context, node, "BackyardDoor1", "Door from Carport to backyard", ThingType.Sensor, "door", true, topicOffset);

                
            }
        }





        private void CreateConfiguration_AlarmZones()
        {
            //string topicOffset = "test/";
            //ThingDAL thing;

            ZoneGuardConfigContextFactory factory = new ZoneGuardConfigContextFactory();
            using (ZoneGuardConfigContext context = factory.CreateDbContext())
            {
                AddAlarmZone(context,"Perimeter", "House Fence Zone", new string[] { "LivingroomWindow1", "LivingroomWindow2", "LivingroomWindow3", "LivingroomDoor1", "OfficeWindow1", "HallwayWindow1", "FrederikWindow1", "CecilieWindow1", "SofieWindow1", "BedroomWindow1", "BackentranceWindow1", "BackentranceDoor1", "BathroomWindow1", "EntranceDoor1", "KitchenWindow1" } );
                AddAlarmZone(context, "Front", "Outdoor front", new string[] { "FrontPir1" });
                AddAlarmZone(context, "Carport", "Carport", new string[] { "BackyardDoor1" });
                //AddAlarmZone(context, "Terrace", "Outdoor front", new string[] { "Pir1" });
                AddAlarmZone(context, "Backyard", "Backyard", new string[] { "BackyardPir1" });
            }
        }

#if USE_OLD_CONCEPT_OH_NO
        private void originalloadTestConfigData()
        {
            ZoneGuardConfigContextFactory factory = new ZoneGuardConfigContextFactory();
            Guid serverNodeGuid = new Guid("5c7a6580-45fc-4402-9ebe-07de9884f5b0");

            using (ZoneGuardConfigContext context = factory.CreateDbContext())
            {
                /*
                NodeDAL node = null;

                if (!context.Node.Any())
                {
                    node = new NodeDAL { UniqueIdentifier = serverNodeGuid, Name = "Server" };
                    context.Node.Add(node);

                    context.SaveChanges();
                }
                else
                {
                    node = context.Node.Where<NodeDAL>(n => n.UniqueIdentifier == serverNodeGuid).FirstOrDefault<NodeDAL>();
                }

                */

                // Look for any students.
                SensorDAL thing = null;

                if (!context.Thing.Any())
                {

                    //                  ThingParameterDAL paramCategory = new ThingParameterDAL { Name = "category", Value = "sensor", Timestamp = DateTime.UtcNow };
                    //                   ThingParameterDAL paramConfigClass = new ThingParameterDAL { Name = "config_class", Value = "ConfigSensor", Timestamp = DateTime.UtcNow };
                    //                    ThingParameterDAL paramThingClass= new ThingParameterDAL { Name = "thing_class", Value = "SensorMQTT", Timestamp = DateTime.UtcNow };

                    List<SensorDAL> sensorsPerimeter = new List<SensorDAL>();
                    //List<String> sensorsPerimeter = new List<String>(); 
                    List<SensorDAL> sensorsHighRisk = new List<SensorDAL>();

                    String topicOffset = "test/";

                    SensorParameterDAL[] thingParameters;


                    thing = new SensorDAL { Name = "OfficeWindow1", Description = "Window in Office", ThingType = ThingType.Sensor/*, Node = node*/, Timestamp = DateTime.UtcNow };
                    sensorsPerimeter.Add(thing);
                    sensorsHighRisk.Add(thing);
                    thingParameters = new SensorParameterDAL[]
                    {
                        new SensorParameterDAL{Name="category", Value="sensor", Timestamp=DateTime.UtcNow},
                        new SensorParameterDAL{Name="config_class", Value="ConfigSensor", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="thing_class", Value="SensorMQTT", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="name", Value = "OfficeWindow1", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="topic", Value = topicOffset + "officewindow1/state", Timestamp=DateTime.UtcNow }
                    };
                    DbInitializer.AddThing(context, thing, thingParameters);

                    /* ***********
                     * 
                     * Sensor 2
                     * 
                     * ************/

                    thing = new SensorDAL { Name = "LivingroomWindow1", Description = "First Window in Livingroom", ThingType = ThingType.Sensor/*, Node = node*/, Timestamp = DateTime.UtcNow };
                    sensorsPerimeter.Add(thing);
                    thingParameters = new SensorParameterDAL[]
                    {
                        new SensorParameterDAL{Name="category", Value="sensor", Timestamp=DateTime.UtcNow},
                        new SensorParameterDAL{Name="config_class", Value="ConfigSensor", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="thing_class", Value="SensorMQTT", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="name", Value = "LivingroomWindow1", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="topic", Value = topicOffset + "livingroomwindow1/state", Timestamp=DateTime.UtcNow }
                    };
                    DbInitializer.AddThing(context, thing, thingParameters);


                    thing = AddThing(context, "LivingroomWindow2", "Second Window in Livingroom", ThingType.Sensor, new Dictionary<string, string>{ { "category", "sensor" },
                                                                                                                                            { "config_class", "ConfigSensor" },
                                                                                                                                            { "thing_class", "SensorMQTT" },
                                                                                                                                            { "name", "LivingroomWindow2"},
                                                                                                                                            { "topic", topicOffset + "livingroomwindow2/state" }});

                    sensorsPerimeter.Add(thing);
                    /*
                    thing = new ThingDAL { Name = "LivingroomWindow2", Description = "Second Window in Livingroom", ThingType = ThingType.Sensor, Timestamp = DateTime.UtcNow };
                    sensorsPerimeter.Add(thing);
                    thingParameters = new ThingParameterDAL[]
                    {
                        new ThingParameterDAL{Name="category", Value="sensor", Timestamp=DateTime.UtcNow},
                        new ThingParameterDAL{Name="config_class", Value="ConfigSensor", Timestamp=DateTime.UtcNow },
                        new ThingParameterDAL{Name="thing_class", Value="SensorMQTT", Timestamp=DateTime.UtcNow },
                        new ThingParameterDAL{Name="name", Value = "LivingroomWindow2", Timestamp=DateTime.UtcNow },
                        new ThingParameterDAL{Name="topic", Value = topicOffset + "livingroomwindow2/state", Timestamp=DateTime.UtcNow }
                    };
                    DbInitializer.AddThing(context, thing, thingParameters);
                    */

                    thing = new SensorDAL { Name = "LivingroomWindow3", Description = "Third Window in Livingroom", ThingType = ThingType.Sensor/*, Node = node*/, Timestamp = DateTime.UtcNow };
                    sensorsPerimeter.Add(thing);
                    thingParameters = new SensorParameterDAL[]
                    {
                        new SensorParameterDAL{Name="category", Value="sensor", Timestamp=DateTime.UtcNow},
                        new SensorParameterDAL{Name="config_class", Value="ConfigSensor", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="thing_class", Value="SensorMQTT", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="name", Value = "LivingroomWindow3", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="topic", Value = topicOffset + "livingroomwindow3/state", Timestamp=DateTime.UtcNow }
                    };
                    DbInitializer.AddThing(context, thing, thingParameters);


                    /* ***********
                     * 
                     * LivingroomDoor1
                     * 
                     * ************/

                    thing = new SensorDAL { Name = "LivingroomDoor1", Description = "Terracedoor in Livingroom", ThingType = ThingType.Sensor/*, Node = node*/, Timestamp = DateTime.UtcNow };
                    sensorsPerimeter.Add(thing);
                    sensorsHighRisk.Add(thing);
                    thingParameters = new SensorParameterDAL[]
                    {
                        new SensorParameterDAL{Name="category", Value="sensor", Timestamp=DateTime.UtcNow},
                        new SensorParameterDAL{Name="config_class", Value="ConfigSensor", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="thing_class", Value="SensorMQTT", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="name", Value = "LivingroomDoor1", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="topic", Value = topicOffset + "livingroomdoor1/state", Timestamp=DateTime.UtcNow }

                    };
                    DbInitializer.AddThing(context, thing, thingParameters);


                    /* ***********
                     * 
                     * FrederikWindow1
                     * 
                     * ************/

                    thing = new SensorDAL { Name = "FrederikWindow1", Description = "Window (Frederik)", ThingType = ThingType.Sensor/*, Node = node*/, Timestamp = DateTime.UtcNow };
                    sensorsPerimeter.Add(thing);
                    thingParameters = new SensorParameterDAL[]
                    {
                        new SensorParameterDAL{Name="category", Value="sensor", Timestamp=DateTime.UtcNow},
                        new SensorParameterDAL{Name="config_class", Value="ConfigSensor", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="thing_class", Value="SensorMQTT", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="name", Value = "FrederikWindow1", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="topic", Value = topicOffset + "frederikwindow1/state", Timestamp=DateTime.UtcNow }
                    };
                    DbInitializer.AddThing(context, thing, thingParameters);


                    /* ***********
                     * CecilieWindow1
                     * ************/
                    thing = new SensorDAL { Name = "CecilieWindow1", Description = "Window (Cecilie)", ThingType = ThingType.Sensor/*, Node = node*/, Timestamp = DateTime.UtcNow };
                    sensorsPerimeter.Add(thing);
                    sensorsHighRisk.Add(thing);
                    thingParameters = new SensorParameterDAL[]
                    {
                        new SensorParameterDAL{Name="category", Value="sensor", Timestamp=DateTime.UtcNow},
                        new SensorParameterDAL{Name="config_class", Value="ConfigSensor", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="thing_class", Value="SensorMQTT", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="name", Value = "CecilieWindow1", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="topic", Value = topicOffset + "ceciliewindow1/state", Timestamp=DateTime.UtcNow }
                    };
                    DbInitializer.AddThing(context, thing, thingParameters);


                    /* ***********
                     * SofieWindow1
                     * ************/
                    thing = new SensorDAL { Name = "SofieWindow1", Description = "Window (Sofie)", ThingType = ThingType.Sensor/*, Node = node*/, Timestamp = DateTime.UtcNow };
                    sensorsPerimeter.Add(thing);
                    sensorsHighRisk.Add(thing);
                    thingParameters = new SensorParameterDAL[]
                    {
                        new SensorParameterDAL{Name="category", Value="sensor", Timestamp=DateTime.UtcNow},
                        new SensorParameterDAL{Name="config_class", Value="ConfigSensor", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="thing_class", Value="SensorMQTT", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="name", Value = "SofieWindow1", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="topic", Value = topicOffset + "sofiewindow1/state", Timestamp=DateTime.UtcNow }
                    };
                    DbInitializer.AddThing(context, thing, thingParameters);


                    /* ***********
                     * BedroomWindow1
                     * ************/
                    thing = new SensorDAL { Name = "BedroomWindow1", Description = "Window (Bedroom)", ThingType = ThingType.Sensor/*, Node = node*/, Timestamp = DateTime.UtcNow };
                    sensorsPerimeter.Add(thing);
                    sensorsHighRisk.Add(thing);
                    thingParameters = new SensorParameterDAL[]
                    {
                        new SensorParameterDAL{Name="category", Value="sensor", Timestamp=DateTime.UtcNow},
                        new SensorParameterDAL{Name="config_class", Value="ConfigSensor", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="thing_class", Value="SensorMQTT", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="name", Value = "BedroomWindow1", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="topic", Value = topicOffset + "bedroomwindow1/state", Timestamp=DateTime.UtcNow }
                    };
                    DbInitializer.AddThing(context, thing, thingParameters);


                    /* ***********
                     * BackentranceWindow1
                     * ************/
                    thing = new SensorDAL { Name = "BackentranceWindow1", Description = "Window (Backentrance)", ThingType = ThingType.Sensor/*, Node = node*/, Timestamp = DateTime.UtcNow };
                    sensorsPerimeter.Add(thing);
                    sensorsHighRisk.Add(thing);
                    thingParameters = new SensorParameterDAL[]
                    {
                        new SensorParameterDAL{Name="category", Value="sensor", Timestamp=DateTime.UtcNow},
                        new SensorParameterDAL{Name="config_class", Value="ConfigSensor", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="thing_class", Value="SensorMQTT", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="name", Value = "BackentranceWindow1", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="topic", Value = topicOffset + "backentrancewindow1/state", Timestamp=DateTime.UtcNow }
                    };
                    DbInitializer.AddThing(context, thing, thingParameters);


                    /* ***********
                     * BackentranceDoor1
                     * ************/
                    thing = new SensorDAL { Name = "BackentranceDoor1", Description = "Door (Backentrance)", ThingType = ThingType.Sensor/*, Node = node*/, Timestamp = DateTime.UtcNow };
                    sensorsPerimeter.Add(thing);
                    sensorsHighRisk.Add(thing);
                    thingParameters = new SensorParameterDAL[]
                    {
                        new SensorParameterDAL{Name="category", Value="sensor", Timestamp=DateTime.UtcNow},
                        new SensorParameterDAL{Name="config_class", Value="ConfigSensor", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="thing_class", Value="SensorMQTT", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="name", Value = "BackentranceDoor1", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="topic", Value = topicOffset + "backentrancedoor1/state", Timestamp=DateTime.UtcNow }
                    };
                    DbInitializer.AddThing(context, thing, thingParameters);


                    /* ***********
                     * BathroomWindow1
                     * ************/
                    thing = new SensorDAL { Name = "BathroomWindow1", Description = "Window (Bathroom)", ThingType = ThingType.Sensor/*, Node = node*/, Timestamp = DateTime.UtcNow };
                    sensorsPerimeter.Add(thing);
                    thingParameters = new SensorParameterDAL[]
                    {
                        new SensorParameterDAL{Name="category", Value="sensor", Timestamp=DateTime.UtcNow},
                        new SensorParameterDAL{Name="config_class", Value="ConfigSensor", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="thing_class", Value="SensorMQTT", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="name", Value = "BathroomWindow1", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="topic", Value = topicOffset + "bathroomwindow1/state", Timestamp=DateTime.UtcNow }
                    };
                    DbInitializer.AddThing(context, thing, thingParameters);


                    /* ***********
                     * EntranceDoor1
                     * ************/
                    thing = new SensorDAL { Name = "EntranceDoor1", Description = "Window (Entrancedoor)", ThingType = ThingType.Sensor/*, Node = node*/, Timestamp = DateTime.UtcNow };
                    sensorsPerimeter.Add(thing);
                    thingParameters = new SensorParameterDAL[]
                    {
                        new SensorParameterDAL{Name="category", Value="sensor", Timestamp=DateTime.UtcNow},
                        new SensorParameterDAL{Name="config_class", Value="ConfigSensor", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="thing_class", Value="SensorMQTT", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="name", Value = "EntranceDoor1", Timestamp=DateTime.UtcNow },
                        new SensorParameterDAL{Name="topic", Value = topicOffset + "frontentrancedoor1/state", Timestamp=DateTime.UtcNow }
                    };
                    DbInitializer.AddThing(context, thing, thingParameters);


                    AlarmZoneDAL alarmZonePerimeter = new AlarmZoneDAL { Name = "Perimeter", Description = "", Enabled = 1, Timestamp = DateTime.UtcNow };
                    List<AlarmZoneThingDAL> alarmZoneSensors = new List<AlarmZoneThingDAL>();

                    foreach (SensorDAL sensor in sensorsPerimeter)
                    {
                        alarmZoneSensors.Add(new AlarmZoneThingDAL { Enabled = 1, Thing = sensor, Timestamp = DateTime.UtcNow });
                    };
                    DbInitializer.AddAlarmZone(context, alarmZonePerimeter, alarmZoneSensors.ToArray());


                    AlarmZoneDAL alarmZoneHighRisk = new AlarmZoneDAL { Name = "High Risk", Description = "", Enabled = 1, Timestamp = DateTime.UtcNow };
                    alarmZoneSensors = new List<AlarmZoneThingDAL>();

                    foreach (SensorDAL sensor in sensorsHighRisk)
                    {
                        alarmZoneSensors.Add(new AlarmZoneThingDAL { Enabled = 1, Thing = sensor, Timestamp = DateTime.UtcNow });
                    };
                    DbInitializer.AddAlarmZone(context, alarmZoneHighRisk, alarmZoneSensors.ToArray());

                }


            }
        }
#endif
        private ThingDAL AddService(ZoneGuardConfigContext context, String Name, String _description, ThingType _type, Dictionary<String, String> parameters)
        {
            ThingDAL thing = null;
            List<ThingParameterDAL> thingParameters = new List<ThingParameterDAL>();
            DateTime timestamp = DateTime.UtcNow;

            thing = new ThingDAL { Name = Name, Description = _description, ThingType = _type, NodeId = null, Timestamp = timestamp };

            foreach (KeyValuePair<String, String> kvp in parameters)
            {
                thingParameters.Add(new ThingParameterDAL { Name = kvp.Key, Value = kvp.Value, Timestamp = timestamp });
            }

            DbInitializer.ThingConfig_Create(context, thing, thingParameters.ToArray());
            return thing;
        }



            //, Dictionary<string, string> parameters
        private ThingDAL AddMQTTSensor(ZoneGuardConfigContext context, NodeDAL node, String Name, String _description, ThingType _type, String sensorType, Boolean isPerimeter, String topicOffset)
        {
            ThingDAL thing = null;
            List<ThingParameterDAL> thingParameters = new List<ThingParameterDAL>();
            DateTime timestamp = DateTime.UtcNow;

            thing = new ThingDAL { Name = Name, Description = _description, ThingType = _type, NodeId = node.Id, Timestamp = timestamp };
            
            thingParameters.Add(new ThingParameterDAL { Name = ConfigCore.PARAMETER_THING_CATEGORY, Value = "sensor", Timestamp = timestamp });
            thingParameters.Add(new ThingParameterDAL { Name = ConfigCore.PARAMETER_CONFIG_CLASS, Value = "ConfigSensor", Timestamp = timestamp });
            thingParameters.Add(new ThingParameterDAL { Name = ConfigCore.PARAMETER_THING_CLASS, Value = "SensorMQTT", Timestamp = timestamp });
            thingParameters.Add(new ThingParameterDAL { Name = ConfigCore.PARAMETER_TYPE, Value = "sensor", Timestamp = timestamp });
            thingParameters.Add(new ThingParameterDAL { Name = ConfigCore.PARAMETER_NAME, Value = Name, Timestamp = timestamp });
            thingParameters.Add(new ThingParameterDAL { Name = ConfigCore.PARAMETER_IS_PERIMETER, Value = isPerimeter.ToString().ToLower() });
            thingParameters.Add(new ThingParameterDAL { Name = ConfigCore.PARAMETER_TOPIC_STATE, Value = topicOffset + Name.ToLower() + "/state" });
            thingParameters.Add(new ThingParameterDAL { Name = ConfigCore.PARAMETER_TOPIC_COMMAND, Value = "" });
           
            DbInitializer.ThingConfig_Create(context, thing, thingParameters.ToArray());
            return thing;
        }


        private AlarmZoneDAL AddAlarmZone(ZoneGuardConfigContext context, String name, String description, String[] sensors)
        {
            AlarmZoneDAL alarmZone = null;
            List<AlarmZoneThingDAL> alarmZoneSensors = new List<AlarmZoneThingDAL>();
            DateTime timestamp = DateTime.UtcNow;

            alarmZone = new AlarmZoneDAL { Name = name, Description = description, Enabled = 1 , Timestamp = timestamp };

            foreach (String sensorName in sensors)
            {
                ThingDAL curSensor = context.Thing.Where<ThingDAL>(n => n.ThingType == ThingType.Sensor).Where<ThingDAL>(n => n.Name == sensorName).FirstOrDefault<ThingDAL>();
                alarmZoneSensors.Add(new AlarmZoneThingDAL { Enabled = 1, Thing = curSensor, Timestamp = timestamp });
            };
            DbInitializer.AddAlarmZone(context, alarmZone, alarmZoneSensors.ToArray());


            return alarmZone;
        }

    }
}
