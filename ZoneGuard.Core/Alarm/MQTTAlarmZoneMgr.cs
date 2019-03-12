using System;
using ZoneGuard.Shared.Thing.Service;

namespace ZoneGuard.Core.Alarm
{
    public class MQTTAlarmZoneMgr  : AlarmZoneManager
    {
        //private HomeMgrLifetimeHostedService serviceManager;
        private ServiceMQTT ServiceMQTT { get; set; }
        private String BaseTopic;


        public MQTTAlarmZoneMgr(ServiceMQTT serviceMQTT)
        {
            //serviceManager = mgr;
            ServiceMQTT = serviceMQTT;

            //TODO:: Add to configurtaion
            BaseTopic = "test/";


            Initialize();

        }

        protected void Initialize()
        {
            string topic;
            
            topic = BaseTopic + MQTT_TOPIC_CONTROL_COMMAND;
            ServiceMQTT.Subscribe(topic, onRequestChangeAlarmActivation);

            topic = BaseTopic + MQTT_TOPIC_CONTROL_SENSITIVITY;
            ServiceMQTT.Subscribe(topic, onChangeAlarmSensitivity);


        }

        /*
        protected bool CheckAlarmStatus1()
        {
            List<string> messages = new List<string>();
            foreach (AlarmZone alarmZone in dictAlarmZones.Values)
            {
                alarmZone.CheckStatus(ref messages);
            }

            return true;
        }
        */

        /*
         Subscribe
         ============
         <BASE>/alarm/control/command      -> "arm" / "arm:<Sensitivity>" / "disarm"
         <BASE>/alarm/control/sensitivity -> 1,2,3,...,9

        
        Send
        ===========
        <BASE>/alarm/status/state             -> "arming", "armed", "disarming", "disarmed"
        <BASE>/alarm/status/message           -> Error Message
        <BASE>/alarm/status/sensitivity       -> 1,2,3,...,9
        
        <BASE>/alarm/status/alarmed           -> ON / OFF
        
        <BASE>/alarm/status/<ZoneId>/state    -> "arming", "armed", "disarming", "disarmed"
        <BASE>/alarm/status/<ZoneId>/alarmed  -> ON / OFF
         */

        protected static String MQTT_TOPIC_CONTROL_COMMAND = "alarm/control/command";
        protected static String MQTT_TOPIC_CONTROL_SENSITIVITY = "alarm/control/sensitivity";

        protected static String MQTT_TOPIC_STATUS_STATE = "alarm/status/state";
        protected static String MQTT_TOPIC_STATUS_SENSITIVITY = "alarm/status/sensitivity";
        protected static String MQTT_TOPIC_STATUS_MESSAGE = "alarm/status/message";

        protected static String MQTT_CONTROL_ACTION_ARM = "ARM";
        protected static String MQTT_CONTROL_ACTION_DISARM = "DISARM";

        protected static String MQTT_STATUS_STATE_ARMING = "ARMING";
        protected static String MQTT_STATUS_STATE_ARMED = "ARMED";
        protected static String MQTT_STATUS_STATE_DISARMING = "DISARMING";
        protected static String MQTT_STATUS_STATE_DISARMED = "DISARMED";



        

        public void onRequestChangeAlarmActivation(string topic, string message, DateTime timestamp)
        {

            if (message.ToUpper().StartsWith(MQTT_CONTROL_ACTION_ARM))
            {
                Armed = true;
            }
            else if (message.ToUpper().Equals(MQTT_CONTROL_ACTION_DISARM))
            {
                Armed = false;
            }
            else {
                //TODO:: RAISE Error
            }

        
        }


        public override void BeforeChangeArmedState(bool currentValue, bool newValue)
        { 
            String replyTopic = replyTopic = BaseTopic + MQTT_TOPIC_STATUS_STATE;

            if (newValue == true && currentValue == false)
            {
                ServiceMQTT.Publish(replyTopic, MQTT_STATUS_STATE_ARMING);
            }
            else if (newValue == false && currentValue == true)
            {
                ServiceMQTT.Publish(replyTopic, MQTT_STATUS_STATE_DISARMING);
            }

        }

        public override void AfterChangeArmedState(bool value)
        {
            String replyTopic = replyTopic = BaseTopic + MQTT_TOPIC_STATUS_STATE;
            String replyPayload = "";


            if (value)
            {
                ServiceMQTT.Publish(replyTopic, MQTT_STATUS_STATE_ARMED);
            }
            else if (!value)
            {
                ServiceMQTT.Publish(replyTopic, MQTT_STATUS_STATE_DISARMED);
            }
            
        }


        


        public void onChangeAlarmSensitivity(string topic, string message, DateTime timestamp)
        {

        }
    }
}
