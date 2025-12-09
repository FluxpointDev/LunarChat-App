using Avalonia;
using Avalonia.Controls;
using LunarChatApp.ViewModels.Dialogs;
using LunarChatSharp;
using LunarChatSharp.Rest;
using LunarChatSharp.Websocket;
using ShadUI;

namespace LunarChatApp.Services;

public sealed class ServiceManager
{
    public static bool IsDev = false;
    public readonly PageManager PageManager;
    public readonly TestState State;
    public readonly LunarClient Client;
    public readonly LunarRestClient Rest;
    public readonly LunarSocketClient Socket;
    public readonly ThemeWatcher ThemeWatcher;
    public readonly DialogService Dialogs;
    public Visual MainControl;
    public ServiceManager(PageManager page, TestState st, LunarClient client, ThemeWatcher theme, DialogService diag)
    {
        PageManager = page;
        State = st;
        Client = client;
        Rest = Client.Rest;
        Socket = Client.WebSocket!;
        ThemeWatcher = theme;
        Dialogs = diag;
    }
    public PopupMaskModel Popup;

    public void CopyText(string? text)
    {
        var topLevel = TopLevel.GetTopLevel(MainControl)!;
        if (topLevel != null && topLevel.Clipboard != null)
            topLevel.Clipboard.SetTextAsync(text);
    }
}