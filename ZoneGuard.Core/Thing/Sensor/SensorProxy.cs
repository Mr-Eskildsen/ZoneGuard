using System;
using ZoneGuard.DAL.Data;
using ZoneGuard.DAL.Models.Log;
using ZoneGuard.Shared.Config;
using ZoneGuard.Shared.Interface;
using ZoneGuard.Shared.Thing.Sensor;

namespace ZoneGuard.Core.Thing.Sensor
{
    public class SensorProxy : SensorCore
    {
        public SensorProxy(ConfigSensor config, IManager manager) : base(config, manager)
        {

        }


        public void setState(bool triggeredState)
        {
            if (Activated)
            {
                //Log TO DATABASE
                ZoneGuardConfigContextFactory factory = new ZoneGuardConfigContextFactory();

                using (ZoneGuardConfigContext context = factory.CreateDbContext())
                {
                    SensorStateLogDAL ssl = new SensorStateLogDAL();
                    ssl.SensorName = Id;
                    ssl.State = triggeredState.ToString();
                    ssl.CreatedTimestamp = DateTime.UtcNow;

                    context.SensorStateLog.Add(ssl);
                    context.SaveChanges();
                }
                
                Triggered = triggeredState;
                //OnTriggeredChanged(new SensorTriggeredEventArgs(Id, Triggered));
                RaiseTriggeredChanged(new SensorTriggeredEventArgs(Id, Triggered));
            }        
        }

        protected override void onDestroy()
        {
            //TODO Implement!
            //throw new NotImplementedException();
        }

        protected override void onInitialize()
        {
            //TODO Implement!
            //throw new NotImplementedException();
        }
    }
}
