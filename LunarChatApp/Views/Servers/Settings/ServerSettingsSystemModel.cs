using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace LunarChatApp.Views.Servers.Settings;

public partial class ServerSettingsSystemModel : ViewModelBase
{
    public ServerSettingsSystemModel()
    {
        Items = new List<ChannelListItem>();
        Items.Add(new ChannelListItem
        {
            Name = "Test",
            id = "1"
        });
    }
    [ObservableProperty]
    private List<ChannelListItem> _items;

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