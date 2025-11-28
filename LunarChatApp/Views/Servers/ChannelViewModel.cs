using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using LunarChatApp.Components;
using LunarChatApp.Services;
using LunarChatApp.Shared.Core.Accounts;
using LunarChatApp.Shared.Core.Channels;
using LunarChatApp.Shared.Core.Messages;
using LunarChatApp.Shared.Rest.Messages;
using LunarChatApp.Shared.WebSocket.Events;
using LunarChatApp.Views;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LunarChatApp.ViewModels;

public partial class ChannelViewModel : ViewModelBase
{
    private TestState state;
    public ServiceManager services;
    private Relation? user;
    public ChannelViewModel(TestState st, ServiceManager sv, Relation? u)
    {
        state = st;
        services = sv;
        user = u;
        Name = st.Socket.CurrentChannel.Name;
        Topic = st.Socket.CurrentChannel.Topic;
        state.Socket.CurrentServer.OnChannelUpdate += ChannelUpdate;
        state.Socket.WebSocket.OnMessageRecieved += State_OnMessageRecieved;
        state.Socket.OnMessageEdit += MessageEdit;
        state.Socket.OnMessageDelete += MessageDelete;

        CrockeryList = new ObservableCollection<MessageItem>();
        _ = Task.Run(async () =>
        {
            Message[]? messages = await services.Rest.GetAsync<Message[]>("/channels/" + state.Socket.CurrentChannel.Id + "/messages");
            if (messages != null)
            {
                Dispatcher.UIThread.Post(() => { CrockeryList.AddRange(messages.Select(x => new MessageItem() { DataContext = new MessageItemModel(services, x) })); MessagesFinished = true; });
            }
            else
            {
                MessagesFinished = true;
            }
        });
    }

    public bool MessagesFinished = false;


    private async Task ChannelUpdate(Channel channel)
    {
        Name = channel.Name;
        Topic = channel.Topic;
    }

    private void State_OnMessageRecieved(MessageRecievedEvent message)
    {
        if (message.channel_id != state.Socket.CurrentChannel?.Id)
            return;

        CrockeryList.Add(new MessageItem()
        {
            DataContext = new MessageItemModel(services, new Message
            {
                ChannelId = message.channel_id,
                AuthorId = message.user.Id,
                Content = message.content,
                Id = message.id,
                Username = message.user.DisplayName ?? message.user.Username,
                CreatedAt = message.created_at
            })
        });
    }

    private async Task MessageEdit(Message message)
    {
        if (message.ChannelId != state.Socket.CurrentChannel?.Id)
            return;

        var messageItem = CrockeryList.FirstOrDefault(x => (x.DataContext as MessageItemModel).messageId == message.Id);
        if (messageItem != null)
            (messageItem.DataContext as MessageItemModel).Update(message.Content);
    }

    private async Task MessageDelete(Message message)
    {
        if (message.ChannelId != state.Socket.CurrentChannel?.Id)
            return;

        var messageItem = CrockeryList.FirstOrDefault(x => (x.DataContext as MessageItemModel).messageId == message.Id);
        if (messageItem != null)
            CrockeryList.Remove(messageItem);
    }

    [ObservableProperty]
    public string _name;

    [ObservableProperty]
    private string? _topic;

    [ObservableProperty]
    private string? _textbox;

    [ObservableProperty]
    private ObservableCollection<MessageItem> _crockeryList;

    [RelayCommand]
    public async Task Enter()
    {
        await services.Rest.PostAsync("/channels/" + state.Socket.CurrentChannel.Id + "/messages", new SendMessageRequest
        {
            content = Textbox
        });

        Textbox = null;
    }

    [RelayCommand]
    public void Clear()
    {
        Textbox = null;
    }
}
