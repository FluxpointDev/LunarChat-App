using CommunityToolkit.Mvvm.ComponentModel;
using LunarChatApp.Services;
using LunarChatApp.Shared.Core.Channels;
using LunarChatApp.ViewModels.Servers;
using LunarChatApp.Views;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LunarChatApp.ViewModels;

public partial class ChannelListViewModel : ViewModelBase
{
    private TestState state;
    public ServiceManager services;
    public ChannelListViewModel(ServiceManager sv, TestState st)
    {
        state = st;
        services = sv;
        if (ChannelsList == null)
            ChannelsList = new ObservableCollection<ChannelItem>(state.Socket.CurrentServer.Channels.Values.Select(x => new ChannelItem() { ChannelName = x.Name, ChannelType = x.Type, DataContext = new ChannelItemViewModel(services, state, x) }));
        state.Socket.CurrentServer.OnChannelUpdate += ChannelUpdate;
        state.Socket.CurrentServer.OnChannelDelete += ChannelDelete;
        state.Socket.CurrentServer.OnChannelCreate += Server_OnChannelCreate;
    }

    private async Task ChannelDelete(Channel channel)
    {
        ChannelItem? item = ChannelsList.FirstOrDefault(x => (x.DataContext as ChannelItemViewModel).id == channel.Id);
        if (item != null)
            ChannelsList.Remove(item);
    }

    private async Task ChannelUpdate(Channel channel)
    {
        ChannelItem? item = ChannelsList.FirstOrDefault(x => (x.DataContext as ChannelItemViewModel).id == channel.Id);
        if (item != null)
        {
            (item.DataContext as ChannelItemViewModel).Name = channel.Name;
        }
    }

    private async Task Server_OnChannelCreate(Shared.Core.Channels.Channel channel)
    {
        ChannelsList.Add(new ChannelItem { ChannelName = channel.Name, ChannelType = channel.Type, DataContext = new ChannelItemViewModel(services, state, channel) });
    }

    [ObservableProperty]
    private ObservableCollection<ChannelItem> _channelsList;
}
