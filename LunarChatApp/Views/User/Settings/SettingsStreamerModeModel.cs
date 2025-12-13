using CommunityToolkit.Mvvm.ComponentModel;
using LunarChatApp.Services;

namespace LunarChatApp.Views.User.Settings;

public partial class SettingsStreamerModeModel : ViewModelBase
{
    private ServiceManager services;

    public SettingsStreamerModeModel(ServiceManager sv)
    {
        services = sv;
    }

    [ObservableProperty]
    private bool isEnabled;

    [ObservableProperty]
    private bool blockCalls = true;

    [ObservableProperty]
    private bool disableSounds = true;

    [ObservableProperty]
    private bool disableNotifications = true;

    [ObservableProperty]
    private bool hidePersonalInfo = true;

    [ObservableProperty]
    private bool hideInvites = true;

    [ObservableProperty]
    private bool hideAccountName = true;

    [ObservableProperty]
    private bool hidePrivateMessages = true;
}
