using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Views;
using LunarChatSharp.Rest.Channels;
using System.Linq;

namespace LunarChatApp.Components;

public partial class DMListItemModel : ViewModelBase
{
    private ServiceManager services;
    private RestChannel channel;
    public string id;
    public DMListItemModel(ServiceManager sv, RestChannel chan)
    {
        services = sv;
        id = chan.Id;
        channel = chan;
        if (chan.Type == LunarChatSharp.Core.Channels.ChannelType.Direct)
            _name = chan.Users.FirstOrDefault(x => x.Id != services.Client.CurrentId).GetCurrentNameDiscrim();
        else
            _name = chan.Name;
    }

    [ObservableProperty]
    private string? _name;

    [RelayCommand]
    public void OpenDM()
    {
        services.State.Socket.CurrentChannel = channel;

        services.Client.OnSelectChannel?.Invoke(services.State.Socket.CurrentChannel);
    }
}
