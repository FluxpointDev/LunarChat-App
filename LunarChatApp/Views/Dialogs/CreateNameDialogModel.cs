using CommunityToolkit.Mvvm.ComponentModel;

namespace LunarChatApp.Views.Dialogs;

public partial class CreateNameDialogModel : ViewModelBase
{
    [ObservableProperty]
    private string _name;
}
