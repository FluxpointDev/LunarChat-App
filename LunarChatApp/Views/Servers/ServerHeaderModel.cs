using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.ViewModels.Dialogs;
using LunarChatApp.Views;
using LunarChatApp.Views.Dialogs;
using LunarChatSharp;
using LunarChatSharp.Core.Servers;
using LunarChatSharp.Rest.Channels;
using LunarChatSharp.Rest.Servers;
using LunarChatSharp.Websocket.Events.Servers;
using System.Threading.Tasks;

namespace LunarChatApp.ViewModels.Servers;

public partial class ServerHeaderModel : ViewModelBase
{
    private ServiceManager services;
    public ServerHeaderModel(ServiceManager sv, RestServer s)
    {
        services = sv;
        Name = s.Name;
        isOwner = sv.Client.CurrentId == s.OwnerId;
        UpdatePermissions();
        services.Client.OnServerUpdate += ServerUpdate;
        services.State.Socket.CurrentServer.OnPermissionUpdate += Update;
    }

    private async Task ServerUpdate(RestServer server, ServerUpdateEvent ev)
    {
        if (server.Id != services.State.Socket.CurrentServer?.Server.Id)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (!string.IsNullOrEmpty(ev.Changed.Name))
                Name = ev.Changed.Name;
        });

    }

    public async Task Update()
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            UpdatePermissions();
        });

    }

    [ObservableProperty]
    private bool canManageChannels;

    public void UpdatePermissions()
    {
        CanManageChannels = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ChannelPermission.ManageChannel);
        CanChangeNickname = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ServerPermission.ChangeNickname) || services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ModPermission.ManageNicknames);
        CanViewSettings = services.State.Socket.CurrentServer.CanManageServer(services.State.Socket.CurrentServer.Member);
    }

    [ObservableProperty]
    private string? name;

    [ObservableProperty]
    private bool isOwner;

    [ObservableProperty]
    private bool _canViewSettings;

    [ObservableProperty]
    private bool canChangeNickname;

    [RelayCommand]
    public async Task CreateChannel()
    {
        services.Dialogs.Create(new CreateChannelDialog(), new CreateChannelDialogModel(services), "Create Channel").WithSubmit(SubmitChannel).Open();
    }

    public async Task SubmitChannel(UserControl control)
    {
        try
        {
            CreateChannelDialogModel model = (CreateChannelDialogModel)control.DataContext!;
            await services.Rest.CreateChannelAsync(new CreateChannelRequest
            {
                Name = model.Name,
                Topic = model.Topic,
                ServerId = services.State.Socket.CurrentServer.Server.Id,
                Type = model.Type,
            });
        }
        catch { }

    }

    [RelayCommand]
    public void CopyServerID()
    {
        services.CopyText(services.State.Socket.CurrentServer?.Server.Id);
    }

    [RelayCommand]
    public void ChangeNickname()
    {
        services.Dialogs.Create(new CreateNameDialog(), new CreateNameDialogModel { Name = services.State.Socket.CurrentServer?.Member.Nickname }, "Change Nickname").WithSubmit(SubmitNickname).Open();
    }

    public async Task SubmitNickname(UserControl control)
    {
        CreateNameDialogModel? model = control.DataContext as CreateNameDialogModel;
        if (model == null)
            return;

        try
        {
            await services.Rest.EditMemberAsync(services.State.Socket.CurrentServer.Server.Id, services.State.Socket.CurrentServer.Member.Id, new EditMemberRequest
            {
                Nickname = model.Name ?? ""
            });
        }
        catch { }
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
        try
        {
            await services.Rest.LeaveServerAsync(services.State.Socket.CurrentServer?.Server.Id);
            services.Client.OnSelectServer?.Invoke(null);
        }
        catch { }

    }
}
