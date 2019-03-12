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


        public SensorStateMessage(string sensorId, bool state, DateTime timestamp) : base(sensorId, timestamp)
        {
            State = state;
        }

        [JsonProperty]
        public bool State { get; private set; }
        
    }
}
