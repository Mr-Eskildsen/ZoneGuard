using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using ZoneGuard.Shared.Thing.Service;

namespace ZoneGuard.Shared.Interface
{
    public interface IManager
    {
        ILoggerFactory getLoggerFactory();

        //TODO:: Migrate
        //SensorCore getSensorByName(String name);
        ServiceCore getServiceByName(String name);

        //TODO:: Migrate
        //void PublishSensorState(SensorStateMessage ssm);
    }

}
