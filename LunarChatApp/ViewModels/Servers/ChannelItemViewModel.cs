using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Shared.Core.Channels;

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
    }


    [RelayCommand]
    public void SelectChannel()
    {
        state.CurrentChannel = channel;
        state.TriggerSelectChannel(channel, null);
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
