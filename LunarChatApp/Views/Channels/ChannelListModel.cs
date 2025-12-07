using CommunityToolkit.Mvvm.ComponentModel;
using LunarChatApp.Services;
using LunarChatApp.ViewModels.Servers;
using LunarChatApp.Views;
using LunarChatSharp.Core.Servers;
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
        bool CanManage = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ChannelPermission.ManageChannel);
        if (ChannelsList == null)
            ChannelsList = new ObservableCollection<ChannelItem>(state.Socket.CurrentServer.Channels.Values.Select(x => new ChannelItem() { ChannelName = x.Name, ChannelType = x.Type, DataContext = new ChannelItemModel(services, state, x, CanManage) }));
        state.Socket.CurrentServer.OnChannelUpdate += ChannelUpdate;
        state.Socket.CurrentServer.OnChannelDelete += ChannelDelete;
        state.Socket.CurrentServer.OnChannelCreate += Server_OnChannelCreate;
        services.State.Socket.CurrentServer.OnPermissionUpdate += PermissionUpdate;
    }

    private async Task PermissionUpdate()
    {
        bool CanManage = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ChannelPermission.ManageChannel);
        foreach (var i in ChannelsList)
        {
            (i.DataContext as ChannelItemModel).CanManage = CanManage;
        }
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
        bool CanManage = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ChannelPermission.ManageChannel);
        ChannelsList.Add(new ChannelItem { ChannelName = channel.Name, ChannelType = channel.Type, DataContext = new ChannelItemModel(services, state, channel, CanManage) });
    }

    [ObservableProperty]
    private ObservableCollection<ChannelItem> _channelsList;
}
