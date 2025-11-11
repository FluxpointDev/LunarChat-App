using Avalonia.Controls;
using LunarChatApp.Services;

namespace LunarChatApp;

[Page("servers")]
public partial class ServersPage : UserControl
{
    public ServersPage()
    {
        InitializeComponent();
    }

    public ServerData? SelectedServer;

    private void OpenSettings(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {

    }
}