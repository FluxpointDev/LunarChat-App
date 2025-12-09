using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.ViewModels.Dialogs;
using LunarChatApp.Views;
using LunarChatSharp;
using LunarChatSharp.Core.Servers;
using LunarChatSharp.Rest.Channels;
using LunarChatSharp.Rest.Servers;
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
        services.State.Socket.CurrentServer.OnPermissionUpdate += Update;
    }

    public async Task Update()
    {
        UpdatePermissions();
    }

    [ObservableProperty]
    private bool canManageChannels;

    public void UpdatePermissions()
    {
        CanManageChannels = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ChannelPermission.ManageChannel);
        bool CanView = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ServerPermission.CreateExpressions);
        if (!CanView)
            CanView = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ServerPermission.ManageExpressions);
        if (!CanView)
            CanView = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ServerPermission.ManageServer);
        if (!CanView)
            CanView = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ServerPermission.ManageApps);
        if (!CanView)
            CanView = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ModPermission.BanMembers);
        if (!CanView)
            CanView = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ModPermission.ViewAuditLogs);
        if (!CanView)
            CanView = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ModPermission.AssignRoles);
        if (!CanView)
            CanView = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ModPermission.ManageRoles);
        if (!CanView)
            CanView = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ModPermission.ManageApprovals);
        if (!CanView)
            CanView = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ModPermission.ManageAppeals);
        if (!CanView)
            CanView = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ChannelPermission.ManageWebhooks);

        CanViewSettings = CanView;
    }

    [ObservableProperty]
    private string? name;

    [ObservableProperty]
    private bool isOwner;

    [ObservableProperty]
    private bool _canViewSettings;

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
            await services.Rest.RemoveMemberAsync(services.State.Socket.CurrentServer?.Server.Id, services.Client.CurrentId);
            services.Client.OnSelectServer?.Invoke(null);
        }
        catch { }

    }
}
