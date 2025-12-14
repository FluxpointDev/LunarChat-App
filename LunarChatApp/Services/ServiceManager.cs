using Avalonia;
using Avalonia.Controls;
using LunarChatApp.ViewModels.Dialogs;
using LunarChatSharp;
using LunarChatSharp.Rest;
using LunarChatSharp.Websocket;
using ShadUI;
using System;

namespace LunarChatApp.Services;

public sealed class ServiceManager
{
    public static bool IsDev = true;
    public readonly PageManager PageManager;
    public readonly TestState State;
    public readonly LunarClient Client;
    public readonly LunarRestClient Rest;
    public readonly LunarSocketClient Socket;
    public readonly ThemeWatcher ThemeWatcher;
    public readonly DialogService Dialogs;
    public readonly ToastManager ToastManager;
    public Visual MainControl;
    public ServiceManager(PageManager page, TestState st, LunarClient client, ThemeWatcher theme, DialogService diag, ToastManager toast)
    {
        PageManager = page;
        State = st;
        Client = client;
        Rest = Client.Rest;
        Socket = Client.WebSocket!;
        ThemeWatcher = theme;
        Dialogs = diag;
        ToastManager = toast;
    }

    public PopupMaskModel Popup;

    public void CopyText(string? text)
    {
        var topLevel = TopLevel.GetTopLevel(MainControl)!;
        if (topLevel != null && topLevel.Clipboard != null)
        {
            topLevel.Clipboard.SetTextAsync(text);

            ToastManager.CreateToast("Copied Text")
                .DismissOnClick()
                .WithDelay(3)
                .Show();
        }
    }

    public void OpenLink(Uri url)
    {
        var launcher = TopLevel.GetTopLevel(MainControl)!.Launcher;
        launcher.LaunchUriAsync(url);
    }
}