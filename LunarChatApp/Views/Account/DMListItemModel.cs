using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Views;
using LunarChatSharp.Rest.Channels;
using System;
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
        {
            var OtherUser = chan.Users.FirstOrDefault(x => x.Id != services.Client.CurrentId);
            _name = OtherUser.GetCurrentNameDiscrim();

            if (!string.IsNullOrEmpty(OtherUser.AvatarId))
                Avatar = new Uri(OtherUser.GetAvatarUrl());
            else
                fallback = OtherUser.GetFallback();
        }
        else
        {
            _name = chan.Name;


            if (!string.IsNullOrEmpty(chan.GroupSettings.IconId))
                Avatar = new Uri(chan.GroupSettings.GetIconUrl());
            else
                fallback = chan.GetFallback();
        }
    }

    public void Update(UpdateChannelRequest request)
    {
        if (request.Name != null)
            Name = request.Name;

        if (request.Icon != null)
        {
            if (!string.IsNullOrEmpty(request.Icon))
                Avatar = new Uri(channel.GroupSettings.GetIconUrl());
            else
            {
                Fallback = channel.GetFallback();
                Avatar = null;
            }
        }
    }

    [ObservableProperty]
    private string? _name;

    [ObservableProperty]
    private Uri? avatar;

    [ObservableProperty]
    private string fallback;

    [RelayCommand]
    public void OpenDM()
    {
        services.State.CurrentChannel = channel;

        services.Client.OnSelectChannel?.Invoke(services.State.CurrentChannel);
    }
}
