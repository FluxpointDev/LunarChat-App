using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Shared.Core.Servers;

namespace LunarChatApp.ViewModels;

public partial class ServerIconViewModel : ViewModelBase
{
    private TestState state;
    private PageManager pageManager;
    public ServerIconViewModel(TestState st, PageManager page, Server server)
    {
        state = st;
        pageManager = page;
        Name = server.Name;
        Id = server.Id;
    }

    [ObservableProperty]
    public string _name;

    public string Id;

    [RelayCommand]
    public void SelectedServer()
    {
        state.CurrentServer = state.Servers[Id];
        state.TriggerSelectServer(state.Servers[Id].Server);
    }
}
