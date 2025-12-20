using CommunityToolkit.Mvvm.ComponentModel;

namespace LunarChatApp.Views.Dialogs;

public partial class RelationNoteDialogModel : ViewModelBase
{
    [ObservableProperty]
    private string _username;

    [ObservableProperty]
    private string? _note;
}
