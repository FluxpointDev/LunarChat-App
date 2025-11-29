using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Shared.Core.Channels;
using LunarChatApp.Views;

namespace LunarChatApp.ViewModels.Servers;

public partial class ChannelItemModel : ViewModelBase
{
    private TestState state;
    private Channel channel;
    private ServiceManager services;
    public string id;
    public ChannelItemModel(ServiceManager sv, TestState st, Channel chan)
    {
        id = chan.Id;
        state = st;
        channel = chan;
        services = sv;
        Name = chan.Name;
        isOwner = st.Socket.CurrentServer?.Server.OwnerId == st.Socket.CurrentId;
    }

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private bool isOwner;

    [RelayCommand]
    public void SelectChannel()
    {
        services.PageManager.SwitchServerChannel(services, channel);
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
