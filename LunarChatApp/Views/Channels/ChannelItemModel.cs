using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Views;
using LunarChatSharp.Core.Channels;
using LunarChatSharp.Rest.Channels;
using Material.Icons;
using System;

namespace LunarChatApp.ViewModels.Servers;

public partial class ChannelItemModel : ViewModelBase
{
    private TestState state;
    private RestChannel channel;
    private ServiceManager services;
    public string id;
    public ChannelItemModel(ServiceManager sv, TestState st, RestChannel chan, bool manage)
    {
        id = chan.Id;
        state = st;
        channel = chan;
        services = sv;
        Name = chan.Name;
        canManage = manage;
        icon = GetIcon(chan.Type);
    }

    private MaterialIconKind GetIcon(ChannelType type)
    {
        switch (type)
        {
            case ChannelType.Voice:
                return MaterialIconKind.VolumeHigh;
                //case ChannelType.Media:
                //    return MaterialIconKind.Image;
                //case ChannelType.Schedule:
                //    return MaterialIconKind.Calendar;
                //case ChannelType.Rules:
                //    return MaterialIconKind.BookCheck;
        }
        return MaterialIconKind.Hashtag;
    }

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private bool canManage;

    [ObservableProperty]
    private MaterialIconKind icon;

    [RelayCommand]
    public void SelectChannel()
    {
        if (services.State.Socket.CurrentChannel?.Id == channel.Id)
            return;

        services.PageManager.SwitchServerChannel(services, channel);

        if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS())
            services.State.OnExpandChannels?.Invoke(false);
    }

    [RelayCommand]
    public void OpenChannelSettings()
    {
        services.PageManager.OnSwitchPage(new ChannelSettings
        {
            DataContext = new ChannelSettingsModel(services, channel)
        });
    }
}
