using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using LunarChatApp.Components;
using LunarChatApp.Services;
using LunarChatApp.ViewModels.Servers;
using LunarChatApp.Views;
using LunarChatApp.Views.Dialogs;
using LunarChatApp.Views.Main;
using LunarChatSharp;
using LunarChatSharp.Core.Servers;
using LunarChatSharp.Rest.Channels;
using LunarChatSharp.Rest.Messages;
using LunarChatSharp.Rest.Servers;
using LunarChatSharp.Websocket.Events.Messages;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LunarChatApp.ViewModels;

public partial class ChannelViewModel : ViewModelBase
{
    private TestState state;
    public ServiceManager services;
    public ChannelViewModel(TestState st, ServiceManager sv)
    {
        state = st;
        services = sv;
        if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS())
            expandMembers = false;

        Name = st.Socket.CurrentChannel.Name;
        Topic = st.Socket.CurrentChannel.Topic;
        if (state.Socket.CurrentServer != null)
            state.Socket.CurrentServer.OnChannelUpdate += ChannelUpdate;

        services.State.OnExpandChannels += ExpandChannels;
        services.Client.OnMessageRecieved += State_OnMessageRecieved;
        services.Client.OnMessageEdit += MessageEdit;
        services.Client.OnMessageDelete += MessageDelete;
        if (state.Socket.CurrentChannel.Type == LunarChatSharp.Core.Channels.ChannelType.Group || state.Socket.CurrentChannel.Type == LunarChatSharp.Core.Channels.ChannelType.Direct)
        {
            canSend = true;
            if (st.Socket.CurrentChannel.Type == LunarChatSharp.Core.Channels.ChannelType.Group)
            {
                canLeaveGroup = st.Socket.CurrentChannel.GroupSettings?.OwnerId != services.Client.CurrentId;
                canDelete = st.Socket.CurrentChannel.GroupSettings?.OwnerId == services.Client.CurrentId;
                canAddFriend = true;
                canChangeName = st.Socket.CurrentChannel.GroupSettings?.OwnerId == services.Client.CurrentId;
                services.Client.OnGroupUpdate += ChannelUpdate;
                services.Client.OnGroupDelete += GroupDelete;
            }
            else
            {
                _name = st.Socket.CurrentChannel.Users.FirstOrDefault(x => x.Id != services.Client.CurrentId).GetCurrentNameDiscrim();
                services.Client.OnDMUpdate += ChannelUpdate;
            }
        }
        else
        {
            services.Client.OnMemberUpdate += MemberUpdate;
            if (services.State.Socket.CurrentServer != null)
            {
                inTimeout = (services.State.Socket.CurrentServer.Member.Timeout.HasValue && services.State.Socket.CurrentServer.Member.Timeout.Value < DateTime.UtcNow);
                canInvite = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ChannelPermission.CreateInvites);
                canManage = services.State.Socket.CurrentServer.CanManageChannel(services.State.Socket.CurrentServer.Member);
                canDelete = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ChannelPermission.ManageChannel);
                canSend = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ChannelPermission.SendMessages);
                services.State.Socket.CurrentServer.OnPermissionUpdate += PermissionUpdate;
                services.State.Socket.CurrentServer.OnChannelDelete += ChannelDelete;
            }
        }

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

    [RelayCommand]
    public async Task PasteImage()
    {
        var topLevel = TopLevel.GetTopLevel(services.MainControl)!;

        var data = await topLevel?.Clipboard?.TryGetDataAsync();
        if (data == null)
            return;

        try
        {
            var item = data.Items.FirstOrDefault();
            if (item == null)
                return;

            var file = await item.TryGetBitmapAsync();
            if (file == null)
                return;

            using (Stream stream = new MemoryStream())
            {
                file.Save(stream);
                await services.Rest.SendMesssageAsync(services.State.Socket.CurrentChannel?.Id, new CreateMessageRequest
                {
                    Attachments = new CreateAttachmentRequest[]
                    {
                        new CreateAttachmentRequest(stream, "image.png")
                    }
                });
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
        finally
        {
            data.Dispose();
        }

    }

    private async Task ExpandChannels(bool? value)
    {
        if (value.HasValue)
            IsChannelsExpanded = value.Value;
        else
            IsChannelsExpanded = !IsChannelsExpanded;
    }

    private async Task ChannelDelete(RestChannel channel)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            services.PageManager.SwitchServerChannel(services, null);
        });

    }

    private async Task GroupDelete(RestChannel channel)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            services.State.TriggerPageSelect(new HomeView() { DataContext = new HomeModel(services) });
        });

    }

    private async Task MemberUpdate(RestServer server, string arg2, EditMemberRequest request)
    {
        if (services.State.Socket.CurrentServer.Server.Id != server.Id)
            return;

        if (arg2 != services.State.Socket.CurrentServer.Member.Id)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (request.TimeoutRemove)
            {
                InTimeout = false;
            }
            else if (request.Timeout.HasValue)
            {
                InTimeout = true;
            }
        });

    }

    private async Task PermissionUpdate()
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            CanInvite = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ChannelPermission.CreateInvites);
            CanDelete = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ChannelPermission.ManageChannel);
            CanManage = services.State.Socket.CurrentServer.CanManageChannel(services.State.Socket.CurrentServer.Member);
            CanSend = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ChannelPermission.SendMessages);
        });
    }

    [ObservableProperty]
    private bool canManage;

    [ObservableProperty]
    private bool canDelete;

    [ObservableProperty]
    private bool canChangeName;

    [ObservableProperty]
    private bool canLeaveGroup;

    [ObservableProperty]
    private bool canAddFriend;

    [ObservableProperty]
    private bool canSend;

    [ObservableProperty]
    private bool inTimeout;

    [ObservableProperty]
    private bool canInvite;

    public bool MessagesFinished = false;


    private async Task ChannelUpdate(RestChannel channel, UpdateChannelRequest request)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (channel.Name != null)
                Name = channel.Name;

            if (channel.Topic != null)
                Topic = channel.Topic.ToNullOrString();
        });
    }

    private async Task State_OnMessageRecieved(RestChannel channel, RestMessage message)
    {
        if (message.ChannelId != state.Socket.CurrentChannel?.Id)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            CrockeryList.Add(new MessageItem()
            {
                DataContext = new MessageItemModel(services, new RestMessage
                {
                    ChannelId = message.ChannelId,
                    Author = message.Author,
                    Content = message.Content,
                    Id = message.Id,
                    CreatedAt = message.CreatedAt,
                    Source = message.Source,
                    SystemMessage = message.SystemMessage,
                    Attachments = message.Attachments
                })
            });
        });

    }

    private async Task MessageEdit(RestChannel channel, MessageUpdateEvent ev, EditMessageRequest message)
    {
        if (channel.Id != state.Socket.CurrentChannel?.Id)
            return;

        var messageItem = CrockeryList.FirstOrDefault(x => (x.DataContext as MessageItemModel).messageId == ev.MessageId);
        if (messageItem == null)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            (messageItem.DataContext as MessageItemModel).Update(ev, message);
        });
    }

    private async Task MessageDelete(RestChannel channel, RestMessage message)
    {
        if (message.ChannelId != state.Socket.CurrentChannel?.Id)
            return;

        var messageItem = CrockeryList.FirstOrDefault(x => (x.DataContext as MessageItemModel).messageId == message.Id);
        if (messageItem == null)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            CrockeryList.Remove(messageItem);
        });

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
        if (string.IsNullOrEmpty(Textbox))
            return;

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
    public void CreateInvite()
    {
        services.Dialogs.Create(new CreateInviteDialog(), new CreateInviteDialogModel(), "Create Invite").WithSubmit(SubmitInvite).Open();
    }

    public async Task SubmitInvite(UserControl control)
    {
        CreateInviteDialogModel? model = control.DataContext as CreateInviteDialogModel;
        if (model == null)
            return;

        CreateInviteRequest req = model.CreateRequest();
        try
        {
            RestInvite invite = await services.Rest.CreateInviteAsync(services.State.Socket.CurrentChannel?.Id, req);
            services.CopyText(invite.Code);
        }
        catch { }
    }

    [RelayCommand]
    public void ChangeGroupName()
    {
        services.Dialogs.Create(new CreateNameDialog(), new CreateNameDialogModel { Name = Name }, "Change Group Name").WithSubmit(SubmitGroupName).Open();
    }

    public async Task SubmitGroupName(UserControl control)
    {
        CreateNameDialogModel? model = control.DataContext as CreateNameDialogModel;
        if (model == null || string.IsNullOrEmpty(model.Name))
            return;

        try
        {
            await services.Rest.UpdateChannelAsync(services.State.Socket.CurrentChannel?.Id, new UpdateChannelRequest
            {
                Name = model.Name
            });
        }
        catch { }
    }

    [RelayCommand]
    public void AddFriend()
    {
        services.Dialogs.Create(new AddFriendDialog(), new AddFriendDialogModel(), "Add Friend to Group").WithSubmit(SubmitFriend).Open();
    }

    public async Task SubmitFriend(UserControl control)
    {
        try
        {
            AddFriendDialogModel? data = control.DataContext as AddFriendDialogModel;
            var friend = state.Socket.Relations.Values.FirstOrDefault(x => x.UserId == data.Username || x.Username == data.Username);
            await services.Rest.PutAsync($"/groups/{services.State.Socket.CurrentChannel?.Id}/users", new GroupAddUserRequest
            {
                UserId = friend.UserId
            });
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
    public void GroupSettings()
    {
        services.PageManager.OnSwitchPage(new ChannelSettings
        {
            DataContext = new ChannelSettingsModel(services, state.Socket.CurrentChannel)
        });
    }

    [RelayCommand]
    public async Task LeaveGroup()
    {
        try
        {
            await services.Rest.DeleteChannelAsync(state.Socket.CurrentChannel?.Id, new DeleteChannelRequest
            {

            });
        }
        catch { }
    }

    [RelayCommand]
    public async Task DeleteChannel()
    {
        try
        {
            await services.Rest.DeleteChannelAsync(state.Socket.CurrentChannel?.Id, new DeleteChannelRequest
            {
                ServerId = state.Socket.CurrentServer?.Server.Id
            });
        }
        catch { }
    }

    [RelayCommand]
    public async Task OpenFilePicker()
    {
        var files = await services.FilePicker();
        if (!files.Any())
            return;

        _ = Task.Run(async () =>
        {
            using (Stream stream = await files.First().OpenReadAsync())
            {
                await services.Rest.SendMesssageAsync(services.State.Socket.CurrentChannel?.Id, new CreateMessageRequest
                {
                    Attachments = new CreateAttachmentRequest[]
                    {
                        new CreateAttachmentRequest(stream, files.First().Name)
                    }
                });
            }
        });
    }

    [RelayCommand]
    public void ToggleExpandMembers()
    {
        ExpandMembers = !ExpandMembers;
    }

    [ObservableProperty]
    private bool expandMembers = true;

    [ObservableProperty]
    private bool isChannelsExpanded = true;

    [RelayCommand]
    public void ExpandChannels()
    {
        services.State.OnExpandChannels?.Invoke(null);
    }

    [RelayCommand]
    public void OpenTopic()
    {
        services.Dialogs.Create(new ChannelTopicDialog(), new ChannelTopicDialogModel(services, Topic), $"{Name} topic").Open();
    }

    [RelayCommand]
    public void Clear()
    {
        Textbox = null;
    }
}
