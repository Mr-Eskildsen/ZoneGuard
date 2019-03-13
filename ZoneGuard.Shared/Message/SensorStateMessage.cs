using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZoneGuard.Shared.Message
{
    public class SensorStateMessage : MessageCore
    {
        public SensorStateMessage() : base()
        {

        }


        public SensorStateMessage(string sensorId, bool triggered, DateTime timestamp) : base(sensorId, timestamp)
        {
            Triggered = triggered;
        }

        [JsonProperty]
        public bool Triggered { get; private set; }
        
    }
}
