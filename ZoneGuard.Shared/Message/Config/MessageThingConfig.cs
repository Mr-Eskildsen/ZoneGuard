using ZoneGuard.Shared.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZoneGuard.Shared.Message
{
    public class MessageThingDAL : MessageConfig
    {
//        public MessageThingDAL(ConfigSensor config ) : base(config.Id, config.T)

        public MessageThingDAL(string thingId, Guid NodeId, DateTime timestamp) : base(thingId, timestamp)
        {
            this.NodeId = NodeId;
        }

        [JsonProperty]
        public Guid NodeId;
    }
}
