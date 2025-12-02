using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.ViewModels.Dialogs;
using LunarChatApp.Views;
using LunarChatSharp.Rest.Channels;
using LunarChatSharp.Rest.Servers;
using System;
using System.Threading.Tasks;

namespace LunarChatApp.ViewModels.Servers;

public partial class ServerHeaderModel : ViewModelBase
{
    private ServiceManager services;
    public ServerHeaderModel(ServiceManager sv, RestServer s)
    {
        services = sv;
        Name = s.Name;
        isOwner = sv.State.Socket.CurrentId == s.OwnerId;
    }

    [ObservableProperty]
    private string? name;

    [ObservableProperty]
    private bool isOwner;

    [RelayCommand]
    public async Task CreateChannel()
    {
        services.Dialogs.Create(new CreateChannelDialog(), new CreateChannelDialogModel(services), "Create Channel").WithSubmit(SubmitChannel).Open();
    }

    public async Task SubmitChannel(UserControl control)
    {
        CreateChannelDialogModel model = (CreateChannelDialogModel)control.DataContext!;
        string Id = Guid.NewGuid().ToString();
        await services.Rest.PostAsync("/channels", new CreateChannelRequest
        {
            Name = model.Name,
            Topic = model.Topic,
            ServerId = services.State.Socket.CurrentServer.Server.Id,
            Type = model.Type,
        });
    }

    [RelayCommand]
    public void CopyServerID()
    {
        services.CopyText(services.State.Socket.CurrentServer?.Server.Id);
    }

    [RelayCommand]
    public void OpenServerSettings()
    {
        services.PageManager.OnSwitchPage(new ServerSettings
        {
            DataContext = new ServerSettingsModel(services.PageManager, services.State, services)
        });
    }

    [RelayCommand]
    public void OpenReportServer()
    {
        services.Dialogs.Create(new ReportServerDialog(), new ReportServerDialogModel(), "Report Server: " + services.State.Socket.CurrentServer.Server.Name).Open();
    }


    [RelayCommand]
    public async Task LeaveServer()
    {
        await services.Rest.DeleteAsync($"/servers/{services.State.Socket.CurrentServer?.Server.Id}/members/{services.State.Socket.CurrentId}");
        services.State.Socket.OnSelectServer?.Invoke(null);
    }
}
