using Avalonia.Controls;
using LunarChatApp.Views.Dialogs.Apps;

namespace LunarChatApp;

public partial class InviteAppPopup : UserControl
{
    public InviteAppPopup()
    {
        InitializeComponent();
    }

    private void Panel_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        if (e.Source is Panel && e.Source is not Grid)
            (DataContext as InviteAppPopupModel)?.Close();
    }
}