using Newtonsoft.Json;
using System.Collections.Generic;

namespace ZoneGuard.Shared.Config
{
    public class ConfigServiceMQ : ConfigService
    {
        public ConfigServiceMQ(Dictionary<string, string> parameters) : base(parameters)
        {
        }

        protected override bool ValidateParameters(Dictionary<string, string> parameters)
        {
            //TODO:: Validate Paarmeter list
            return true;
        }


        [JsonProperty(PropertyName = PARAMETER_HOST)]
        public string Host { get { return getParameterValue(PARAMETER_HOST); } }

        [JsonProperty(PropertyName = PARAMETER_USER)]
        public string User { get { return getParameterValue(PARAMETER_USER); } }

        [JsonProperty(PropertyName = PARAMETER_PASSWORD)]
        public string Password { get { return getParameterValue(PARAMETER_PASSWORD); } }

        [JsonProperty(PropertyName = PARAMETER_VIRTUALHOST)]
        public string VirtualHost { get { return getParameterValue(PARAMETER_VIRTUALHOST); } }
    }

}
