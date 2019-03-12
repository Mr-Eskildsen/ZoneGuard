using System;

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
