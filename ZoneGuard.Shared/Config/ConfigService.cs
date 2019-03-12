using System;
using System.Collections.Generic;
using System.Text;

namespace ZoneGuard.Shared.Config //.Service
{
    public abstract class ConfigService : ConfigCore
    {
        protected const string PARAMETER_HOST = "host";
        protected const string PARAMETER_USER = "user";
        protected const string PARAMETER_PASSWORD = "password";
        protected const string PARAMETER_VIRTUALHOST = "vhost";



        public ConfigService(Dictionary<string, string> parameters) : base(parameters)
        {
        }



        //[JsonProperty(PropertyName = PARAMETER_CONFIG_CLASS)]
        /*[JsonConverter(typeof(ServiceTypeConverter))]
        public ConfigServiceType ServiceType { get {
                Enum.TryParse(getParameterValue(PARAMETER_CONFIG_CLASS), out ConfigServiceType result);
                //ConfigServiceType result = ConfigServiceType.Unknown;
                return result; } }
                */
        //public string ConfigClass { get { return getParameterValue(PARAMETER_CONFIG_CLASS); } }


        /*
    public static ConfigService CreateServiceConfigFromJSON(String json)
    {
        //Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        return CreateFromJSON<ConfigService>(json);
    }
    */

    }
}
