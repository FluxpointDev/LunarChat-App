using CommunityToolkit.Mvvm.ComponentModel;
using LunarChatApp.Services;
using LunarChatApp.ViewModels.Servers;
using LunarChatApp.Views;
using LunarChatSharp.Rest.Channels;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LunarChatApp.ViewModels;

public partial class ChannelListModel : ViewModelBase
{
    private TestState state;
    public ServiceManager services;
    public ChannelListModel(ServiceManager sv, TestState st)
    {
        state = st;
        services = sv;
        if (ChannelsList == null)
            ChannelsList = new ObservableCollection<ChannelItem>(state.Socket.CurrentServer.Channels.Values.Select(x => new ChannelItem() { ChannelName = x.Name, ChannelType = x.Type, DataContext = new ChannelItemModel(services, state, x) }));
        state.Socket.CurrentServer.OnChannelUpdate += ChannelUpdate;
        state.Socket.CurrentServer.OnChannelDelete += ChannelDelete;
        state.Socket.CurrentServer.OnChannelCreate += Server_OnChannelCreate;
    }

    private async Task ChannelDelete(RestChannel channel)
    {
        ChannelItem? item = ChannelsList.FirstOrDefault(x => (x.DataContext as ChannelItemModel).id == channel.Id);
        if (item != null)
            ChannelsList.Remove(item);
    }

    private async Task ChannelUpdate(RestChannel channel)
    {
        ChannelItem? item = ChannelsList.FirstOrDefault(x => (x.DataContext as ChannelItemModel).id == channel.Id);
        if (item != null)
        {
            (item.DataContext as ChannelItemModel).Name = channel.Name;
        }
    }

    private async Task Server_OnChannelCreate(RestChannel channel)
    {
        ChannelsList.Add(new ChannelItem { ChannelName = channel.Name, ChannelType = channel.Type, DataContext = new ChannelItemModel(services, state, channel) });
    }

    [ObservableProperty]
    private ObservableCollection<ChannelItem> _channelsList;
}
