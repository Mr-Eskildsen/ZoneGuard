using ZoneGuard.DAL.Models.Config;
//using ZoneGuard.Shared.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using ZoneGuard.Shared.Config;

namespace ZoneGuard.DAL.Data
{
    public class ZoneGuardConfigContextFactory : IDesignTimeDbContextFactory<ZoneGuardConfigContext>
    {
        private static string _connectionString;

        public ZoneGuardConfigContext CreateDbContext()
        {
            return CreateDbContext(null);
        }

        public ZoneGuardConfigContext CreateDbContext(string[] args)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                LoadConnectionString();
            }

            var builder = new DbContextOptionsBuilder<ZoneGuardConfigContext>();
            //TODO:: FIX MSSQL builder.UseSqlServer(_connectionString);
            builder.UseSqlite("Data Source=zoneguard.db");

            return new ZoneGuardConfigContext(builder.Options);
        }

        private static void LoadConnectionString()
        {
            /*
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: false);

            var configuration = builder.Build();

            _connectionString = configuration.GetConnectionString("DefaultConnection");
            */
            _connectionString = "Server=(localdb)\\mssqllocaldb;Database=AlarmManager;Trusted_Connection=True;MultipleActiveResultSets=true";
        }

        public static ConfigAlarmZone CreateConfigAlarmZoneFromDAL(AlarmZoneDAL zone)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            ConfigAlarmZone alarmZoneConfig = null;

            //foreach (ThingParameterDAL tpc in thing.Parameters)
            {
                parameters[ConfigCore.PARAMETER_NAME] = zone.Name;
                parameters[ConfigCore.PARAMETER_CONFIG_CLASS] = "ConfigAlarmZone";
                //parameters[PARAMETER_THING_CLASS] = zone.Name;

            }
            //string configClassName = parameters[PARAMETER_CONFIG_CLASS];

            //Type cls = Assembly.GetExecutingAssembly().GetType("HomeManager2.Shared.Config." + configClassName);
            alarmZoneConfig = new ConfigAlarmZone(parameters);


            return alarmZoneConfig;
        }

        public static ConfigSensor CreateConfigAlarmZoneSensorFromDAL(AlarmZoneThingDAL thing)
        {
            return null;
        }

        public static T CreateThingFromDAL<T>(ThingDAL thing)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            T configObj;
            foreach (ThingParameterDAL tpc in thing.Parameters)
            {
                parameters[tpc.Name] = tpc.Value;

            }
            //Set Node Identifier
//TODO:: NODE            parameters[ConfigCore.PARAMETER_NODE_ID] = thing.Node.UniqueIdentifier.ToString();

            string configClassName = parameters[ConfigCore.PARAMETER_CONFIG_CLASS];

            //            String name = myType.FullName;
            //          ServiceCore serviceObj = null; // new ServiceMQ();
            //myObject = (MyAbstractClass)Activator.CreateInstance("AssemblyName", "TypeName");
            //Type cls = Assembly.GetExecutingAssembly().GetType("HomeManager2.Shared.Config." + configClassName);
            Type cls = ConfigCore.GetConfigAssembly().GetType("ZoneGuard.Shared.Config." + configClassName);
            configObj = (T)Activator.CreateInstance(cls, parameters);


            /*if (configObj.GetType() == typeof(ConfigSensor))
            {
                
                    
            }*/
            //configObj = new ConfigSensor(parameters);
            return configObj;
        }

        
    }
}
