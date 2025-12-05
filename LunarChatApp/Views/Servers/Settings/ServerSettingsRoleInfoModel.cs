using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatSharp;
using LunarChatSharp.Core.Servers;
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
        _allowEdit = role.Id != "0";
        Permissions = new RolePermissionsModel(role);
    }

    [ObservableProperty]
    private bool _allowEdit;

    [ObservableProperty]
    private string? _roleName;

    [ObservableProperty]
    private string? _color;

    public RolePermissionsModel Permissions { get; set; }

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

        req.Permissions = Permissions.GetPermissions();

        try
        {
            if (role.Id == "0")
                await services.Rest.EditServerAsync(services.State.Socket.CurrentServer?.Server.Id, new LunarChatSharp.Rest.Servers.EditServerRequest
                {
                    DefaultPermissions = req.Permissions,
                });
            else
            {
                RestRole getRole = await services.Rest.EditRoleAsync(services.State.Socket.CurrentServer?.Server.Id, role.Id, req);
                RoleName = getRole.Name;
                role.Name = getRole.Name;
            }
        }
        catch { }

    }
}

public partial class RolePermissionsModel : ViewModelBase
{
    public RolePermissionsModel(RestRole role)
    {
        Update(role.Permissions);
    }

    public RestPermissions GetPermissions()
    {
        var permissions = new RestPermissions();
        permissions.SetValue(ChangeNickname, ServerPermission.ChangeNickname);
        permissions.SetValue(CreateExpressions, ServerPermission.CreateExpressions);
        permissions.SetValue(ManageExpressions, ServerPermission.ManageExpressions);
        permissions.SetValue(ManageServer, ServerPermission.ManageServer);
        permissions.SetValue(ManageApps, ServerPermission.ManageApps);
        permissions.SetValue(Administrator, ServerPermission.Administrator);

        permissions.SetValue(KickMembers, ModPermission.KickMembers);
        permissions.SetValue(BanMembers, ModPermission.BanMembers);
        permissions.SetValue(TimeoutMembers, ModPermission.TimeoutMembers);
        permissions.SetValue(ViewAuditLogs, ModPermission.ViewAuditLogs);
        permissions.SetValue(AssignRoles, ModPermission.AssignRoles);
        permissions.SetValue(ManageRoles, ModPermission.ManageRoles);
        permissions.SetValue(ManageRolePermissions, ModPermission.ManageRolePermissions);
        permissions.SetValue(ManageNicknames, ModPermission.ManageNicknames);
        permissions.SetValue(ManageApprovals, ModPermission.ManageApprovals);
        permissions.SetValue(ManageAppeals, ModPermission.ManageAppeals);
        permissions.SetValue(UseModView, ModPermission.UseModView);

        permissions.SetValue(CreateInvites, ChannelPermission.CreateInvites);
        permissions.SetValue(ViewChannels, ChannelPermission.ViewChannel);
        permissions.SetValue(ReadMessageHistory, ChannelPermission.ReadMessageHistory);
        permissions.SetValue(SendMessages, ChannelPermission.SendMessages);
        permissions.SetValue(EmbedLinks, ChannelPermission.EmbedLinks);
        permissions.SetValue(AttachFiles, ChannelPermission.AttachFiles);
        permissions.SetValue(AddReactions, ChannelPermission.AddReactions);
        permissions.SetValue(SendPolls, ChannelPermission.SendPolls);
        permissions.SetValue(UseExternalEmojis, ChannelPermission.UseExternalEmojis);
        permissions.SetValue(UseAppCommands, ChannelPermission.UseAppCommands);
        permissions.SetValue(MentionEveryone, ChannelPermission.MentionEveryone);
        permissions.SetValue(ManageMessages, ChannelPermission.ManageMessages);
        permissions.SetValue(ManagePins, ChannelPermission.ManagePins);
        permissions.SetValue(BypassSlowmode, ChannelPermission.BypassSlowmode);
        permissions.SetValue(ManageChannels, ChannelPermission.ManageChannel);
        permissions.SetValue(ManageChannelPermissions, ChannelPermission.ManageChannelPermissions);
        permissions.SetValue(ManageWebhooks, ChannelPermission.ManageWebhooks);

        permissions.SetValue(Connect, VoicePermission.Connect);
        permissions.SetValue(Speak, VoicePermission.Speak);
        permissions.SetValue(Video, VoicePermission.Video);
        permissions.SetValue(UseVoiceActivity, VoicePermission.UseVoiceActivity);
        permissions.SetValue(MuteMembers, VoicePermission.MuteMembers);
        permissions.SetValue(DeafenMembers, VoicePermission.DeafenMembers);
        permissions.SetValue(MoveMembers, VoicePermission.MoveMembers);
        permissions.SetValue(SetVoiceStatus, VoicePermission.SetVoiceStatus);
        permissions.SetValue(RequestToSpeak, VoicePermission.RequestToSpeak);
        return permissions;
    }



    public void Update(RestPermissions permissions)
    {
        #region Server
        ChangeNickname = permissions.ServerPermissions.HasFlag(ServerPermission.ChangeNickname);
        CreateExpressions = permissions.ServerPermissions.HasFlag(ServerPermission.CreateExpressions);
        ManageExpressions = permissions.ServerPermissions.HasFlag(ServerPermission.ManageExpressions);
        ManageServer = permissions.ServerPermissions.HasFlag(ServerPermission.ManageServer);
        ManageApps = permissions.ServerPermissions.HasFlag(ServerPermission.ManageApps);
        Administrator = permissions.ServerPermissions.HasFlag(ServerPermission.Administrator);
        #endregion

        #region Mod
        KickMembers = permissions.ModPermissions.HasFlag(ModPermission.KickMembers);
        BanMembers = permissions.ModPermissions.HasFlag(ModPermission.BanMembers);
        TimeoutMembers = permissions.ModPermissions.HasFlag(ModPermission.TimeoutMembers);
        ViewAuditLogs = permissions.ModPermissions.HasFlag(ModPermission.ViewAuditLogs);
        AssignRoles = permissions.ModPermissions.HasFlag(ModPermission.AssignRoles);
        ManageRoles = permissions.ModPermissions.HasFlag(ModPermission.ManageRoles);
        ManageRolePermissions = permissions.ModPermissions.HasFlag(ModPermission.ManageRolePermissions);
        ManageNicknames = permissions.ModPermissions.HasFlag(ModPermission.ManageNicknames);
        ManageApprovals = permissions.ModPermissions.HasFlag(ModPermission.ManageApprovals);
        ManageAppeals = permissions.ModPermissions.HasFlag(ModPermission.ManageAppeals);
        UseModView = permissions.ModPermissions.HasFlag(ModPermission.UseModView);
        #endregion

        #region Channel
        CreateInvites = permissions.ChannelPermissions.HasFlag(ChannelPermission.CreateInvites);
        ViewChannels = permissions.ChannelPermissions.HasFlag(ChannelPermission.ViewChannel);
        ReadMessageHistory = permissions.ChannelPermissions.HasFlag(ChannelPermission.ReadMessageHistory);
        SendMessages = permissions.ChannelPermissions.HasFlag(ChannelPermission.SendMessages);
        EmbedLinks = permissions.ChannelPermissions.HasFlag(ChannelPermission.EmbedLinks);
        AttachFiles = permissions.ChannelPermissions.HasFlag(ChannelPermission.AttachFiles);
        AddReactions = permissions.ChannelPermissions.HasFlag(ChannelPermission.AddReactions);
        SendPolls = permissions.ChannelPermissions.HasFlag(ChannelPermission.SendPolls);
        UseExternalEmojis = permissions.ChannelPermissions.HasFlag(ChannelPermission.UseExternalEmojis);
        UseAppCommands = permissions.ChannelPermissions.HasFlag(ChannelPermission.UseAppCommands);
        MentionEveryone = permissions.ChannelPermissions.HasFlag(ChannelPermission.MentionEveryone);
        ManageMessages = permissions.ChannelPermissions.HasFlag(ChannelPermission.ManageMessages);
        ManagePins = permissions.ChannelPermissions.HasFlag(ChannelPermission.ManagePins);
        BypassSlowmode = permissions.ChannelPermissions.HasFlag(ChannelPermission.BypassSlowmode);
        ManageChannels = permissions.ChannelPermissions.HasFlag(ChannelPermission.ManageChannel);
        ManageChannelPermissions = permissions.ChannelPermissions.HasFlag(ChannelPermission.ManageChannelPermissions);
        ManageWebhooks = permissions.ChannelPermissions.HasFlag(ChannelPermission.ManageWebhooks);
        #endregion

        #region Voice
        Connect = permissions.VoicePermissions.HasFlag(VoicePermission.Connect);
        Speak = permissions.VoicePermissions.HasFlag(VoicePermission.Speak);
        Video = permissions.VoicePermissions.HasFlag(VoicePermission.Video);
        UseVoiceActivity = permissions.VoicePermissions.HasFlag(VoicePermission.UseVoiceActivity);
        MuteMembers = permissions.VoicePermissions.HasFlag(VoicePermission.MuteMembers);
        DeafenMembers = permissions.VoicePermissions.HasFlag(VoicePermission.DeafenMembers);
        MoveMembers = permissions.VoicePermissions.HasFlag(VoicePermission.MoveMembers);
        SetVoiceStatus = permissions.VoicePermissions.HasFlag(VoicePermission.SetVoiceStatus);
        RequestToSpeak = permissions.VoicePermissions.HasFlag(VoicePermission.RequestToSpeak);
        #endregion
    }

    [ObservableProperty]
    private bool _changeNickname;

    [ObservableProperty]
    private bool _createExpressions;

    [ObservableProperty]
    private bool _manageExpressions;

    [ObservableProperty]
    private bool _manageServer;

    [ObservableProperty]
    private bool _manageApps;

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
    private bool _assignRoles;

    [ObservableProperty]
    private bool _manageRoles;

    [ObservableProperty]
    private bool _manageRolePermissions;

    [ObservableProperty]
    private bool _manageNicknames;

    [ObservableProperty]
    private bool _manageApprovals;

    [ObservableProperty]
    private bool _manageAppeals;

    [ObservableProperty]
    private bool _useModView;

    [ObservableProperty]
    private bool _createInvites;

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
    private bool _manageMessages;

    [ObservableProperty]
    private bool _managePins;

    [ObservableProperty]
    private bool _bypassSlowmode;

    [ObservableProperty]
    private bool _manageChannels;

    [ObservableProperty]
    private bool _manageChannelPermissions;

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
