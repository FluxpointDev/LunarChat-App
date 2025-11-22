using CommunityToolkit.Mvvm.ComponentModel;
using LunarChatApp.Components;
using LunarChatApp.Services;
using System.Collections.ObjectModel;

namespace LunarChatApp.ViewModels.Main;

public partial class FriendsListModel : ViewModelBase
{
    public FriendsListModel(ServiceManager services)
    {
        _crockeryList = new ObservableCollection<FriendListItem>();
        _crockeryList.Add(new FriendListItem(new Shared.Core.Users.User { DisplayName = "Bob", Username = "bob" }) { DataContext = new FriendListItemModel(services, new Shared.Core.Users.User { DisplayName = "Bob", Username = "bob" }) });
    }

    [ObservableProperty]
    public string? _searchText;

    [ObservableProperty]
    private ObservableCollection<FriendListItem> _crockeryList;
}
