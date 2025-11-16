using CommunityToolkit.Mvvm.ComponentModel;
using LunarChatApp.ViewModels.Servers;
using System.Collections.ObjectModel;
using System.Linq;

namespace LunarChatApp.ViewModels;

public partial class ChannelListViewModel : ViewModelBase
{
    public ChannelListViewModel(TestState state, ServerState server)
    {
        if (ChannelsList == null)
            ChannelsList = new ObservableCollection<ChannelItem>(server.Channels.Values.Select(x => new ChannelItem() { ChannelName = x.Name, ChannelType = x.Type, DataContext = new ChannelItemViewModel(state, x) }));

    }

    [ObservableProperty]
    private ObservableCollection<ChannelItem> _channelsList;
}
