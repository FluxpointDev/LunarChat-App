using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Shared.Core.Servers;
using LunarChatApp.Shared.Rest.Servers;
using LunarChatApp.ViewModels.Dialogs;
using LunarChatApp.Views;
using System;
using System.Threading.Tasks;

namespace LunarChatApp.ViewModels;

public partial class ServerIconModel : ViewModelBase
{
    private ServiceManager services;
    public ServerIconModel(ServiceManager sv, Server server)
    {
        services = sv;
        Name = server.Name;
        Fallback = server.GetFallbackName();
        Id = server.Id;
        switch (Name)
        {
            case "Fluxpoint Community":
                Icon = new Bitmap(AssetLoader.Open(new Uri("avares://LunarChatApp/Assets/fluxpoint.ico")));
                break;
            case "Lunar Community":
                Icon = new Bitmap(AssetLoader.Open(new Uri("avares://LunarChatApp/Assets/lunar-icon.png")));
                break;
            case "Lunar Developers":
                Icon = new Bitmap(AssetLoader.Open(new Uri("avares://LunarChatApp/Assets/lunar-dev-icon.png")));
                break;
        }

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
            if (services.State.Socket.Servers.TryGetValue(Id, out var server))
                services.PageManager.SwitchServer(services, server.Server);
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
