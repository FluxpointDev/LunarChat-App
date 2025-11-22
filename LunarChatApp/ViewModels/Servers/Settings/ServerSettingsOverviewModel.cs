using CommunityToolkit.Mvvm.ComponentModel;

namespace LunarChatApp.ViewModels.Servers.Settings;

public partial class ServerSettingsOverviewModel : ViewModelBase
{
    public ServerSettingsOverviewModel(TestState state)
    {
        ServerNameEdit = state.CurrentServer.Server.Name;
    }

    [ObservableProperty]
    private string? _serverNameEdit;
}
