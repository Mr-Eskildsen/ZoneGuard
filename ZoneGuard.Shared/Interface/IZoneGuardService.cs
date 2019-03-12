using ZoneGuard.Shared.Config;


namespace ZoneGuard.Shared.Interface
{
    public interface IZoneGuardService
    {
        void OnInitialized(ConfigService config);
    }
}
