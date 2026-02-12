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

public partial class BlockListItemModel : ViewModelBase
{
    private readonly ServiceManager services;

    public BlockListItemModel(ServiceManager sv, RestRelation u)
    {
        services = sv;
        id = u.UserId;
        Username = u.Username;
        DisplayName = u.DisplayName ?? u.Username;
        // Todo avatar
    }

    public ulong id;

    [ObservableProperty]
    private string? _username;

    [ObservableProperty]
    private string? _displayName;

    //[ObservableProperty]
    //private Uri? avatar;

    //[ObservableProperty]
    //private string fallback;

    [RelayCommand]
    public async Task RemoveBlock()
    {
        try
        {
            await services.Rest.RemoveBlockAsync(id);
        }
        catch { }
    }

    [RelayCommand]
    public void OpenNote()
    {
        services.Dialogs.Create(new RelationNoteDialog(), new RelationNoteDialogModel
        {
            Username = id.ToString(),
            Note = services.State.Socket.Relations.GetValueOrDefault(id)?.Note
        }, "Note").WithSubmit(SubmitNote).Open();
    }

    public async Task SubmitNote(UserControl control)
    {
        try
        {
            RelationNoteDialogModel? model = control.DataContext as RelationNoteDialogModel;
            if (model == null)
                return;

            await services.Rest.UpdateNoteAsync(id, model.Note);
        }
        catch { }
    }
}
