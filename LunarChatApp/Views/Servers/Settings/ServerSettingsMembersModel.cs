using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using LunarChatApp.Services;
using LunarChatApp.Views.Dialogs.Servers;
using LunarChatSharp;
using LunarChatSharp.Core.Servers;
using LunarChatSharp.Rest.Roles;
using LunarChatSharp.Rest.Servers;
using ShadUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace LunarChatApp.Views.Servers.Settings;

public partial class ServerSettingsMembersModel : ViewModelBase
{
    private readonly List<MemberListItem> _originalItems = new List<MemberListItem>();

    private readonly Timer? _searchTimer;
    private readonly ServiceManager services;
    public ServerSettingsMembersModel(ServiceManager sv)
    {
        services = sv;
        _searchTimer = new Timer(500); // 500ms debounce
        _searchTimer.Elapsed += SearchTimerElapsed;
        _searchTimer.AutoReset = false;
        services.Client.OnMemberJoin += MemberJoin;
        services.Client.OnMemberLeft += MemberLeft;
        services.State.CurrentServer.OnPermissionUpdate += PermissionUpdate;
        PropertyChanged += OnPropertyChanged;

        Items = new ObservableCollection<MemberListItem>(_originalItems);

        _ = Task.Run(async () =>
        {
            var Members = await services.Rest.GetMembersAsync(services.State.CurrentServer.Server.Id);
            if (Members == null)
                return;

            _originalItems.AddRange(Members.Select(x => new MemberListItem(services, x)));

            Dispatcher.UIThread.Post(() =>
            {

                Items = new ObservableCollection<MemberListItem>(_originalItems);
                Loaded = true;
            });
        });
    }

    private async Task PermissionUpdate(RestServer server)
    {
        if (services.State.CurrentServer == null)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            foreach (var i in _originalItems)
            {
                i.Update(services.State.CurrentServer.Member);
            }
        });

    }

    private async Task MemberLeft(RestServer server, RestMember member)
    {
        if (services.State.CurrentServer?.Server?.Id != server.Id)
            return;

        var item = _originalItems.FirstOrDefault(x => x.id == member.Id);
        if (item == null)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            _originalItems.Remove(item);
            Items = new ObservableCollection<MemberListItem>(_originalItems);
        });

    }

    private async Task MemberJoin(RestServer server, RestMember member)
    {
        if (services.State.CurrentServer?.Server?.Id != server.Id)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            _originalItems.Add(new MemberListItem(services, member));
            Items = new ObservableCollection<MemberListItem>();
        });

    }

    [ObservableProperty]
    private bool loaded;

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SearchString))
        {
            if (SearchString.Length > 0)
            {
                IsSearching = true;
                _searchTimer?.Stop();
                _searchTimer?.Start();
            }
            else
            {
                _searchTimer?.Stop();
                IsSearching = false;
                Items.Clear();
                Items.AddRange(_originalItems);
            }
        }
    }

    private void SearchTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var filteredItems = _originalItems
                .Where(item => item.Name.Contains(SearchString, StringComparison.OrdinalIgnoreCase))
                .ToList();

            Items.Clear();
            Items.AddRange(filteredItems);

            IsSearching = false;
            _searchTimer?.Stop();
        });
    }

    [ObservableProperty]
    private string _searchString = string.Empty;

    [ObservableProperty]
    private bool _isSearching;

    [ObservableProperty]
    private ObservableCollection<MemberListItem> _items;

}
public partial class MemberListItem : ObservableObject
{
    private readonly ServiceManager services;

    public MemberListItem(ServiceManager sv, RestMember member)
    {
        services = sv;
        id = member.User.Id;
        Update(member);
    }

    public void Update(RestMember member)
    {
        if (services.State.CurrentServer == null)
            return;


        Name = member.GetCurrentNameDiscrim();
        CreatedAt = member.User.CreatedAt.ToLocalTime().ToString("d MMMM yyyy");
        JoinedAt = member.JoinedAt.ToLocalTime().ToString("d MMMM yyyy");

        bool CanManage = false;
        CanTransferOwner = !member.User.IsBot && services.State.CurrentServer.Server.OwnerId == services.Client.CurrentId && id != services.State.CurrentServer.Server.OwnerId;
        if (member.User.Id != services.State.CurrentServer.Server.OwnerId)
        {
            CanBanMember = services.State.CurrentServer.HasPermission(services.State.CurrentServer.Member, ModPermission.BanMembers);
            CanKickUser = services.State.CurrentServer.HasPermission(services.State.CurrentServer.Member, ModPermission.KickMembers);
            CanTimeoutUser = services.State.CurrentServer.HasPermission(services.State.CurrentServer.Member, ModPermission.TimeoutMembers);
        }
        if (services.Client.CurrentId == member.User.Id)
        {
            CanBanMember = false;
            CanKickUser = false;
            CanTimeoutUser = false;
        }
        BanItemText = $"Ban {Name}";
        KickItemText = $"Kick {Name}";
        if (member.Timeout.HasValue)
            TimeoutItemText = $"Remove Timeout {Name}";
        else
            TimeoutItemText = $"Timeout {Name}";

        RestRole? CurrentRole = null;
        int RolesCount = 0;
        foreach (var i in member.Roles)
        {
            if (services.State.Socket.Roles.TryGetValue(i, out var role))
            {
                if (CurrentRole != null)
                    RolesCount += 1;

                if (CurrentRole == null || role.Position > CurrentRole.Position)
                    CurrentRole = role;
            }
        }
        if (CurrentRole != null)
        {
            this.CurrentRole = CurrentRole.Name;
            ShowCurrentRole = !string.IsNullOrEmpty(this.CurrentRole);

            if (RolesCount != 0)
            {
                ShowRolesCount = true;
                this.RolesCount = $"+{RolesCount}";
            }
        }
        else
        {
            ShowCurrentRole = false;
            ShowRolesCount = false;
        }
    }

    [ObservableProperty]
    private bool _isSelected;

    public ulong id;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string createdAt;

    [ObservableProperty]
    private string joinedAt;

    [ObservableProperty]
    private string? _currentRole;

    [ObservableProperty]
    private string rolesCount;

    [ObservableProperty]
    private bool showCurrentRole;

    [ObservableProperty]
    private bool showRolesCount;

    [ObservableProperty]
    private bool canTimeoutUser;

    [ObservableProperty]
    private bool canKickUser;

    [ObservableProperty]
    private bool canBanMember;

    [ObservableProperty]
    private bool canTransferOwner;

    [ObservableProperty]
    private string banItemText;

    [ObservableProperty]
    private string kickItemText;

    [ObservableProperty]
    private string timeoutItemText;

    [RelayCommand]
    public void AddRole()
    {

    }

    [RelayCommand]
    public void CopyUserID()
    {
        services.CopyText(id.ToString());
    }

    [RelayCommand]
    public async Task BanUser()
    {
        services.Dialogs.Create(new BanMemberDialog(), new BanMemberDialogModel(), "Ban " + Name).WithSubmit(SubmitBan).Open();
    }

    public async Task SubmitBan(UserControl control)
    {
        BanMemberDialogModel? model = control.DataContext as BanMemberDialogModel;
        if (model == null || services.State.CurrentServer == null)
            return;
        CreateBanRequest req = model.CreateRequest();
        try
        {
            await services.Rest.BanMemberAsync(services.State.CurrentServer.Server.Id, id, req);
            services.ToastManager.CreateToast("Member Banned").DismissOnClick().WithDelay(3).Show();
        }
        catch { }
    }

    [RelayCommand]
    public async Task TransferOwnership()
    {
        if (services.State.CurrentServer == null)
            return;

        try
        {
            await services.Rest.EditServerAsync(services.State.CurrentServer.Server.Id, new EditServerRequest
            {
                OwnerId = id
            });
        }
        catch { }
    }

    [RelayCommand]
    public async Task KickUser()
    {
        services.Dialogs.Create(new KickMemberDialog(), new KickMemberDialogModel(), "Kick " + Name).WithSubmit(SubmitKick).Open();
    }

    public async Task SubmitKick(UserControl control)
    {
        KickMemberDialogModel? model = control.DataContext as KickMemberDialogModel;
        if (model == null || services.State.CurrentServer == null)
            return;

        try
        {
            await services.Rest.KickMemberAsync(services.State.CurrentServer.Server.Id, id, new LunarChatSharp.Rest.ReasonRequest
            {
                Reason = model.Reason
            });
            services.ToastManager.CreateToast("Member Kicked").DismissOnClick().WithDelay(3).Show();
        }
        catch { }
    }

    [RelayCommand]
    public async Task TimeoutUser()
    {
        if (services.State.CurrentServer == null)
            return;

        try
        {
            if (TimeoutItemText.StartsWith("Remove"))
            {
                await services.Rest.TimeoutMemberAsync(services.State.CurrentServer.Server.Id, id, null, null);
                services.ToastManager.CreateToast("Timeout Removed").DismissOnClick().WithDelay(3).Show();
            }
            else
                services.Dialogs.Create(new TimeoutMemberDialog(), new TimeoutMemberDialogModel(), "Timeout " + Name).WithSubmit(SubmitTimeout).Open();
        }
        catch { }
        {

        }
    }

    public async Task SubmitTimeout(UserControl control)
    {
        TimeoutMemberDialogModel? model = control.DataContext as TimeoutMemberDialogModel;
        if (model == null || services.State.CurrentServer == null)
            return;

        try
        {
            await services.Rest.TimeoutMemberAsync(services.State.CurrentServer.Server.Id, id, model.GetTimeout(), model.Reason);
            services.ToastManager.CreateToast("Member Timed-out").DismissOnClick().WithDelay(3).Show();
        }
        catch { }
    }
}
