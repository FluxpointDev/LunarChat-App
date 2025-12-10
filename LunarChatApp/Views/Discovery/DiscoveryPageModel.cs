using LunarChatApp.Services;

namespace LunarChatApp.Views.Discovery;

public partial class DiscoveryPageModel : ViewModelBase
{
    private ServiceManager services;
    public DiscoveryPageModel(ServiceManager sv)
    {
        services = sv;
    }
}
