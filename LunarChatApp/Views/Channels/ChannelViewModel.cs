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
using LunarChatSharp.Rest.Helpers;
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
    public TestState state { get; set; }
    public ServiceManager services;
    public ChannelViewModel(TestState st, ServiceManager sv)
    {
        state = st;
        services = sv;
        if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS())
            expandMembers = false;

        Name = st.CurrentChannel.Name;
        Topic = st.CurrentChannel.Topic;
        if (state.CurrentServer != null)
            services.Client.OnChannelUpdate += ChannelUpdate;

        services.State.UseEmoji += UseEmoji;
        services.State.OnExpandChannels += ExpandChannels;
        services.Client.OnMessageRecieved += State_OnMessageRecieved;
        services.Client.OnMessageEdit += MessageEdit;
        services.Client.OnMessageDelete += MessageDelete;
        if (state.CurrentChannel.Type == LunarChatSharp.Core.Channels.ChannelType.Group || state.CurrentChannel.Type == LunarChatSharp.Core.Channels.ChannelType.Direct)
        {
            canSend = true;
            if (st.CurrentChannel.Type == LunarChatSharp.Core.Channels.ChannelType.Group)
            {
                canLeaveGroup = st.CurrentChannel.GroupSettings?.OwnerId != services.Client.CurrentId;
                canDelete = st.CurrentChannel.GroupSettings?.OwnerId == services.Client.CurrentId;
                canAddFriend = true;
                canChangeName = st.CurrentChannel.GroupSettings?.OwnerId == services.Client.CurrentId;
                services.Client.OnGroupUpdate += ChannelUpdate;
                services.Client.OnGroupDelete += GroupDelete;
            }
            else
            {
                _name = st.CurrentChannel.Users.FirstOrDefault(x => x.Id != services.Client.CurrentId).GetCurrentNameDiscrim();
                services.Client.OnDMUpdate += ChannelUpdate;
            }
        }
        else
        {
            services.Client.OnMemberUpdate += MemberUpdate;
            if (services.State.CurrentServer != null)
            {
                inTimeout = (services.State.CurrentServer.Member.Timeout.HasValue && services.State.CurrentServer.Member.Timeout.Value < DateTime.UtcNow);
                canInvite = services.State.CurrentServer.HasPermission(services.State.CurrentServer.Member, ChannelPermission.CreateInvites);
                canManage = services.State.CurrentServer.CanManageChannel(services.State.CurrentServer.Member);
                canDelete = services.State.CurrentServer.HasPermission(services.State.CurrentServer.Member, ChannelPermission.ManageChannel);
                canSend = services.State.CurrentServer.HasPermission(services.State.CurrentServer.Member, ChannelPermission.SendMessages);
                services.State.CurrentServer.OnPermissionUpdate += PermissionUpdate;
                services.Client.OnChannelDelete += ChannelDelete;
            }
        }

        CrockeryList = new ObservableCollection<MessageItem>();
        _ = Task.Run(async () =>
        {
            RestMessage[]? messages = await services.Rest.GetMessagesAsync(state.CurrentChannel.Id);
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

    private async Task UseEmoji(EmojiListItemModel model)
    {
        if (services.State.CurrentChannel == null || services.State.EmojisMenu.ReactionMessage == null)
            return;
        await services.Rest.AddReactionAsync(services.State.CurrentChannel.Id, services.State.EmojisMenu.ReactionMessage.Value, model.emojiId);
    }

    [RelayCommand]
    public async Task PasteImage()
    {
        if (services.State.CurrentChannel == null)
            return;

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

                CreateAttachmentRequest attach = await CreateAttachmentRequest.CreateFromStream(stream, "image.png");
                await services.Rest.SendMesssageAsync(services.State.CurrentChannel.Id, new CreateMessageRequest
                {
                    Attachments = new CreateAttachmentRequest[]
                    {
                        attach
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
        if (services.State.CurrentChannel?.Id != channel.Id)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            services.PageManager.SwitchServerChannel(services, null);
        });

    }

    private async Task GroupDelete(RestChannel channel)
    {
        if (services.State.CurrentChannel?.Id != channel.Id)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            services.State.TriggerPageSelect(new HomeView() { DataContext = new HomeModel(services) });
        });

    }

    private async Task MemberUpdate(RestServer server, ulong arg2, EditMemberRequest request)
    {
        if (services.State.CurrentServer.Server.Id != server.Id)
            return;

        if (arg2 != services.State.CurrentServer.Member.Id)
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

    private async Task PermissionUpdate(RestServer server)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            CanInvite = services.State.CurrentServer.HasPermission(services.State.CurrentServer.Member, ChannelPermission.CreateInvites);
            CanDelete = services.State.CurrentServer.HasPermission(services.State.CurrentServer.Member, ChannelPermission.ManageChannel);
            CanManage = services.State.CurrentServer.CanManageChannel(services.State.CurrentServer.Member);
            CanSend = services.State.CurrentServer.HasPermission(services.State.CurrentServer.Member, ChannelPermission.SendMessages);
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
        if (services.State.CurrentChannel?.Id != channel.Id)
            return;

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
        if (message.ChannelId != state.CurrentChannel?.Id)
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
        if (channel.Id != state.CurrentChannel?.Id)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            var messageItem = CrockeryList.FirstOrDefault(x => (x.DataContext as MessageItemModel).messageId == ev.MessageId);
            if (messageItem == null)
                return;

            (messageItem.DataContext as MessageItemModel).Update(ev, message);
        });
    }

    private async Task MessageDelete(RestChannel channel, RestMessage message)
    {
        if (message.ChannelId != state.CurrentChannel?.Id)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            var messageItem = CrockeryList.FirstOrDefault(x => (x.DataContext as MessageItemModel).messageId == message.Id);
            if (messageItem == null)
                return;

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
            await services.Rest.SendMesssageAsync(state.CurrentChannel.Id, new CreateMessageRequest
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
        services.CopyText(services.State.CurrentChannel?.Id.ToString());
    }

    [RelayCommand]
    public void CreateInvite()
    {
        services.Dialogs.Create(new CreateInviteDialog(), new CreateInviteDialogModel(), "Create Invite").WithSubmit(SubmitInvite).Open();
    }

    public async Task SubmitInvite(UserControl control)
    {
        if (services.State.CurrentChannel == null)
            return;

        CreateInviteDialogModel? model = control.DataContext as CreateInviteDialogModel;
        if (model == null)
            return;

        CreateInviteRequest req = model.CreateRequest();
        try
        {
            RestInvite invite = await services.Rest.CreateInviteAsync(services.State.CurrentChannel.Id, req);
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
        if (services.State.CurrentChannel == null)
            return;

        CreateNameDialogModel? model = control.DataContext as CreateNameDialogModel;
        if (model == null || string.IsNullOrEmpty(model.Name))
            return;

        try
        {
            await services.Rest.UpdateChannelAsync(services.State.CurrentChannel.Id, new UpdateChannelRequest
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
            var friend = state.Socket.Relations.Values.FirstOrDefault(x => x.Username == data.Username);
            await services.Rest.PutAsync($"/groups/{services.State.CurrentChannel?.Id}/users", new GroupAddUserRequest
            {
                UserId = friend.UserId
            });
        }
        catch { }
    }

    [RelayCommand]
    public void ChannelSettings()
    {
        if (state.CurrentChannel == null)
            return;

        services.PageManager.OnSwitchPage(new ChannelSettings
        {
            DataContext = new ChannelSettingsModel(services, state.CurrentChannel)
        });
    }

    [RelayCommand]
    public void GroupSettings()
    {
        if (state.CurrentChannel == null)
            return;

        services.PageManager.OnSwitchPage(new ChannelSettings
        {
            DataContext = new ChannelSettingsModel(services, state.CurrentChannel)
        });
    }

    [RelayCommand]
    public async Task LeaveGroup()
    {
        if (state.CurrentChannel == null)
            return;

        try
        {
            await services.Rest.DeleteChannelAsync(state.CurrentChannel.Id, new DeleteChannelRequest
            {

            });
        }
        catch { }
    }

    [RelayCommand]
    public async Task DeleteChannel()
    {
        if (state.CurrentChannel == null)
            return;

        try
        {
            await services.Rest.DeleteChannelAsync(state.CurrentChannel.Id, new DeleteChannelRequest
            {
                ServerId = state.CurrentServer?.Server.Id
            });
        }
        catch { }
    }

    [RelayCommand]
    public async Task OpenFilePicker()
    {
        if (state.CurrentChannel == null)
            return;
        Console.WriteLine("Picked file");
        try
        {
            var files = await services.FilePicker();
            if (!files.Any())
                return;

            Console.WriteLine("Got file");

            _ = Task.Run(async () =>
            {
                Console.WriteLine("Read stream");
                Console.WriteLine("Name: " + files.First().Name);
                try
                {
                    using (Stream stream = await files.First().OpenReadAsync())
                    {
                        CreateAttachmentRequest attach = await CreateAttachmentRequest.CreateFromStream(stream, files.First().Name);

                        await services.Rest.SendMesssageAsync(services.State.CurrentChannel.Id, new CreateMessageRequest
                        {
                            Attachments = new CreateAttachmentRequest[]
                            {
                                attach
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
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
