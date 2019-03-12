using System;
using System.Collections.Generic;
using System.Text;

namespace ZoneGuard.Shared.Config
{
    public class ConfigAlarmZone : ConfigCore
    {
        public ConfigAlarmZone(Dictionary<string, string> parameters) : base(parameters)
        {
            
        }


        protected override bool ValidateParameters(Dictionary<string, string> parameters)
        {
            //TODO:: Validate Paarmeter list
            return true;
        }
    }
}
