using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.ViewModels;

namespace LunarChatApp.Views.Discovery;

public partial class DiscoverySidebarModel(ServiceManager services) : ViewModelBase
{
    [RelayCommand]
    public void ShowServers()
    {
        MainModel model = (services.MainControl.DataContext as MainModel);
        if (model == null)
            return;

        if (model.SelectedPage is ServersPage sp && sp.DataContext is ServersModel sm && sm.SelectedPage is DiscoveryPage page && page.DataContext is DiscoveryPageModel pageModel)
        {
            if (!pageModel.ShowApps)
                return;

            pageModel.ShowApps = false;
        }
    }

    [RelayCommand]
    public void ShowApps()
    {
        MainModel model = (services.MainControl.DataContext as MainModel);
        if (model == null)
            return;

        if (model.SelectedPage is ServersPage sp && sp.DataContext is ServersModel sm && sm.SelectedPage is DiscoveryPage page && page.DataContext is DiscoveryPageModel pageModel)
        {
            if (pageModel.ShowApps)
                return;

            pageModel.ShowApps = true;
        }
    }

}
