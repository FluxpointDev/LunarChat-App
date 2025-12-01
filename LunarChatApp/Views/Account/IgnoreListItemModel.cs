using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Views.Dialogs;
using LunarChatSharp.Rest.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LunarChatApp.Views.Account;

public partial class IgnoreListItemModel : ViewModelBase
{
    private ServiceManager services;
    private RestRelation user;

    public IgnoreListItemModel(ServiceManager sv, RestRelation u)
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
    public async Task RemoveIgnore()
    {
        await services.Rest.DeleteAsync($"/users/{id}/ignore");
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
        await services.Rest.PatchAsync($"/users/{id}/note", new UpdateNoteRequest
        {
            Note = model.Note
        });
    }
}
