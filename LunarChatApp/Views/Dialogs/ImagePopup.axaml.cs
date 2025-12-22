using Avalonia.Controls;
using LunarChatApp.Views.Dialogs;

namespace LunarChatApp;

public partial class ImagePopup : UserControl
{
    public ImagePopup()
    {
        InitializeComponent();
    }

    private void Panel_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        if (e.Source is not Image)
            (DataContext as ImagePopupModel)!.Close();
    }
}