using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Views;
using LunarChatApp.Views.Dialogs;
using LunarChatSharp;
using LunarChatSharp.Rest.Users;
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

    public string id;

    [ObservableProperty]
    private string? _username;

    [ObservableProperty]
    private string? _displayName;

    [RelayCommand]
    public void OpenMessages()
    {
        //services.State.Socket.CurrentChannel = new Channel
        //{
        //    Id = user.id,
        //    Name = user.display_name ?? user.username
        //};
        //services.State.Socket.TriggerSelectChannel(services.State.Socket.CurrentChannel, user);
    }

    [RelayCommand]
    public async Task RemoveFriend()
    {
        await services.Rest.RemoveFriendAsync(id);
    }

    [RelayCommand]
    public void OpenNote()
    {
        services.Dialogs.Create(new RelationNoteDialog(), new RelationNoteDialogModel
        {
            Username = id,
            Note = services.State.Socket.Relations.GetValueOrDefault(id)?.Note
        }, "Note").WithSubmit(SubmitNote).Open();
    }

    public async Task SubmitNote(UserControl control)
    {
        RelationNoteDialogModel model = control.DataContext as RelationNoteDialogModel;
        await services.Rest.UpdateNoteAsync(id, model.Note);
    }
}
