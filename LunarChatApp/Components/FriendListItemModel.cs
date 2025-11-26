using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Shared.Core.Accounts;
using LunarChatApp.Views;

namespace LunarChatApp.Components;

public partial class FriendListItemModel : ViewModelBase
{
    private ServiceManager services;
    private Relation user;

    public FriendListItemModel(ServiceManager sv, Relation u)
    {
        services = sv;
        user = u;
        id = u.id;
        Username = u.username;
        DisplayName = u.display_name ?? u.username;
    }

    public string id;

    [ObservableProperty]
    private string? _username;

    [ObservableProperty]
    private string? _displayName;

    [RelayCommand]
    public void OpenMessages()
    {
        //services.State.Socket.CurrentChannel = new Channel
        //{
        //    Id = user.id,
        //    Name = user.display_name ?? user.username
        //};
        //services.State.Socket.TriggerSelectChannel(services.State.Socket.CurrentChannel, user);
    }
}
