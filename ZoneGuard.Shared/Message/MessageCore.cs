using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZoneGuard.Shared.Message
{
    public abstract class MessageCore
    {
        protected MessageCore()
        {

        }

        protected MessageCore(string NodeId, DateTime timestamp)
        {
            this.Id = NodeId;
            this.Timestamp = timestamp;
        }

        [JsonProperty]
        public String Id { get; private set; }

        [JsonProperty]
        public DateTime Timestamp { get; private set; }


        public string toJSON()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static T fromJSON<T>(string json) where T : MessageCore, new()
        {
            return JsonConvert.DeserializeObject<T>(json);
            //return JsonConvert.SerializeObject(this);
            //return null;
        }
    }
}
