using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using LunarChatApp.Components;
using LunarChatApp.ViewModels;
using LunarChatApp.Views;

namespace LunarChatApp;

public partial class MessageItem : UserControl
{
    public MessageItem()
    {
        InitializeComponent();
    }

    private void Border_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        var ctl = sender as Control;
        if (ctl != null)
        {
            var Top = TopLevel.GetTopLevel(ctl);
            (Top.Content as MainView).UserPopup = new UserPopup { DataContext = new UserPopupModel(((Top.Content as MainView).DataContext as MainModel).services, (DataContext as MessageItemModel).authorId) };
            FlyoutBase.ShowAttachedFlyout(ctl);
        }
    }

    private void Flyout_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        e.Cancel = true;
    }
}