using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using LunarChatApp.Components;
using LunarChatApp.Services;
using LunarChatApp.Views;
using LunarChatSharp;
using LunarChatSharp.Rest.Channels;
using LunarChatSharp.Rest.Messages;
using LunarChatSharp.Rest.Users;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LunarChatApp.ViewModels;

public partial class ChannelModel : ViewModelBase
{
    private TestState state;
    public ServiceManager services;
    private RestRelation? user;
    public ChannelModel(TestState st, ServiceManager sv, RestRelation? u)
    {
        state = st;
        services = sv;
        user = u;
        Name = st.Socket.CurrentChannel.Name;
        Topic = st.Socket.CurrentChannel.Topic;
        state.Socket.CurrentServer.OnChannelUpdate += ChannelUpdate;
        state.Socket.OnMessageRecieved += State_OnMessageRecieved;
        state.Socket.OnMessageEdit += MessageEdit;
        state.Socket.OnMessageDelete += MessageDelete;

        CrockeryList = new ObservableCollection<MessageItem>();
        _ = Task.Run(async () =>
        {
            RestMessage[]? messages = await services.Rest.GetMessagesAsync(state.Socket.CurrentChannel.Id);
            if (messages != null)
            {
                Dispatcher.UIThread.Post(() => { CrockeryList.AddRange(messages.Select(x => new MessageItem() { DataContext = new MessageItemModel(services, x) })); MessagesFinished = true; });
            }
            else
            {
                Dispatcher.UIThread.Post(() =>
                {
                    MessagesFinished = true;
                });

            }
        });
    }

    public bool MessagesFinished = false;


    private async Task ChannelUpdate(RestChannel channel)
    {
        Name = channel.Name;
        Topic = channel.Topic;
    }

    private async Task State_OnMessageRecieved(RestMessage message)
    {
        if (message.ChannelId != state.Socket.CurrentChannel?.Id)
            return;

        CrockeryList.Add(new MessageItem()
        {
            DataContext = new MessageItemModel(services, new RestMessage
            {
                ChannelId = message.ChannelId,
                Author = message.Author,
                Content = message.Content,
                Id = message.Id,
                CreatedAt = message.CreatedAt
            })
        });
    }

    private async Task MessageEdit(RestMessage message)
    {
        if (message.ChannelId != state.Socket.CurrentChannel?.Id)
            return;

        var messageItem = CrockeryList.FirstOrDefault(x => (x.DataContext as MessageItemModel).messageId == message.Id);
        if (messageItem != null)
            (messageItem.DataContext as MessageItemModel).Update(message.Content);
    }

    private async Task MessageDelete(RestMessage message)
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
        Textbox = null;
        await services.Rest.SendMesssageAsync(state.Socket.CurrentChannel.Id, new CreateMessageRequest
        {
            Content = Textbox
        });
    }

    [RelayCommand]
    public void CopyChannelID()
    {
        services.CopyText(services.State.Socket.CurrentChannel?.Id);
    }

    [RelayCommand]
    public void Clear()
    {
        Textbox = null;
    }
}
