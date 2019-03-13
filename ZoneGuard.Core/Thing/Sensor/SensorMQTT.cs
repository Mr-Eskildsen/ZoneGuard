
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using ZoneGuard.Shared.Config;
using ZoneGuard.Shared.Daemon;
using ZoneGuard.Shared.Interface;
using ZoneGuard.Shared.Message;
using ZoneGuard.Shared.Thing.Sensor;
using ZoneGuard.Shared.Thing.Service;

namespace ZoneGuard.Core.Thing.Sensor
{
    public class SensorMQTT : SensorCore
    {
        //private ServiceMQTT serviceMQTT;
        private string topic;
        

        public SensorMQTT(ConfigSensor config, IManager manager) : base(config, manager)
        {
            Triggered = false;
        }

        public bool Triggered { get; private set; }

        protected override void onDestroy()
        {
            ServiceMQTT serviceMQTT = (ServiceMQTT)getManager().getServiceByName(CoreDaemonService.SERVICE_ID_MQTT);
            serviceMQTT.Unsubscribe(topic);
        }

        protected override void onInitialize()
        {
            ConfigSensor cfg = getConfig<ConfigSensor>();

            topic = cfg.getParameterValue(ConfigCore.PARAMETER_TOPIC_STATE);

            ServiceMQTT serviceMQTT = (ServiceMQTT)getManager().getServiceByName(CoreDaemonService.SERVICE_ID_MQTT);

            serviceMQTT.Subscribe(topic, this.onMqttMessage);
            

        }

        private void updateTriggeredFromString(string message)
        {
            switch(message.ToUpper())
            {
                case "OPEN":
                case "ON":
                    Triggered = true;
                    break;
                case "CLOSED":
                case "OFF":
                    Triggered = false;
                    break;
            }
        }
        
        public void onMqttMessage(string topic, string message, DateTime timestamp)
        {
            getLogger().LogDebug("Sensor '{0}' received MQTT message. topic='{1}', message='{2}', timestamp='{3}'", Id, topic, message, timestamp.ToString());
            updateTriggeredFromString(message);

            SensorStateMessage msg = new SensorStateMessage(Id, Triggered, timestamp);
            getManager().PublishSensorState(msg);
        }
    }
}
