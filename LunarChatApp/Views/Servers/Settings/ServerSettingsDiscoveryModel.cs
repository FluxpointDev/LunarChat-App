using LunarChatApp.Services;

namespace LunarChatApp.Views.Servers.Settings;

public partial class ServerSettingsDiscoveryModel : ViewModelBase
{
    public ServerSettingsDiscoveryModel(ServiceManager sv)
    {
        services = sv;
    }

    private ServiceManager services;
}
