using LunarChatApp.Services;

namespace LunarChatApp.Views.Servers.Settings;

public partial class ServerSettingsAuditLogsModel : ViewModelBase
{
    private ServiceManager services;

    public ServerSettingsAuditLogsModel(ServiceManager sv)
    {
        services = sv;
    }
}
