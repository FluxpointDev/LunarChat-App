using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Views;
using LunarChatSharp.Rest.Users;

namespace LunarChatApp.Components;

public partial class FriendListItemModel : ViewModelBase
{
    private ServiceManager services;
    private RestRelation user;

    public FriendListItemModel(ServiceManager sv, RestRelation u)
    {
        services = sv;
        user = u;
        id = u.UserId;
        Username = u.Username;
        DisplayName = u.DisplayName ?? u.Username;
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
