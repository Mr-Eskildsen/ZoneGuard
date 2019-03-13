
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZoneGuard.Shared.Config
{
    public class ConfigSensor : ConfigCore
    {
        public ConfigSensor(Dictionary<string, string> parameters) : base(parameters)
        {
        }

        protected override bool ValidateParameters(Dictionary<string, string> parameters)
        {
            //TODO:: Validate Parameter list
            return true;
        }

        [JsonProperty(PropertyName = PARAMETER_NODE_ID)]
        public string NodeId { get { return getParameterValue(ConfigCore.PARAMETER_NODE_ID); } }

        [JsonProperty(PropertyName = ConfigCore.PARAMETER_TOPIC_STATE)]
        public string TopicState { get { return getParameterValue(ConfigCore.PARAMETER_TOPIC_STATE); } }

        //[JsonProperty(PropertyName = ConfigCore.PARAMETER_TOPIC_STATE)]
        //public string TopicCommand { get { return getParameterValue(ConfigCore.PARAMETER_TOPIC_STATE); } }

    }
}
