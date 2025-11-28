using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Shared.Core.Accounts;
using LunarChatApp.Shared.Core.Channels;
using LunarChatApp.Views;

namespace LunarChatApp.Components;

public partial class DMListItemModel : ViewModelBase
{
    private ServiceManager services;
    private Relation user;

    public DMListItemModel(ServiceManager sv, Relation u)
    {
        services = sv;
        user = u;
        Name = u.display_name ?? u.username;
    }

    [ObservableProperty]
    private string? _name;

    [RelayCommand]
    public void OpenDM()
    {
        services.State.Socket.CurrentChannel = new Channel
        {
            Id = user.id,
            Name = user.display_name ?? user.username
        };

        services.State.Socket.TriggerSelectChannel(services.State.Socket.CurrentChannel, user);
    }
}
