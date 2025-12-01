using Avalonia.Controls;
using LunarChatApp.ViewModels.Dialogs;
using LunarChatSharp.Rest;
using ShadUI;

namespace LunarChatApp.Services;

public sealed class ServiceManager
{
    public bool IsDev = true;
    public readonly PageManager PageManager;
    public readonly TestState State;
    public readonly LunarRestClient Rest;
    public readonly ThemeWatcher ThemeWatcher;
    public readonly DialogService Dialogs;
    public ServiceManager(PageManager page, TestState st, LunarRestClient rs, ThemeWatcher theme, DialogService diag)
    {
        PageManager = page;
        State = st;
        Rest = rs;
        Rest.Initialize(IsDev ? "http://localhost:5156/" : "https://lunar.fluxpoint.dev/api/");
        ThemeWatcher = theme;
        Dialogs = diag;
    }
    public PopupMaskModel Popup;

    public void CopyText(string? text)
    {
        var topLevel = TopLevel.GetTopLevel(State.CachedServersPage)!;
        if (topLevel.Clipboard != null)
            topLevel.Clipboard.SetTextAsync(text);
    }
}