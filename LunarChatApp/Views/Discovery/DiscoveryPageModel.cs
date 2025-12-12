using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using LunarChatApp.Services;
using LunarChatSharp.Rest.Dev;
using LunarChatSharp.Rest.Servers;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LunarChatApp.Views.Discovery;

public partial class DiscoveryPageModel : ViewModelBase
{
    private ServiceManager services;
    public DiscoveryPageModel(ServiceManager sv)
    {
        services = sv;

        _ = Task.Run(async () =>
        {
            RestServer[]? servers = await services.Rest.GetAsync<RestServer[]>("/discovery/servers");
            if (servers == null)
                return;

            Dispatcher.UIThread.Post(() =>
            {
                ServersList = new ObservableCollection<DiscoverCard>(servers.Select(x => new DiscoverCard
                {
                    DataContext = new DiscoverCardModel(services, x)
                }));
            });

            RestApp[]? apps = await services.Rest.GetAsync<RestApp[]>("/discovery/apps");
            if (apps == null)
                return;

            Dispatcher.UIThread.Post(() =>
            {
                AppsList = new ObservableCollection<DiscoverCard>(apps.Select(x => new DiscoverCard
                {
                    DataContext = new DiscoverCardModel(services, x)
                }));
            });
        });
    }

    [ObservableProperty]
    private ObservableCollection<DiscoverCard> _serversList;

    [ObservableProperty]
    private ObservableCollection<DiscoverCard> _appsList;

    [ObservableProperty]
    private bool showApps;
}
