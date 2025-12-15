using Avalonia.Controls;
using Avalonia.Input;
using LunarChatApp.Services;
using LunarChatSharp.Rest.Servers;
using System.Diagnostics;

namespace LunarChatApp;

[Page("servers")]
public partial class ServersPage : UserControl
{
    public ServersPage()
    {
        InitializeComponent();
        ServerPage.AddHandler(Gestures.PullGestureEndedEvent, (s, e) =>
        {
            Debug.WriteLine(s);
            Debug.WriteLine(e.PullDirection);
        });
    }

    public RestServer? SelectedServer;

    private void OpenSettings(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {

    }
}