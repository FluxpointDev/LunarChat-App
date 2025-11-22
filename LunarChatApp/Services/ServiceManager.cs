using LunarChatApp.Shared.Rest;
using LunarChatApp.ViewModels.Dialogs;
using ShadUI;

namespace LunarChatApp.Services;

public sealed class ServiceManager
{
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
        ThemeWatcher = theme;
        Dialogs = diag;
        Dialogs.Register<CreateChannelDialog, CreateChannelDialogModel>();
        Dialogs.Register<ReportServerDialog, ReportServerDialogModel>();
        Dialogs.Register<StatusDialog, StatusDialogModel>();
    }
    public PopupMaskModel Popup;
}