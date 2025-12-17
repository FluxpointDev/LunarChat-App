using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Views;
using LunarChatSharp.Rest.Channels;
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
    }

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private bool canManage;

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
