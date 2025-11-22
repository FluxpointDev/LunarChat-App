using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Shared.Core.Channels;
using LunarChatApp.Shared.Core.Users;
using LunarChatApp.ViewModels;

namespace LunarChatApp.Components;

public partial class DMListItemModel : ViewModelBase
{
    private ServiceManager services;
    private User user;

    public DMListItemModel(ServiceManager sv, User u)
    {
        services = sv;
        user = u;
        Name = u.DisplayName ?? u.Username;
    }

    [ObservableProperty]
    private string? _name;

    [RelayCommand]
    public void OpenDM()
    {
        services.State.CurrentChannel = new Channel
        {
            Id = "1",
            Name = Name
        };
        services.State.TriggerSelectChannel(services.State.CurrentChannel, user);
    }
}
