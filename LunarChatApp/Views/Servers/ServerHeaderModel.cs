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
    private ServiceManager Services;
    public ServerHeaderModel(ServiceManager sv, RestServer s)
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
            Name = model.Name,
            Topic = model.Topic,
            ServerId = Services.State.Socket.CurrentServer.Server.Id,
            Type = model.Type,
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
