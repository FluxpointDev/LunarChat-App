using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using LunarChatApp.Components;
using LunarChatApp.Services;
using LunarChatApp.ViewModels.Servers;
using LunarChatApp.Views;
using LunarChatSharp;
using LunarChatSharp.Core.Servers;
using LunarChatSharp.Rest.Channels;
using LunarChatSharp.Rest.Messages;
using LunarChatSharp.Rest.Servers;
using LunarChatSharp.Rest.Users;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LunarChatApp.ViewModels;

public partial class ChannelViewModel : ViewModelBase
{
    private TestState state;
    public ServiceManager services;
    private RestRelation? user;
    public ChannelViewModel(TestState st, ServiceManager sv, RestRelation? u)
    {
        state = st;
        services = sv;
        user = u;
        Name = st.Socket.CurrentChannel.Name;
        Topic = st.Socket.CurrentChannel.Topic;
        state.Socket.CurrentServer.OnChannelUpdate += ChannelUpdate;
        services.Client.OnMessageRecieved += State_OnMessageRecieved;
        services.Client.OnMessageEdit += MessageEdit;
        services.Client.OnMessageDelete += MessageDelete;
        services.Client.OnMemberUpdate += MemberUpdate;
        inTimeout = (services.State.Socket.CurrentServer.Member.Timeout.HasValue && services.State.Socket.CurrentServer.Member.Timeout.Value < DateTime.UtcNow);
        canInvite = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ChannelPermission.CreateInvites);
        canManage = services.State.Socket.CurrentServer.CanManageChannel(services.State.Socket.CurrentServer.Member);
        canDelete = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ChannelPermission.ManageChannel);
        canSend = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ChannelPermission.SendMessages);
        services.State.Socket.CurrentServer.OnPermissionUpdate += PermissionUpdate;

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

    private async Task MemberUpdate(RestServer server, string arg2, EditMemberRequest request)
    {
        if (services.State.Socket.CurrentServer.Server.Id != server.Id)
            return;

        if (arg2 != services.State.Socket.CurrentServer.Member.Id)
            return;

        if (request.TimeoutRemove)
        {
            InTimeout = false;
        }
        else if (request.Timeout.HasValue)
        {
            InTimeout = true;
        }
    }

    private async Task PermissionUpdate()
    {
        CanInvite = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ChannelPermission.CreateInvites);
        CanDelete = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ChannelPermission.ManageChannel);
        CanManage = services.State.Socket.CurrentServer.CanManageChannel(services.State.Socket.CurrentServer.Member);
        CanSend = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ChannelPermission.SendMessages);
    }

    [ObservableProperty]
    private bool canManage;

    [ObservableProperty]
    private bool canDelete;

    [ObservableProperty]
    private bool canSend;

    [ObservableProperty]
    private bool inTimeout;

    [ObservableProperty]
    private bool canInvite;

    public bool MessagesFinished = false;


    private async Task ChannelUpdate(RestChannel channel)
    {
        Name = channel.Name;
        Topic = channel.Topic;
    }

    private async Task State_OnMessageRecieved(RestChannel channel, RestMessage message)
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

    private async Task MessageEdit(RestChannel channel, RestMessage message)
    {
        if (message.ChannelId != state.Socket.CurrentChannel?.Id)
            return;

        var messageItem = CrockeryList.FirstOrDefault(x => (x.DataContext as MessageItemModel).messageId == message.Id);
        if (messageItem != null)
            (messageItem.DataContext as MessageItemModel).Update(message.Content);
    }

    private async Task MessageDelete(RestChannel channel, RestMessage message)
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
        try
        {
            await services.Rest.SendMesssageAsync(state.Socket.CurrentChannel.Id, new CreateMessageRequest
            {
                Content = Textbox
            });
        }
        catch { }

        Textbox = null;
    }

    [RelayCommand]
    public void CopyChannelID()
    {
        services.CopyText(services.State.Socket.CurrentChannel?.Id);
    }

    [RelayCommand]
    public async Task CreateInvite()
    {
        try
        {
            RestInvite invite = await services.Rest.CreateInviteAsync(services.State.Socket.CurrentChannel?.Id);
            services.CopyText(invite.Code);
        }
        catch { }
    }

    [RelayCommand]
    public void ChannelSettings()
    {
        services.PageManager.OnSwitchPage(new ChannelSettings
        {
            DataContext = new ChannelSettingsModel(services, state.Socket.CurrentChannel)
        });
    }

    [RelayCommand]
    public async Task DeleteChannel()
    {
        try
        {
            await services.Rest.DeleteChannelAsync(state.Socket.CurrentChannel?.Id, new DeleteChannelRequest
            {
                ServerId = state.Socket.CurrentServer.Server.Id
            });
        }
        catch { }
    }

    [RelayCommand]
    public void Clear()
    {
        Textbox = null;
    }
}
