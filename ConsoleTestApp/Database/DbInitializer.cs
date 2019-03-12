using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZoneGuard.DAL.Data;
using ZoneGuard.DAL.Models.Config;
using ZoneGuard.DAL.Models.Log;

namespace ZoneGuard.ConsoleTestApp.Database
{
    public class DbInitializer
    {
        public static void AddAlarmZone(ZoneGuardConfigContext context, AlarmZoneDAL alarmZone, AlarmZoneThingDAL[] alarmZoneThings)
        {
            context.AlarmZone.Add(alarmZone);
            context.SaveChanges();

            foreach (AlarmZoneThingDAL sensor in alarmZoneThings)
            {
                sensor.AlarmZone = alarmZone;
                context.AlarmZoneThing.Add(sensor);
            }
            context.SaveChanges();
        }

        public static void ThingConfig_Create(ZoneGuardConfigContext context, ThingDAL thing, ThingParameterDAL[] parameters)
        {
            context.Thing.Add(thing);
            context.SaveChanges();

            foreach (ThingParameterDAL param in parameters)
            {
                param.Thing = thing;
                context.ThingParameter.Add(param);
            }
            context.SaveChanges();
        }

        /*
        public static void AddServiceConfig(ZoneGuardConfigContext context, ServiceDAL thing, ServiceParameterDAL[] parameters)
        {
            context.Service.Add(thing);
            context.SaveChanges();

            foreach (ServiceParameterDAL param in parameters)
            {
                param.Service = thing;
                context.ServiceParameter.Add(param);
            }
            context.SaveChanges();
        }
        */

        public static void AddSensorStateLog(ZoneGuardConfigContext context, SensorStateLogDAL ssl)
        {
            context.SensorState.Add(ssl);
            context.SaveChanges();
        }

#if USE_OLD_STUFF     
        private void InitializeConfigData_NA(ZoneGuardConfigContext context)
        {
            

            SensorDAL thing = null;

            if (!context.Sensor.Any())
            {
                List<SensorDAL> sensors = new List<SensorDAL>();

                String topicOffset = "test/";

                SensorParameterDAL[] thingParameters;


                thing = new SensorDAL { Name = "OfficeWindow1", Description = "Window in Office", ThingType = ThingType.Sensor/*, Node = node*/, Timestamp = DateTime.UtcNow };
                sensors.Add(thing);
                thingParameters = new SensorParameterDAL[]
                {
                    new SensorParameterDAL{Name="topic", Value="test/officewindows1/state", Timestamp=DateTime.UtcNow}
                    //new ConfigThingParameter{Name="Action 1", ThingType=ThingType.Action, Timestamp=DateTime.UtcNow}
                };
                AddThing(context, thing, thingParameters);

                /* ***********
                 * 
                 * Sensor 2
                 * 
                 * ************/

                thing = new SensorDAL { Name = "LivingroomWindow1", Description = "First Window in Livingroom", ThingType = ThingType.Sensor/*, Node = node*/, Timestamp = DateTime.UtcNow };
                sensors.Add(thing);
                thingParameters = new SensorParameterDAL[]
                {
                    new SensorParameterDAL{Name="topic", Value=topicOffset + "livingroomwindows1/state", Timestamp=DateTime.UtcNow},
                    new SensorParameterDAL{Name="a_value", Value="Some value", Timestamp=DateTime.UtcNow}

                    //new ConfigThingParameter{Name="Action 1", ThingType=ThingType.Action, Timestamp=DateTime.UtcNow}
                };
                AddThing(context, thing, thingParameters);



                thing = new SensorDAL { Name = "Livingroom2", Description = "Second Window in Livingroom", ThingType = ThingType.Sensor/*, Node = node*/, Timestamp = DateTime.UtcNow };
                sensors.Add(thing);
                thingParameters = new SensorParameterDAL[]
                {
                    new SensorParameterDAL{Name="topic", Value=topicOffset + "livingroomwindows2/state", Timestamp=DateTime.UtcNow}
                    //new ConfigThingParameter{Name="Action 1", ThingType=ThingType.Action, Timestamp=DateTime.UtcNow}
                };
                AddThing(context, thing, thingParameters);


                thing = new SensorDAL { Name = "Livingroom3", Description = "Third Window in Livingroom", ThingType = ThingType.Sensor/*, Node = node*/, Timestamp = DateTime.UtcNow };
                sensors.Add(thing);
                thingParameters = new SensorParameterDAL[]
                {
                    new SensorParameterDAL{Name="topic", Value=topicOffset + "livingroomwindows3/state", Timestamp=DateTime.UtcNow}
                    //new ConfigThingParameter{Name="Action 1", ThingType=ThingType.Action, Timestamp=DateTime.UtcNow}
                };
                AddThing(context, thing, thingParameters);



                AlarmZoneDAL alarmZone = null;
                alarmZone = new AlarmZoneDAL { Name = "Perimeter", Description = "", Enabled = 1, Timestamp = DateTime.UtcNow };
                List<AlarmZoneThingDAL> alarmZoneSensors = new List<AlarmZoneThingDAL>();

                foreach (SensorDAL sensor in sensors)
                {


                    alarmZoneSensors.Add(new AlarmZoneThingDAL { Enabled = 1, Thing = sensor, Timestamp = DateTime.UtcNow });
                };
                AddAlarmZone(context, alarmZone, alarmZoneSensors.ToArray());

            }





            SensorStateLogDAL ssl = null;


            thing = context.Thing.Where(s => s.Id == 2).First<SensorDAL>();

            ssl = new SensorStateLogDAL { Triggered = 1, Sensor = thing, CreatedTimestamp = DateTime.UtcNow.AddHours(-1) };
            AddSensorStateLog(context, ssl);

            ssl = new SensorStateLogDAL { Triggered = 0, Sensor = thing, CreatedTimestamp = DateTime.UtcNow.AddHours(-1).AddMinutes(1) };
            AddSensorStateLog(context, ssl);

            return;   // DB has been seeded

            //}
            /*
             * 
             * DETTE VIRKER
            var things = new ConfigThing[]
            {
            new ConfigThing{Name="Sensor 1", ThingType=ThingType.Sensor, Timestamp=DateTime.UtcNow},
            new ConfigThing{Name="Sensor 2", ThingType=ThingType.Sensor, Timestamp=DateTime.UtcNow},
            new ConfigThing{Name="Action 1", ThingType=ThingType.Action, Timestamp=DateTime.UtcNow}
            };
            foreach (ConfigThing s in things)
            {
                context.Thing.Add(s);
                context.SaveChanges();
                int id = s.ID;
                Console.WriteLine("Id is {0}", id);
            }
            context.SaveChanges();
            IEnumerable<ConfigThing> things1 = context.Thing.AsEnumerable<ConfigThing>();
            int count = things.Count<ConfigThing>();
            */

            /*
                        var things = new ConfigThingParameter[]
            {
                        new ConfigThingParameter{ThingId=1, Name="Topic", Key="test/officewindow1/state", Timestamp=DateTime.UtcNow},
                        new ConfigThing{Name="Sensor 2", ThingType=ThingType.Sensor, Timestamp=DateTime.UtcNow},
                        new ConfigThing{Name="Action 1", ThingType=ThingType.Action, Timestamp=DateTime.UtcNow}
            };Ø/

                        /*
                         * 
                        var courses = new Course[]
                        {
                        new Course{CourseID=1050,Title="Chemistry",Credits=3},
                        new Course{CourseID=4022,Title="Microeconomics",Credits=3},
                        new Course{CourseID=4041,Title="Macroeconomics",Credits=3},
                        new Course{CourseID=1045,Title="Calculus",Credits=4},
                        new Course{CourseID=3141,Title="Trigonometry",Credits=4},
                        new Course{CourseID=2021,Title="Composition",Credits=3},
                        new Course{CourseID=2042,Title="Literature",Credits=4}
                        };
                        foreach (Course c in courses)
                        {
                            context.Course.Add(c);
                        }
                        context.SaveChanges();

                        var enrollments = new Enrollment[]
                        {
                        new Enrollment{StudentID=1,CourseID=1050,Grade=Grade.A},
                        new Enrollment{StudentID=1,CourseID=4022,Grade=Grade.C},
                        new Enrollment{StudentID=1,CourseID=4041,Grade=Grade.B},
                        new Enrollment{StudentID=2,CourseID=1045,Grade=Grade.B},
                        new Enrollment{StudentID=2,CourseID=3141,Grade=Grade.F},
                        new Enrollment{StudentID=2,CourseID=2021,Grade=Grade.F},
                        new Enrollment{StudentID=3,CourseID=1050},
                        new Enrollment{StudentID=4,CourseID=1050},
                        new Enrollment{StudentID=4,CourseID=4022,Grade=Grade.F},
                        new Enrollment{StudentID=5,CourseID=4041,Grade=Grade.C},
                        new Enrollment{StudentID=6,CourseID=1045},
                        new Enrollment{StudentID=7,CourseID=3141,Grade=Grade.A},
                        };
                        foreach (Enrollment e in enrollments)
                        {
                            context.Enrollment.Add(e);
                        }
                        context.SaveChanges();
                        */
        }
#endif
    }

}
