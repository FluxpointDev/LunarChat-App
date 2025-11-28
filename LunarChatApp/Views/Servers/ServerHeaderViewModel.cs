using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Shared.Core.Servers;
using LunarChatApp.Shared.Rest.Channels;
using LunarChatApp.ViewModels.Dialogs;
using LunarChatApp.Views;
using System;
using System.Threading.Tasks;

namespace LunarChatApp.ViewModels.Servers;

public partial class ServerHeaderViewModel : ViewModelBase
{
    private ServiceManager Services;
    public ServerHeaderViewModel(ServiceManager sv, Server s)
    {
        Services = sv;
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
        Services.Dialogs.Create(new CreateChannelDialog(), new CreateChannelDialogModel(Services), "Create Channel").WithSubmit(SubmitChannel).Open();
    }

    public async Task SubmitChannel(UserControl control)
    {
        CreateChannelDialogModel model = (CreateChannelDialogModel)control.DataContext!;
        string Id = Guid.NewGuid().ToString();
        await Services.Rest.PostAsync("/channels", new CreateChannelRequest
        {
            name = model.Name,
            topic = model.Topic,
            serverId = Services.State.Socket.CurrentServer.Server.Id,
            type = model.Type,
        });
    }

    [RelayCommand]
    public void OpenServerSettings()
    {
        Services.PageManager.OnSwitchPage(new ServerSettings
        {
            DataContext = new ServerSettingsModel(Services.PageManager, Services.State, Services)
        });
    }

    [RelayCommand]
    public void OpenReportServer()
    {
        Services.Dialogs.Create(new ReportServerDialog(), new ReportServerDialogModel(), "Report Server: " + Services.State.Socket.CurrentServer.Server.Name).Open();
    }


    [RelayCommand]
    public void LeaveServer()
    {

    }
}
