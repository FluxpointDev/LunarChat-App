using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Shared.Core.Channels;

namespace LunarChatApp.ViewModels.Servers;

public partial class ChannelItemViewModel : ViewModelBase
{
    private TestState state;
    private Channel channel;
    public ChannelItemViewModel(TestState st, Channel chan)
    {
        state = st;
        channel = chan;
    }


    [RelayCommand]
    public void SelectChannel()
    {
        state.CurrentChannel = channel;
        state.TriggerSelectChannel(channel);
    }
}
