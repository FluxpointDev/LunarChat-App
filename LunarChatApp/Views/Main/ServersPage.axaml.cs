using Avalonia.Controls;
using LunarChatApp.Services;
using LunarChatApp.Shared.Core.Servers;

namespace LunarChatApp;

[Page("servers")]
public partial class ServersPage : UserControl
{
    public ServersPage()
    {
        InitializeComponent();
    }

    public Server? SelectedServer;

    private void OpenSettings(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {

    }
}