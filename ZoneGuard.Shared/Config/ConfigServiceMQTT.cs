using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;


namespace ZoneGuard.Shared.Config //.Service
{
    public class ConfigServiceMQTT : ConfigService
    {

        public ConfigServiceMQTT(Dictionary<string, string> parameters) : base(parameters)
        {
        }


        [JsonProperty(PropertyName = PARAMETER_HOST)]
        public string Host { get { return getParameterValue(PARAMETER_HOST); } }

        [JsonProperty(PropertyName = PARAMETER_USER)]
        public string UserName { get { return getParameterValue(PARAMETER_USER); } }

        [JsonProperty(PropertyName = PARAMETER_PASSWORD)]
        public string Password { get { return getParameterValue(PARAMETER_PASSWORD); } }

        
        public bool UseCrentials()
        {
            return (HasParameter(PARAMETER_USER) && HasParameter(PARAMETER_PASSWORD));
        }
            
    }
}
