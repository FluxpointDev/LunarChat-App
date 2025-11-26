using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Shared.Core.Channels;
using LunarChatApp.Views;

namespace LunarChatApp.ViewModels.Servers;

public partial class ChannelItemViewModel : ViewModelBase
{
    private TestState state;
    private Channel channel;
    private ServiceManager services;
    public ChannelItemViewModel(ServiceManager sv, TestState st, Channel chan)
    {
        state = st;
        channel = chan;
        services = sv;
        isOwner = st.Socket.CurrentServer?.Server.OwnerId == st.Socket.CurrentId;
    }

    [ObservableProperty]
    private bool isOwner;

    [RelayCommand]
    public void SelectChannel()
    {
        state.Socket.CurrentChannel = channel;
        state.Socket.TriggerSelectChannel(channel, null);
    }

    [RelayCommand]
    public void OpenChannelSettings()
    {
        services.PageManager.OnSwitchPage(new ChannelSettings
        {
            DataContext = new ChannelSettingsModel(services.PageManager, state, channel)
        });
    }
}
