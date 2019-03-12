using System;
using System.Collections.Generic;
using System.Text;
using ZoneGuard.Shared.Config;
using ZoneGuard.Shared.Interface;

namespace ZoneGuard.Shared.Thing.Service
{
    public abstract class ServiceCore : ThingCore
    {
        public ServiceCore(ConfigService config, IManager manager) : base(config, manager)
        {

            //            manager.getLoggerFactory().CreateLogger(GetType().FullName).LogInformation("Jepperdi");
        }
        /*        
                public static ServiceCore CreateServiceFromJSON(Type myType, String clsName)
                {
                    String name = myType.FullName;
                    ServiceCore serviceObj = null; // new ServiceMQ();
                                                   //myObject = (MyAbstractClass)Activator.CreateInstance("AssemblyName", "TypeName");
                    Type cls = Assembly.GetExecutingAssembly().GetType("HomeManager2.Shared.Thing.Service." + clsName);
                   // serviceObj = Activator.CreateInstance(cls);
                    return serviceObj;
                }
                */


    }
}
