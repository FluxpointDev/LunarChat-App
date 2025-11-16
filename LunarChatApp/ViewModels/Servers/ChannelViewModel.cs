using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Shared.Core.Messages;
using System.Collections.ObjectModel;
using System.Linq;

namespace LunarChatApp.ViewModels;

public partial class ChannelViewModel : ViewModelBase
{
    public TestState state;
    public ChannelViewModel(TestState st)
    {
        state = st;
        Name = st.CurrentChannel.Name;
        if (state.CurrentServer.Messages.TryGetValue(state.CurrentChannel.Id, out var messages))
            CrockeryList = new ObservableCollection<MessageItem>(messages.Select(x => new MessageItem(st.Username, x.Content)));
        else
            CrockeryList = new ObservableCollection<MessageItem>();
    }

    [ObservableProperty]
    public string _name;

    [ObservableProperty]
    private string? _textbox;

    [ObservableProperty]
    private ObservableCollection<MessageItem> _crockeryList;

    [RelayCommand]
    public void Enter()
    {
        CrockeryList.Add(new MessageItem(state.Username, Textbox));
        state.CurrentServer.Messages[state.CurrentChannel.Id].Add(new Message() { Content = Textbox });
        Textbox = null;
    }

    [RelayCommand]
    public void Clear()
    {
        Textbox = null;
    }
}
