using CommunityToolkit.Mvvm.ComponentModel;
using LunarChatApp.Services;
using LunarChatApp.ViewModels.Servers;
using LunarChatApp.Views;
using LunarChatSharp.Rest.Channels;
using LunarChatSharp.Rest.Servers;
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
        bool CanManage = services.State.CurrentServer.CanManageChannel(services.State.CurrentServer.Member);
        if (ChannelsList == null)
            ChannelsList = new ObservableCollection<ChannelItem>(state.CurrentServer.Channels.Values.Select(x => new ChannelItem() { DataContext = new ChannelItemModel(services, state, x, CanManage) }));
        services.Client.OnChannelUpdate += ChannelUpdate;
        services.Client.OnChannelDelete += ChannelDelete;
        services.Client.OnChannelCreate += Server_OnChannelCreate;
        services.State.CurrentServer.OnPermissionUpdate += PermissionUpdate;
    }

    private async Task PermissionUpdate(RestServer server)
    {
        bool CanManage = services.State.CurrentServer.CanManageChannel(services.State.CurrentServer.Member);
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            foreach (var i in ChannelsList)
            {
                (i.DataContext as ChannelItemModel).CanManage = CanManage;
            }
        });
    }

    private async Task ChannelDelete(RestChannel channel)
    {
        if (state.CurrentServer == null || state.CurrentServer.Server?.Id != channel.ServerId)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            ChannelItem? item = ChannelsList.FirstOrDefault(x => (x.DataContext as ChannelItemModel).id == channel.Id);
            if (item == null)
                return;

            ChannelsList.Remove(item);
        });

    }

    private async Task ChannelUpdate(RestChannel channel, UpdateChannelRequest request)
    {
        if (state.CurrentServer == null || state.CurrentServer.Server?.Id != channel.ServerId)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            ChannelItem? item = ChannelsList.FirstOrDefault(x => (x.DataContext as ChannelItemModel).id == channel.Id);
            if (item == null)
                return;

            (item.DataContext as ChannelItemModel).Name = channel.Name;
        });
    }

    private async Task Server_OnChannelCreate(RestChannel channel)
    {
        if (state.CurrentServer == null || state.CurrentServer.Server?.Id != channel.ServerId)
            return;

        bool CanManage = services.State.CurrentServer.CanManageChannel(services.State.CurrentServer.Member);
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            ChannelsList.Add(new ChannelItem { DataContext = new ChannelItemModel(services, state, channel, CanManage) });
        });

    }

    [ObservableProperty]
    private ObservableCollection<ChannelItem> _channelsList;
}
