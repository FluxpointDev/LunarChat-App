using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Components;
using LunarChatApp.Services;
using LunarChatApp.Shared.Core.Accounts;
using LunarChatApp.Views;
using LunarChatApp.Views.Dialogs;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LunarChatApp.ViewModels.Main;

public partial class FriendsListModel : ViewModelBase
{
    private ServiceManager services;
    public FriendsListModel(ServiceManager sv)
    {
        services = sv;
        services.State.Socket.OnFriendAdd += OnFriendAdd;
        services.State.Socket.OnFriendRemove += OnFriendRemove;
        services.State.Socket.OnBlockAdd += OnBlockAdd;
        services.State.Socket.OnBlockRemove += OnBlockRemove;
        _friendsList = new ObservableCollection<FriendListItem>();
        foreach (var i in services.State.Socket.Friends.Values)
        {
            _friendsList.Add(new FriendListItem() { DataContext = new FriendListItemModel(services, i) });
        }

        _blocksList = new ObservableCollection<BlockListItem>();
        foreach (var i in services.State.Socket.Blocks.Values)
        {
            _blocksList.Add(new BlockListItem() { DataContext = new BlockListItemModel(services, i) });
        }
    }

    private async Task OnFriendAdd(Relation user)
    {
        FriendsList.Add(new FriendListItem() { DataContext = new FriendListItemModel(services, user) });
    }

    private async Task OnFriendRemove(Relation user)
    {
        var item = FriendsList.FirstOrDefault(x => ((FriendListItemModel)x.DataContext!).id == user.id);
        if (item == null)
            return;
        FriendsList.Remove(item);
    }

    private async Task OnBlockAdd(Relation user)
    {
        BlocksList.Add(new BlockListItem() { DataContext = new BlockListItemModel(services, user) });
    }

    private async Task OnBlockRemove(Relation user)
    {
        var item = BlocksList.FirstOrDefault(x => ((BlockListItemModel)x.DataContext!).id == user.id);
        if (item == null)
            return;
        BlocksList.Remove(item);
    }

    [RelayCommand]
    public void AddFriend()
    {
        services.Dialogs.Create(new AddFriendDialog(), new AddFriendDialogModel(), "Add Friend").WithSubmit(SubmitFriend).Open();
    }

    public async Task SubmitFriend(UserControl control)
    {
        AddFriendDialogModel? data = control.DataContext as AddFriendDialogModel;
        await services.Rest.PutAsync("/users/" + data.StatusText + "/friend");
    }

    [RelayCommand]
    public void AddBlock()
    {
        services.Dialogs.Create(new AddFriendDialog(), new AddFriendDialogModel(), "Add Block").WithSubmit(SubmitBlock).Open();
    }

    public async Task SubmitBlock(UserControl control)
    {
        AddFriendDialogModel? data = control.DataContext as AddFriendDialogModel;
        await services.Rest.PutAsync("/users/" + data.StatusText + "/block");
    }

    [ObservableProperty]
    public string? _searchText;

    [ObservableProperty]
    private ObservableCollection<FriendListItem> _friendsList;

    [ObservableProperty]
    private ObservableCollection<BlockListItem> _blocksList;
}
