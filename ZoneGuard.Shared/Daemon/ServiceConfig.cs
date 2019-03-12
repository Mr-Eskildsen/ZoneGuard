using System;
using System.Collections.Generic;
using System.Text;

namespace ZoneGuard.Shared.Daemon
{
    public class ServiceConfig
    {
        public string Name  { get; set; }



        protected const string PARAMETER_HOST = "host";
        protected const string PARAMETER_USER = "user";
        protected const string PARAMETER_PASSWORD = "password";
        protected const string PARAMETER_VIRTUALHOST = "vhost";



    }
}
