using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using LunarChatApp.Components;
using LunarChatApp.Services;
using LunarChatApp.Shared.Core.Accounts;
using LunarChatApp.Shared.Core.Messages;
using LunarChatApp.Shared.Rest.Messages;
using LunarChatApp.Shared.WebSocket.Events;
using LunarChatApp.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LunarChatApp.ViewModels;

public partial class ChannelViewModel : ViewModelBase
{
    private TestState state;
    private ServiceManager services;
    private Relation? user;
    public ChannelViewModel(TestState st, ServiceManager sv, Relation? u)
    {
        state = st;
        services = sv;
        user = u;
        Name = st.Socket.CurrentChannel.Name;
        state.Socket.WebSocket.OnMessageRecieved += State_OnMessageRecieved;
        state.Socket.OnMessageEdit += MessageEdit;
        state.Socket.OnMessageDelete += MessageDelete;
        if (u != null)
        {
            if (state.Socket.PrivateMessages.TryGetValue(state.Socket.CurrentChannel.Id, out var messages))
                CrockeryList = new ObservableCollection<MessageItem>(messages.Select(x => new MessageItem(st.Username, x.Content) { DataContext = new MessageItemModel(services, x.Id, x.AuthorId) }));
            else
                CrockeryList = new ObservableCollection<MessageItem>();
        }
        else
        {
            CrockeryList = new ObservableCollection<MessageItem>();
            _ = Task.Run(async () =>
            {
                Message[]? messages = await services.Rest.GetAsync<Message[]>("/channels/" + state.Socket.CurrentChannel.Id + "/messages");
                if (messages != null)
                {

                    Dispatcher.UIThread.Post(() => { CrockeryList.AddRange(messages.Select(x => new MessageItem(st.Username, x.Content) { DataContext = new MessageItemModel(services, x.Id, x.AuthorId) })); });
                }

            });



            //if (state.CurrentServer.Messages.TryGetValue(state.CurrentChannel.Id, out var messages))
            //    CrockeryList = new ObservableCollection<MessageItem>(messages.Select(x => new MessageItem(st.Username, x.Content)));
            //else
            //    CrockeryList = new ObservableCollection<MessageItem>();
        }
    }

    private void State_OnMessageRecieved(MessageRecievedEvent message)
    {
        if (message.channel_id != state.Socket.CurrentChannel?.Id)
            return;

        CrockeryList.Add(new MessageItem(message.user.Username, message.content) { DataContext = new MessageItemModel(services, message.id, message.user.Id) });
    }

    private async Task MessageEdit(Message message)
    {
        if (message.ChannelId != state.Socket.CurrentChannel?.Id)
            return;

        var messageItem = CrockeryList.FirstOrDefault(x => (x.DataContext as MessageItemModel).messageId == message.Id);
        if (messageItem != null)
            messageItem.Update(message.Content);
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
    private string? _textbox;

    [ObservableProperty]
    private ObservableCollection<MessageItem> _crockeryList;

    [RelayCommand]
    public async Task Enter()
    {

        if (user != null)
        {
            CrockeryList.Add(new MessageItem(state.Username, Textbox) { DataContext = new MessageItemModel(services, Guid.NewGuid().ToString(), "1") });
            if (state.Socket.PrivateMessages.ContainsKey("1"))
                state.Socket.PrivateMessages["1"].Add(new Message() { Content = Textbox });
            else
                state.Socket.PrivateMessages.TryAdd("1", new System.Collections.Generic.List<Message>
                {
                    new Message() { Content = Textbox }
                });
        }
        else
        {
            await services.Rest.PostAsync("/channels/" + state.Socket.CurrentChannel.Id + "/messages", new SendMessageRequest
            {
                content = Textbox
            });
        }

        Textbox = null;
    }

    [RelayCommand]
    public void Clear()
    {
        Textbox = null;
    }
}
