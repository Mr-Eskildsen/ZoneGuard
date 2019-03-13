using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using ZoneGuard.Shared.Message;
using ZoneGuard.Shared.Thing.Sensor;
using ZoneGuard.Shared.Thing.Service;

namespace ZoneGuard.Shared.Interface
{
    public interface IManager
    {
        ILoggerFactory getLoggerFactory();

        SensorCore getSensorByName(String name);
        ServiceCore getServiceByName(String name);

        
        void PublishSensorState(SensorStateMessage ssm);
    }

}
