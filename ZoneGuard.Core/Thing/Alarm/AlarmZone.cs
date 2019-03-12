using ZoneGuard.Shared.Config;
using ZoneGuard.Shared.Interface;
using ZoneGuard.Shared.Thing;
using System;
using System.Collections.Generic;
using ZoneGuard.Core.Thing.Link;

namespace ZoneGuard.Core.Thing.Alarm
{
    public class AlarmZone : ThingCore
    {
        Dictionary<string, SensorLink> dictSensors = new Dictionary<string, SensorLink>();

        public bool Armed { get; set; }

        public AlarmZone(ConfigAlarmZone config, IManager manager) : base(config, manager)
        {

        }


        protected override void onDestroy()
        {
        }

        protected override void onInitialize()
        {
            /*
            string baseTopic = "test/alarm/in/";
            string topic;

            //TODO:: Hardcoded
            ServiceMQTT serviceMQTT = (ServiceMQTT)getManager().getServiceByName("MQTT");

            topic = baseTopic + "active";
            serviceMQTT.Subscribe(topic, this.onChangeAlarmActivation);

            topic = baseTopic + "sensitivity";
            serviceMQTT.Subscribe(topic, this.onChangeAlarmSensitivity);
            */

        }
        
        
        public void addSensorLink(SensorLink sensorLink)
        {

            if (dictSensors.ContainsKey(sensorLink.SensorId))
                dictSensors.Remove(sensorLink.SensorId);

            dictSensors.Add(sensorLink.SensorId, sensorLink);
        }

        public bool CanDisarm()
        {
            
            return true;
        }

        public bool CanArm()
        {
            foreach (SensorLink sensor in dictSensors.Values)
            {
                if (sensor.Triggered)
                {
                    //TODO:: Disabled sensor
                }
            }
            return true;
        }


        public bool CheckStatus(ref List<string> messages)
        {
            foreach (SensorLink sensor in dictSensors.Values)
            {
                if (sensor.Triggered)
                {

                }
            }
            return true;
        }
        

        public void onSensorTriggeredEvent (string SensorName, bool state)
        {

        }

        protected void onMqttMessage(string topic, string message, DateTime timestamp)
        {
            //setTriggeredFromString(message);
            Console.WriteLine("Hello From MQTT Callback (AlarmZone) topic='{0}', message='{1}', timestamp='{2}'", topic, message, timestamp.ToString());
            //SensorStateMessage msg = new SensorStateMessage(Id, Triggered, timestamp);
            //string json = msg.toJSON();
            //SensorStateMessage msg2 = MessageCore.fromJSON<SensorStateMessage>(json);
            //getManager().PublishSensorState(msg);
        }
    }
}
