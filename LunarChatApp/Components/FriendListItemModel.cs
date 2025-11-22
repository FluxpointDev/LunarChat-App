using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Shared.Core.Channels;
using LunarChatApp.Shared.Core.Users;
using LunarChatApp.ViewModels;

namespace LunarChatApp.Components;

public partial class FriendListItemModel : ViewModelBase
{
    private ServiceManager services;
    private User user;

    public FriendListItemModel(ServiceManager sv, User u)
    {
        services = sv;
        user = u;
    }
    [RelayCommand]
    public void OpenMessages()
    {
        services.State.CurrentChannel = new Channel
        {
            Id = "1",
            Name = user.DisplayName ?? user.Username
        };
        services.State.TriggerSelectChannel(services.State.CurrentChannel, user);
    }
}
