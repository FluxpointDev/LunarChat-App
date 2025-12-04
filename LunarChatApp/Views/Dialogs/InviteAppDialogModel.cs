using CommunityToolkit.Mvvm.ComponentModel;
using LunarChatApp.Services;
using LunarChatSharp.Core.Servers;
using System.Collections.ObjectModel;
using System.Linq;

namespace LunarChatApp.Views.Dialogs;

public partial class InviteAppDialogModel : ViewModelBase
{
    public InviteAppDialogModel(ServiceManager services)
    {
        Items = new ObservableCollection<AppListItem>(services.State.Socket.Servers.Values.Where(x => x.HasPermission(x.Member, ServerPermission.ManageApps)).Select(x => new AppListItem
        {
            id = x.Server.Id,
            Name = x.Server.Name
        }));
    }

    [ObservableProperty]
    private ObservableCollection<AppListItem> _items;

    [ObservableProperty]
    private AppListItem? _selectedServer;
}
public partial class AppListItem : ObservableObject
{
    [ObservableProperty]
    private string _name;

    public string id;
}
