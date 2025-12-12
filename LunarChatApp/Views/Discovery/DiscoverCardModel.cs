using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Views.Dialogs;
using LunarChatSharp;
using LunarChatSharp.Rest.Dev;
using LunarChatSharp.Rest.Servers;
using System;
using System.Threading.Tasks;

namespace LunarChatApp.Views.Discovery;

public partial class DiscoverCardModel : ViewModelBase
{
    private ServiceManager services;
    public DiscoverCardModel(ServiceManager sv, RestServer server)
    {
        services = sv;
        id = server.Id;
        name = server.Name;
        description = server.Description;
        joinText = "Join Server";
    }

    public DiscoverCardModel(ServiceManager sv, RestApp app)
    {
        services = sv;
        id = app.Id;
        isApp = true;
        name = app.Name;
        description = app.Description;
        joinText = "Add App";
    }

    private string id;
    private bool isApp;

    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private string description;

    [ObservableProperty]
    private string joinText;

    [RelayCommand]
    public async Task Join()
    {

        if (isApp)
        {
            services.Dialogs.Create(new InviteAppDialog(), new InviteAppDialogModel(services, id), "Invite " + name).WithSubmit(InviteApp).Open();
        }
        else
        {
            try
            {
                await services.Rest.AddMemberAsync(id, services.Client.CurrentId);
                await Task.Delay(new TimeSpan(0, 0, 1));
                if (services.State.Socket.Servers.TryGetValue(id, out var getServer))
                    services.PageManager.SwitchServer(services, getServer.Server);
            }
            catch { }

        }
    }

    public async Task InviteApp(UserControl control)
    {
        try
        {
            InviteAppDialogModel? model = control.DataContext as InviteAppDialogModel;
            await services.Rest.AddServerAppAsync(model.SelectedServer.id, id);
        }
        catch { }
    }
}
