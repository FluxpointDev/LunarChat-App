using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.ViewModels;
using LunarChatApp.Views.Dialogs;
using LunarChatApp.Views.Dialogs.Apps;
using LunarChatSharp;
using LunarChatSharp.Rest.Dev;
using LunarChatSharp.Rest.Servers;
using ShadUI;
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
            var view = services.GetMainView();
            if (view == null)
                return;
            (view.DataContext as MainModel)!.CurrentImage = new InviteAppPopup { DataContext = new InviteAppPopupModel(services, id) };
            //services.Dialogs.Create(new InviteAppDialog(), new InviteAppDialogModel(services, id), "Invite " + name).WithSubmit(InviteApp).Open();
        }
        else
        {
            if (services.Socket.State.Servers.TryGetValue(id, out var server))
            {
                services.ToastManager.CreateToast("Already Joined")
                 .WithContent("Click to view the server.")
                 .DismissOnClick()
                 .WithAction("View", () =>
                 {
                     services.PageManager.SwitchServer(services, server.Server);
                 })
                 .Show();
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
