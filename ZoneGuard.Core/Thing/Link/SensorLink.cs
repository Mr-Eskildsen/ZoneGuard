using System;
using ZoneGuard.Core.Thing.Alarm;
using ZoneGuard.Shared.Thing.Link;
using ZoneGuard.Shared.Interface;
using ZoneGuard.Shared.Thing.Sensor;
using Microsoft.Extensions.Logging;

namespace ZoneGuard.Core.Thing.Link
{
    public class SensorLink : LinkCore
    {
        protected SensorCore sensor;
        protected AlarmZone alarmZone;



        public SensorLink(SensorCore source, AlarmZone target, IManager manager) : base(manager)
        {
            source.addLink(OnSensorTriggeredEvent);
            this.sensor = source;
            this.alarmZone = target;
        }

        protected override void onDestroy()
        {
            //throw new NotImplementedException();
        }

        protected override void onInitialize()
        {
            //throw new NotImplementedException();
        }

        private void OnSensorTriggeredEvent(object sender, SensorTriggeredEventArgs e)
        {

            if (alarmZone.Activated)
            {
                
                if (!Activated && !e.Triggered)
                {
                    Activated = true;
                    getLogger().LogInformation("Activated alarm sensor");
                }

                if (Activated)
                {
                    Activated = true;
                    getLogger().LogInformation("Alarm Sensor '{0}' received state '{1}'", e.Name, e.Triggered);
                    alarmZone.onSensorTriggeredEvent(e.Name, e.Triggered);
                }
            }
            //Console.WriteLine("OnSourceTrigegred Target.Name='{0}' Source.Name='{1}', State='{2}'", target.Id, e.Name, e.Triggered);
        }


        //TODO:: Check  if link is disabeld
        public bool Triggered { get { return sensor.Triggered; } }

        public String SensorId { get { return sensor.Id;  } }

    }
}
