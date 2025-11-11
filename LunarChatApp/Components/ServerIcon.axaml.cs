using Avalonia;
using Avalonia.Controls;

namespace LunarChatApp;

public partial class ServerIcon : UserControl
{
    public ServerIcon()
    {
        InitializeComponent();
    }

    public static readonly StyledProperty<string> ServerNameProperty = AvaloniaProperty.Register<ServerIcon, string>(nameof(ServerName));

    public string ServerName
    {
        get { return GetValue(ServerNameProperty); }
        set { SetValue(ServerNameProperty, value); }
    }


    private void Clicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
    }
}