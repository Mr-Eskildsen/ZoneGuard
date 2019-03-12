using System;
using System.Collections.Generic;
using System.Text;
using ZoneGuard.Shared.Daemon;

namespace ZoneGuard.Core
{
    class ConfigServiceMQTT : ServiceConfig
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public bool UseCredentials()
        {
            return (!String.IsNullOrEmpty(UserName) && !String.IsNullOrEmpty(Password));
        }


        /*
         //[JsonProperty(PropertyName = PARAMETER_HOST)]
         public string Host { get { return getParameterValue(PARAMETER_HOST); } }

         //[JsonProperty(PropertyName = PARAMETER_USER)]
         public string UserName { get { return getParameterValue(PARAMETER_USER); } }

         [JsonProperty(PropertyName = PARAMETER_PASSWORD)]
         public string Password { get { return getParameterValue(PARAMETER_PASSWORD); } }


         public bool UseCrentials()
         {
             return (HasParameter(PARAMETER_USER) && HasParameter(PARAMETER_PASSWORD));
         }
         */

    }
}
