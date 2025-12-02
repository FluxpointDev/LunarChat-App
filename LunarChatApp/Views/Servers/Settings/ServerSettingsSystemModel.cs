using CommunityToolkit.Mvvm.ComponentModel;
using LunarChatApp.Services;
using System.Collections.ObjectModel;
using System.Linq;

namespace LunarChatApp.Views.Servers.Settings;

public partial class ServerSettingsSystemModel : ViewModelBase
{
    public ServerSettingsSystemModel(ServiceManager services)
    {
        Items = new ObservableCollection<ChannelListItem>(services.State.Socket.CurrentServer.Channels.Values.Select(x => new ChannelListItem
        {
            id = x.Id,
            Name = x.Name
        }));
    }
    [ObservableProperty]
    private ObservableCollection<ChannelListItem> _items;

    [ObservableProperty]
    private ChannelListItem? _selectedJoinMessage;
    [ObservableProperty]
    private ChannelListItem? _selectedLeftMessage;
    [ObservableProperty]
    private ChannelListItem? _selectedBanMessage;
    [ObservableProperty]
    private ChannelListItem? _selectedKickMessage;
    [ObservableProperty]
    private ChannelListItem? _selectedTimeoutMessage;
}
public partial class ChannelListItem : ObservableObject
{
    [ObservableProperty]
    private string _name;

    public string id;
}