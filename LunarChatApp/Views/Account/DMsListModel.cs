using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Components;
using LunarChatApp.Services;
using LunarChatApp.ViewModels.Main;
using LunarChatApp.Views;
using LunarChatApp.Views.Main;
using LunarChatSharp.Rest.Channels;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LunarChatApp.ViewModels.User;

public partial class DMsListModel : ViewModelBase
{
    private ServiceManager services;

    public DMsListModel(ServiceManager sv)
    {
        services = sv;
        services.Client.OnReady += Ready;
        services.Client.OnDMCreate += ChannelCreate;
        services.Client.OnGroupCreate += ChannelCreate;
        services.Client.OnDMUpdate += ChannelUpdate;
        services.Client.OnGroupUpdate += ChannelUpdate;
        services.Client.OnGroupDelete += GroupDelete;
        _crockeryList = new ObservableCollection<DMListItem>(services.Socket.State.PrivateChannels.Select(x => new DMListItem
        {
            DataContext = new DMListItemModel(services, x.Value)
        }));
    }

    private async Task GroupDelete(RestChannel channel)
    {
        var item = CrockeryList.FirstOrDefault(x => (x.DataContext as DMListItemModel).id == channel.Id);
        if (item == null)
            return;
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            CrockeryList.Remove(item);
        });
    }

    private async Task ChannelUpdate(RestChannel channel, UpdateChannelRequest request)
    {
        var item = CrockeryList.FirstOrDefault(x => (x.DataContext as DMListItemModel).id == channel.Id);
        if (item == null)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (request.Name != null)
                item.Name = request.Name;
        });

    }

    private async Task ChannelCreate(RestChannel channel)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            CrockeryList.Add(new DMListItem
            {
                DataContext = new DMListItemModel(services, channel)
            });
        });

    }

    private async Task Ready()
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            CrockeryList = new ObservableCollection<DMListItem>(services.Socket.State.PrivateChannels.Select(x => new DMListItem
            {
                DataContext = new DMListItemModel(services, x.Value)
            }));
        });
    }

    [ObservableProperty]
    private ObservableCollection<DMListItem> _crockeryList;

    [RelayCommand]
    public void OpenHome()
    {
        services.State.TriggerPageSelect(new HomeView() { DataContext = new HomeModel(services) });
    }

    [RelayCommand]
    public void OpenFriends()
    {
        services.State.TriggerPageSelect(new FriendsList() { DataContext = new FriendsListModel(services) });
    }
}
