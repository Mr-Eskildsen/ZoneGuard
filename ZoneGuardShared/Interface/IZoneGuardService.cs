using System;
using System.Collections.Generic;
using System.Text;
using ZoneGuard.Shared.Service;

namespace ZoneGuard.Shared.Interface
{
    public interface IZoneGuardService
    {
        void OnInitialized(ZoneGuardServiceConfig config);
    }
}
