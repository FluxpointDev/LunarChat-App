using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;

namespace LunarChatApp.ViewModels.Dialogs;

public partial class PopupMaskModel(DialogService Dialogs) : ViewModelBase
{
    internal DialogMenu? currentMenu { get; set; }
    public DialogMenu? CurrentMenu
    {
        get { return currentMenu; }
        set { currentMenu = value; ShowDialog = currentMenu != null; }
    }

    public void SetMenu(DialogMenu? menu)
    {
        Title = menu?.Title;
        Control = menu?.Control;
        CurrentMenu = menu;
    }

    [ObservableProperty]
    private string? _title;

    [ObservableProperty]
    private UserControl? _control;

    [ObservableProperty]
    private bool _showDialog;

    [ObservableProperty]
    private bool _dismissible = true;

    [RelayCommand]
    public void CloseDialog()
    {
        Dialogs.OnDialogClose.Invoke();
    }

    [RelayCommand]
    public void SubmitDialog()
    {
        CurrentMenu?.OnSubmit?.Invoke(CurrentMenu.Control);
        Dialogs.OnDialogClose.Invoke();
    }
}
