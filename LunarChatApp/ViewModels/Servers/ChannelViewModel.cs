using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using LunarChatApp.Services;
using LunarChatApp.Shared.Core.Messages;
using LunarChatApp.Shared.Rest.Messages;
using LunarChatApp.Shared.WebSocket;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LunarChatApp.ViewModels;

public partial class ChannelViewModel : ViewModelBase
{
    private TestState state;
    private ServiceManager services;
    private Shared.Core.Users.User? user;
    public ChannelViewModel(TestState st, ServiceManager sv, Shared.Core.Users.User? u)
    {
        state = st;
        services = sv;
        user = u;
        Name = st.CurrentChannel.Name;
        state.WebSocket.OnMessageRecieved += State_OnMessageRecieved;
        if (u != null)
        {
            if (state.PrivateMessages.TryGetValue(state.CurrentChannel.Id, out var messages))
                CrockeryList = new ObservableCollection<MessageItem>(messages.Select(x => new MessageItem(st.Username, x.Content)));
            else
                CrockeryList = new ObservableCollection<MessageItem>();
        }
        else
        {
            CrockeryList = new ObservableCollection<MessageItem>();
            _ = Task.Run(async () =>
            {
                Message[]? messages = await services.Rest.GetAsync<Message[]>("/channels/" + state.CurrentChannel.Id + "/messages");
                if (messages != null)
                {

                    Dispatcher.UIThread.Post(() => { CrockeryList.AddRange(messages.Select(x => new MessageItem(st.Username, x.Content))); });
                }

            });



            //if (state.CurrentServer.Messages.TryGetValue(state.CurrentChannel.Id, out var messages))
            //    CrockeryList = new ObservableCollection<MessageItem>(messages.Select(x => new MessageItem(st.Username, x.Content)));
            //else
            //    CrockeryList = new ObservableCollection<MessageItem>();
        }
    }

    private void State_OnMessageRecieved(SocketMessageRecieve message)
    {
        if (message.channel_id != state.CurrentChannel?.Id)
            return;

        CrockeryList.Add(new MessageItem(message.username, message.content));
    }

    [ObservableProperty]
    public string _name;

    [ObservableProperty]
    private string? _textbox;

    [ObservableProperty]
    private ObservableCollection<MessageItem> _crockeryList;

    [RelayCommand]
    public async Task Enter()
    {

        if (user != null)
        {
            CrockeryList.Add(new MessageItem(state.Username, Textbox));
            if (state.PrivateMessages.ContainsKey("1"))
                state.PrivateMessages["1"].Add(new Message() { Content = Textbox });
            else
                state.PrivateMessages.Add("1", new System.Collections.Generic.List<Message>
                {
                    new Message() { Content = Textbox }
                });
        }
        else
        {
            await services.Rest.PostAsync("/channels/" + state.CurrentChannel.Id + "/messages", new SendMessageRequest
            {
                content = Textbox
            });

            //state.CurrentServer.Messages[state.CurrentChannel.Id].Add(new Message() { Content = Textbox });
        }

        Textbox = null;
    }

    [RelayCommand]
    public void Clear()
    {
        Textbox = null;
    }
}
