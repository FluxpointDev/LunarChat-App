using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.ViewModels;
using LunarChatSharp.Core.Channels;
using LunarChatSharp.Core.Servers;
using System.Collections.ObjectModel;
using System.Linq;

namespace LunarChatApp.Views.Dialogs.Apps;

public partial class InviteAppPopupModel : ViewModelBase
{
    private readonly ServiceManager services;

    public InviteAppPopupModel(ServiceManager sv, string appId)
    {
        services = sv;
        _serverItems = new ObservableCollection<AppListItem>(services.State.Socket.Servers.Values.Where(x => !x.Apps.ContainsKey(appId) && x.HasPermission(x.Member, ServerPermission.ManageApps)).Select(x => new AppListItem
        {
            id = x.Server.Id,
            Name = x.Server.Name
        }));

        _groupItems = new ObservableCollection<AppListItem>(services.State.Socket.PrivateChannels.Values.Where(x => x.Type == ChannelType.Group && x.GroupSettings?.OwnerId == services.Client.CurrentId).Select(x => new AppListItem
        {
            id = x.Id,
            Name = x.Name,
        }));
    }

    [ObservableProperty]
    private ObservableCollection<AppListItem> _serverItems;

    [ObservableProperty]
    private ObservableCollection<AppListItem> _groupItems;

    [ObservableProperty]
    private AppListItem? _selectedServer;

    [ObservableProperty]
    private AppListItem? _selectedGroup;

    [RelayCommand]
    public void Close()
    {
        MainView? view = services.GetMainView();

        if (view == null)
            return;

        (view.DataContext as MainModel)!.CurrentImage = null;
    }
}

public partial class AppListItem : ObservableObject
{
    [ObservableProperty]
    private string _name;

    public string id;
}