using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Views;
using LunarChatSharp.Rest.Channels;
using LunarChatSharp.Rest.Users;

namespace LunarChatApp.Components;

public partial class DMListItemModel : ViewModelBase
{
    private ServiceManager services;
    private RestRelation user;

    public DMListItemModel(ServiceManager sv, RestRelation u)
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
        services.State.Socket.CurrentChannel = new RestChannel
        {
            Id = user.UserId,
            Name = user.DisplayName ?? user.Username,
            Type = LunarChatSharp.Core.Channels.ChannelType.Direct
        };

        services.Client.OnSelectChannel?.Invoke(services.State.Socket.CurrentChannel, user);
    }
}
