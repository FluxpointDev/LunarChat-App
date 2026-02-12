using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Views;
using LunarChatApp.Views.Dialogs;
using LunarChatSharp;
using LunarChatSharp.Rest.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LunarChatApp.Components;

public partial class FriendListItemModel : ViewModelBase
{
    private ServiceManager services;
    private RestRelation user;

    public FriendListItemModel(ServiceManager sv, RestRelation u)
    {
        services = sv;
        user = u;
        id = u.UserId;
        Username = u.Username;
        DisplayName = u.DisplayName ?? u.Username;
    }

    public ulong id;

    [ObservableProperty]
    private string? _username;

    [ObservableProperty]
    private string? _displayName;

    [RelayCommand]
    public void CreateGroup()
    {
        services.Dialogs.Create(new CreateNameDialog(), new CreateNameDialogModel(), "Create Group").WithSubmit(SubmitGroup).Open();
    }

    public async Task SubmitGroup(UserControl control)
    {
        CreateNameDialogModel? model = control.DataContext as CreateNameDialogModel;
        if (model == null || string.IsNullOrEmpty(model.Name))
            return;

        try
        {
            var channel = await services.Rest.CreateChannelAsync(new LunarChatSharp.Rest.Channels.CreateChannelRequest
            {
                Name = model.Name,
                Type = LunarChatSharp.Core.Channels.ChannelType.Group,
                Users = new ulong[]
                {
                    id
                }
            });
            if (channel == null)
                return;

            await Task.Delay(new TimeSpan(0, 0, 1));
            services.State.CurrentChannel = channel;
            services.Client.OnSelectChannel?.Invoke(services.State.CurrentChannel);
        }
        catch { }
    }

    [RelayCommand]
    public async Task OpenMessages()
    {
        if (!services.Socket.State.PrivateChannels.TryGetValue(id, out var channel))
        {
            try
            {
                channel = await services.Rest.CreateChannelAsync(new LunarChatSharp.Rest.Channels.CreateChannelRequest
                {
                    Name = "",
                    Type = LunarChatSharp.Core.Channels.ChannelType.Direct,
                    Users = new ulong[]
                    {
                        id
                    }
                });
                await Task.Delay(new TimeSpan(0, 0, 1));
            }
            catch { }
        }
        if (channel == null)
            return;
        services.State.CurrentChannel = channel;
        services.Client.OnSelectChannel?.Invoke(services.State.CurrentChannel);
    }

    [RelayCommand]
    public async Task RemoveFriend()
    {
        try
        {
            await services.Rest.RemoveFriendAsync(id);
        }
        catch { }
    }

    [RelayCommand]
    public void OpenNote()
    {
        services.Dialogs.Create(new RelationNoteDialog(), new RelationNoteDialogModel
        {
            Username = Username ?? id.ToString(),
            Note = services.State.Socket.Relations.GetValueOrDefault(id)?.Note
        }, "Note").WithSubmit(SubmitNote).Open();
    }

    public async Task SubmitNote(UserControl control)
    {
        try
        {
            RelationNoteDialogModel model = control.DataContext as RelationNoteDialogModel;
            await services.Rest.UpdateNoteAsync(id, model.Note);
        }
        catch { }
    }
}
