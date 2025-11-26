using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Shared.Core.Servers;
using LunarChatApp.Shared.Rest.Servers;
using LunarChatApp.ViewModels.Dialogs;
using LunarChatApp.Views;
using System.Threading.Tasks;

namespace LunarChatApp.ViewModels;

public partial class ServerIconViewModel : ViewModelBase
{
    private ServiceManager services;
    public ServerIconViewModel(ServiceManager sv, Server server)
    {
        services = sv;
        Name = server.Name;
        Fallback = server.GetFallbackName();
        Id = server.Id;
        if (Id == "0")
            ShowPlus = true;
    }

    [ObservableProperty]
    private bool _showPlus;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _fallback;

    [ObservableProperty]
    private IImage? _icon;

    public string Id;

    [RelayCommand]
    public void SelectedServer()
    {
        if (Id == "0")
        {
            services.Dialogs.Create(new JoinServerDialog(), new JoinServerDialogModel(services), "Server").WithSubmit(SubmitServer).Open();
        }
        else
        {
            if (Id == services.State.Socket.CurrentServer?.Server.Id)
                return;

            services.State.Socket.CurrentServer = services.State.Socket.Servers[Id];
            services.State.Socket.TriggerSelectServer(services.State.Socket.Servers[Id].Server);
        }
    }

    public async Task SubmitServer(UserControl control)
    {
        JoinServerDialogModel? model = control.DataContext as JoinServerDialogModel;
        if (model.ShowJoin)
        {

        }
        else if (model.ShowCreate)
        {
            if (string.IsNullOrEmpty(model.Textbox))
                return;

            await services.Rest.PostAsync<CreateServerRequest>("/servers", new CreateServerRequest
            {
                name = model.Textbox
            });
        }
    }
}
