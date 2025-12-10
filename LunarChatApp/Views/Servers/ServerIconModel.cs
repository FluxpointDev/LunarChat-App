using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.ViewModels.Dialogs;
using LunarChatApp.Views;
using LunarChatSharp;
using LunarChatSharp.Rest.Servers;
using System;
using System.Threading.Tasks;

namespace LunarChatApp.ViewModels;

public partial class ServerIconModel : ViewModelBase
{
    private ServiceManager services;
    public ServerIconModel(ServiceManager sv, RestServer server)
    {
        services = sv;
        Name = server.Name;
        Fallback = server.GetFallback();
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

        if (string.IsNullOrEmpty(model.Textbox))
            return;

        if (model.ShowJoin)
        {
            try
            {
                var invite = await services.Rest.UseInviteAsync(model.Textbox);
                if (services.State.Socket.Servers.TryGetValue(invite.ServerId, out var getServer))
                    services.PageManager.SwitchServer(services, getServer.Server);
            }
            catch { }
        }
        else if (model.ShowCreate)
        {
            try
            {
                var server = await services.Rest.CreateServerAsync(new CreateServerRequest
                {
                    Name = model.Textbox
                });
                if (services.State.Socket.Servers.TryGetValue(server.Id, out var getServer))
                    services.PageManager.SwitchServer(services, getServer.Server);

            }
            catch { }
        }
    }
}
