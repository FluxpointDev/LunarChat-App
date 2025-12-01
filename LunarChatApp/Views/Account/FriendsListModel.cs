using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Components;
using LunarChatApp.Services;
using LunarChatApp.Views;
using LunarChatApp.Views.Account;
using LunarChatApp.Views.Dialogs;
using LunarChatSharp.Core.Users;
using LunarChatSharp.Rest.Users;
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
        services.State.Socket.OnRelationAdd += OnRelationAdd;
        services.State.Socket.OnRelationRemove += OnRelationRemove;
        _friendsList = new ObservableCollection<FriendListItem>();
        foreach (var i in services.State.Socket.Relations.Values.Where(x => x.Type == LunarChatSharp.Core.Users.UserRelationType.Friend))
        {
            _friendsList.Add(new FriendListItem() { DataContext = new FriendListItemModel(services, i) });
        }

        _requestList = new ObservableCollection<FriendRequestListItem>();
        foreach (var i in services.State.Socket.Relations.Values.Where(x => x.Type == LunarChatSharp.Core.Users.UserRelationType.FriendRequest))
        {
            _requestList.Add(new FriendRequestListItem() { DataContext = new FriendRequestListItemModel(services, i) });
        }

        _ignoreList = new ObservableCollection<IgnoreListItem>();
        foreach (var i in services.State.Socket.Relations.Values.Where(x => x.Type == LunarChatSharp.Core.Users.UserRelationType.Ignored))
        {
            _ignoreList.Add(new IgnoreListItem() { DataContext = new IgnoreListItemModel(services, i) });
        }

        _blocksList = new ObservableCollection<BlockListItem>();
        foreach (var i in services.State.Socket.Relations.Values.Where(x => x.Type == LunarChatSharp.Core.Users.UserRelationType.Blocked))
        {
            _blocksList.Add(new BlockListItem() { DataContext = new BlockListItemModel(services, i) });
        }
    }

    private async Task OnRelationAdd(RestRelation user)
    {
        switch (user.Type)
        {
            case UserRelationType.Friend:
                FriendsList.Add(new FriendListItem() { DataContext = new FriendListItemModel(services, user) });
                break;
            case UserRelationType.FriendRequest:
                RequestList.Add(new FriendRequestListItem() { DataContext = new FriendRequestListItemModel(services, user) });
                break;
            case UserRelationType.Ignored:
                IgnoreList.Add(new IgnoreListItem() { DataContext = new IgnoreListItemModel(services, user) });
                break;
            case UserRelationType.Blocked:
                BlocksList.Add(new BlockListItem() { DataContext = new BlockListItemModel(services, user) });
                break;
        }
    }

    private async Task OnRelationRemove(RestRelation user)
    {
        var item1 = FriendsList.FirstOrDefault(x => ((FriendListItemModel)x.DataContext!).id == user.UserId);
        if (item1 != null)
            FriendsList.Remove(item1);

        var item2 = RequestList.FirstOrDefault(x => ((FriendRequestListItemModel)x.DataContext!).id == user.UserId);
        if (item2 != null)
            RequestList.Remove(item2);

        var item3 = IgnoreList.FirstOrDefault(x => ((IgnoreListItemModel)x.DataContext!).id == user.UserId);
        if (item3 != null)
            IgnoreList.Remove(item3);

        var item4 = BlocksList.FirstOrDefault(x => ((BlockListItemModel)x.DataContext!).id == user.UserId);
        if (item4 != null)
            BlocksList.Remove(item4);
    }

    [RelayCommand]
    public void AddFriend()
    {
        services.Dialogs.Create(new AddFriendDialog(), new AddFriendDialogModel(), "Add Friend").WithSubmit(SubmitFriend).Open();
    }

    public async Task SubmitFriend(UserControl control)
    {
        AddFriendDialogModel? data = control.DataContext as AddFriendDialogModel;
        await services.Rest.PutAsync("/users/" + data.Username + "/friend");
    }

    [RelayCommand]
    public void AddIgnore()
    {
        services.Dialogs.Create(new AddFriendDialog(), new AddFriendDialogModel(), "Add Ignore").WithSubmit(SubmitIgnore).Open();
    }

    public async Task SubmitIgnore(UserControl control)
    {
        AddFriendDialogModel? data = control.DataContext as AddFriendDialogModel;
        await services.Rest.PutAsync("/users/" + data.Username + "/ignore");
    }

    [RelayCommand]
    public void AddBlock()
    {
        services.Dialogs.Create(new AddFriendDialog(), new AddFriendDialogModel(), "Add Block").WithSubmit(SubmitBlock).Open();
    }

    public async Task SubmitBlock(UserControl control)
    {
        AddFriendDialogModel? data = control.DataContext as AddFriendDialogModel;
        await services.Rest.PutAsync("/users/" + data.Username + "/block");
    }

    [ObservableProperty]
    public string? _searchText;

    [ObservableProperty]
    private ObservableCollection<FriendListItem> _friendsList;

    [ObservableProperty]
    private ObservableCollection<FriendRequestListItem> _requestList;

    [ObservableProperty]
    private ObservableCollection<IgnoreListItem> _ignoreList;

    [ObservableProperty]
    private ObservableCollection<BlockListItem> _blocksList;
}
