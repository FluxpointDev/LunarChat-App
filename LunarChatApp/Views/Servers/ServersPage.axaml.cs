using Avalonia.Controls;
using LunarChatApp.Services;
using LunarChatSharp.Rest.Servers;

namespace LunarChatApp;

[Page("servers")]
public partial class ServersPage : UserControl
{
    public ServersPage()
    {
        InitializeComponent();
    }

    public RestServer? SelectedServer;

    private void OpenSettings(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {

    }
}