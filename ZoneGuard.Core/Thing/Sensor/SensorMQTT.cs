
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using ZoneGuard.DAL.Data;
using ZoneGuard.DAL.Models.Log;
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
        private string topicState;
        private string sensorType;



        public SensorMQTT(ConfigSensor config, IManager manager) : base(config, manager)
        {
            Triggered = false;
        }

        public bool Triggered { get; private set; }

        protected override void onDestroy()
        {
            ServiceMQTT serviceMQTT = (ServiceMQTT)getManager().getServiceByName(CoreDaemonService.SERVICE_ID_MQTT);
            serviceMQTT.Unsubscribe(topicState);
        }

        protected override void onInitialize()
        {
            ConfigSensor cfg = getConfig<ConfigSensor>();

            topicState = cfg.getParameterValue(ConfigCore.PARAMETER_TOPIC_STATE);
            sensorType = cfg.getParameterValue(ConfigCore.PARAMETER_TYPE);
            ServiceMQTT serviceMQTT = (ServiceMQTT)getManager().getServiceByName(CoreDaemonService.SERVICE_ID_MQTT);
            if (serviceMQTT!=null)
            {
                serviceMQTT.Subscribe(topicState, this.onMqttMessage);
            }
            

        }

        private bool getTriggeredFromString(string message)
        {
            bool triggered = false;
            switch (message.ToUpper())
            {
                case "OPEN":
                case "ON":
                    triggered = true;
                    break;
                case "CLOSED":
                case "OFF":
                    triggered = false;
                    break;
            }
            return triggered;
        }

        
        public void onMqttMessage(string topic, string message, DateTime timestamp)
        {
            getLogger().LogDebug("Sensor '{0}' received MQTT message. topic='{1}', message='{2}', timestamp='{3}'", Id, topic, message, timestamp.ToString());
            getTriggeredFromString(message);


            //Log Raw data to database
            ZoneGuardConfigContextFactory factory = new ZoneGuardConfigContextFactory();
            using (ZoneGuardConfigContext context = factory.CreateDbContext())
            {
                SensorStateRawLogDAL rawLog = new SensorStateRawLogDAL();
                rawLog.SensorName = Id;
                rawLog.State = message;
                rawLog.SensorType = sensorType;
                rawLog.AdditionalInfo = topicState;

                rawLog.Timestamp = DateTime.UtcNow;

                context.RawSensorStateLog.Add(rawLog);
                context.SaveChanges();
            }


            SensorStateMessage msg = new SensorStateMessage(Id, getTriggeredFromString(message), timestamp);
            getManager().PublishSensorState(msg);
        }
    }
}
