using LunarChatApp.Shared.Rest;
using LunarChatApp.ViewModels.Dialogs;
using ShadUI;

namespace LunarChatApp.Services;

public sealed class ServiceManager
{
    public bool IsDev = false;
    public readonly PageManager PageManager;
    public readonly TestState State;
    public readonly RestClient Rest;
    public readonly ThemeWatcher ThemeWatcher;
    public readonly DialogService Dialogs;
    public ServiceManager(PageManager page, TestState st, RestClient rs, ThemeWatcher theme, DialogService diag)
    {
        PageManager = page;
        State = st;
        Rest = rs;
        Rest.Initialize(IsDev ? "http://localhost:5156/" : "https://lunar.fluxpoint.dev/api/");
        ThemeWatcher = theme;
        Dialogs = diag;
    }
    public PopupMaskModel Popup;
}