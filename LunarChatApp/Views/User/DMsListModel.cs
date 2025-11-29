using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.ViewModels.Main;
using LunarChatApp.Views;
using LunarChatApp.Views.Main;
using System.Collections.ObjectModel;

namespace LunarChatApp.ViewModels.User;

public partial class DMsListModel : ViewModelBase
{
    private ServiceManager services;

    public DMsListModel(ServiceManager sv)
    {
        services = sv;
        _crockeryList = new ObservableCollection<DMListItem>();
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
