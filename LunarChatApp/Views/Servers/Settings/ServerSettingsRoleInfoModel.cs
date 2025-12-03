using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatSharp;
using LunarChatSharp.Rest.Roles;
using System;
using System.Threading.Tasks;

namespace LunarChatApp.Views.Servers.Settings;

public partial class ServerSettingsRoleInfoModel : ViewModelBase
{
    private ServiceManager services;
    private RestRole role;
    private Action backAction;
    public ServerSettingsRoleInfoModel(ServiceManager sv, RestRole r, Action back)
    {
        services = sv;
        backAction = back;
        role = r;
        _roleName = role.Name;
        _color = role.Color;
    }

    [ObservableProperty]
    private string? _roleName;

    [ObservableProperty]
    private string? _color;

    [RelayCommand]
    public void Back()
    {
        backAction.Invoke();
    }

    [RelayCommand]
    public void ClearColor()
    {
        Color = null;
    }

    [RelayCommand]
    public async Task Save()
    {
        EditRoleRequest req = new EditRoleRequest();
        if (RoleName != role.Name)
            req.Name = RoleName;

        if (Color != role.Color)
            req.Color = Color ?? "";

        await services.Rest.EditRoleAsync(services.State.Socket.CurrentServer?.Server.Id, role.Id, req);
    }
}

public partial class RolePermissionsModel : ViewModelBase
{
    public void Update(RestPermissions permissions)
    {

    }

    [ObservableProperty]
    private bool _createInvites;

    [ObservableProperty]
    private bool _changeNicknames;

    [ObservableProperty]
    private bool _createExpressions;

    [ObservableProperty]
    private bool _manageExpressions;

    [ObservableProperty]
    private bool _manageServer;

    [ObservableProperty]
    private bool _administrator;

    [ObservableProperty]
    private bool _kickMembers;

    [ObservableProperty]
    private bool _banMembers;

    [ObservableProperty]
    private bool _timeoutMembers;

    [ObservableProperty]
    private bool _viewAuditLogs;

    [ObservableProperty]
    private bool _manageRoles;

    [ObservableProperty]
    private bool _manageNicknames;

    [ObservableProperty]
    private bool _manageApprovals;

    [ObservableProperty]
    private bool _manageAppeals;

    [ObservableProperty]
    private bool _useModView;

    [ObservableProperty]
    private bool _viewChannels;

    [ObservableProperty]
    private bool _readMessageHistory;

    [ObservableProperty]
    private bool _sendMessages;

    [ObservableProperty]
    private bool _embedLinks;

    [ObservableProperty]
    private bool _attachFiles;

    [ObservableProperty]
    private bool _addReactions;

    [ObservableProperty]
    private bool _sendPolls;

    [ObservableProperty]
    private bool _sendVoiceMessages;

    [ObservableProperty]
    private bool _useExternalEmojis;

    [ObservableProperty]
    private bool _useAppCommands;

    [ObservableProperty]
    private bool _mentionEveryone;

    [ObservableProperty]
    private bool _mentionRoles;

    [ObservableProperty]
    private bool _manageMessages;

    [ObservableProperty]
    private bool _managePins;

    [ObservableProperty]
    private bool _bypassSlowmode;

    [ObservableProperty]
    private bool _manageChannels;

    [ObservableProperty]
    private bool _manageWebhooks;

    [ObservableProperty]
    private bool _connect;

    [ObservableProperty]
    private bool _speak;

    [ObservableProperty]
    private bool _video;

    [ObservableProperty]
    private bool _useVoiceActivity;

    [ObservableProperty]
    private bool _muteMembers;

    [ObservableProperty]
    private bool _deafenMembers;

    [ObservableProperty]
    private bool _moveMembers;

    [ObservableProperty]
    private bool _setVoiceStatus;

    [ObservableProperty]
    private bool _requestToSpeak;
}
