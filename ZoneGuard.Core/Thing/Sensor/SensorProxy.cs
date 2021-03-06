﻿using System;
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


        public void setTriggeredState(bool newTriggeredState)
        {
            if (Activated)
            {
                if (Triggered != newTriggeredState)
                {
                    //Log Raw data to database
                    ZoneGuardConfigContextFactory factory = new ZoneGuardConfigContextFactory();
                    using (ZoneGuardConfigContext context = factory.CreateDbContext())
                    {
                        SensorStateLogDAL stateLog = new SensorStateLogDAL();
                        stateLog.SensorName = Id;
                        stateLog.Triggered = (newTriggeredState ? 1 : 0);
                        stateLog.Timestamp = DateTime.UtcNow;
                        
                        context.SensorStateLog.Add(stateLog);
                        context.SaveChanges();
                    }
                }

                Triggered = newTriggeredState;


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
