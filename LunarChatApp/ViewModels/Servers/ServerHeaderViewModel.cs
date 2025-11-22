using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Shared.Core.Channels;
using LunarChatApp.ViewModels.Dialogs;
using System;
using System.Threading.Tasks;

namespace LunarChatApp.ViewModels.Servers;

public partial class ServerHeaderViewModel(ServiceManager Services) : ViewModelBase
{
    [RelayCommand]
    public async Task CreateChannel()
    {
        //Services.Dialogs.Create(new CreateChannelDialogModel(), "Create Channel").WithSubmit(SubmitChannel).Open();
    }

    public void SubmitChannel(UserControl control)
    {
        CreateChannelDialogModel model = (CreateChannelDialogModel)control.DataContext!;
        string Id = Guid.NewGuid().ToString();
        Channel chan = new Channel
        {
            Id = Id,
            Name = model.Name,
            Type = model.Type
        };
        Services.State.CurrentServer.Channels.Add(Id, chan);
        Services.State.CurrentServer.OnChannelUpdated.Invoke(chan);
    }

    [RelayCommand]
    public void OpenServerSettings()
    {
        Services.PageManager.OnSwitchPage(new ServerSettings
        {
            DataContext = new ServerSettingsModel(Services.PageManager, Services.State)
        });
    }

    [RelayCommand]
    public void OpenReportServer()
    {
        Services.Dialogs.Create(new ReportServerDialogModel(), "Report Server: " + Services.State.CurrentServer.Server.Name).Open();
    }


    [RelayCommand]
    public void LeaveServer()
    {

    }
}
