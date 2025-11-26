using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Components;
using LunarChatApp.Services;
using LunarChatApp.Shared.Core.Accounts;
using LunarChatApp.ViewModels.Main;
using LunarChatApp.Views;
using System.Collections.ObjectModel;

namespace LunarChatApp.ViewModels.User;

public partial class DMsListModel : ViewModelBase
{
    private ServiceManager services;

    public DMsListModel(ServiceManager sv)
    {
        services = sv;
        _crockeryList = new ObservableCollection<DMListItem>();
        _crockeryList.Add(new DMListItem() { DataContext = new DMListItemModel(services, new Relation { username = "bob", display_name = "Bob", id = "1" }) });
    }

    [ObservableProperty]
    private ObservableCollection<DMListItem> _crockeryList;

    [RelayCommand]
    public void OpenHome()
    {
        services.State.TriggerPageSelect(new HomeView());
    }

    [RelayCommand]
    public void OpenFriends()
    {
        services.State.TriggerPageSelect(new FriendsList() { DataContext = new FriendsListModel(services) });
    }
}
