using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using LunarChatApp.Shared.WebSocket;
using LunarChatApp.ViewModels.Dialogs;

namespace LunarChatApp;

public partial class TestState : ObservableObject
{
    public StatusType StatusType;
    public string? StatusText;
    public ServersPage? CachedServersPage;
    public SocketState Socket;

    [ObservableProperty]
    private string _username = "test";

    [ObservableProperty]
    public string _displayName = "Test";


    public delegate void PageEventHandler(UserControl control);
    public event PageEventHandler? OnPageSelect;

    public void TriggerPageSelect(UserControl control)
    {
        OnPageSelect?.Invoke(control);
    }
}