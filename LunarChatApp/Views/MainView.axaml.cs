using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LunarChatApp.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
    }

    public UserPopup? UserPopup { get; set; }
}