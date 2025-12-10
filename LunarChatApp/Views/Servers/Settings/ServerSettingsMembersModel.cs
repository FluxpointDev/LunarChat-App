using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using LunarChatApp.Services;
using LunarChatSharp;
using LunarChatSharp.Core.Servers;
using LunarChatSharp.Rest.Roles;
using LunarChatSharp.Rest.Servers;
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
        services.Socket.State.CurrentServer.OnPermissionUpdate += PermissionUpdate;
        PropertyChanged += OnPropertyChanged;

        foreach (var i in _originalItems) i.PropertyChanged += OnItemsChanged;
        Items = new ObservableCollection<MemberListItem>(_originalItems);

        _ = Task.Run(async () =>
        {
            var Members = await services.Rest.GetMembersAsync(services.Socket.State.CurrentServer.Server.Id);
            if (Members == null)
                return;

            Dispatcher.UIThread.Post(() =>
            {
                _originalItems.AddRange(Members.Select(x => new MemberListItem(services, x)));
                Items = new ObservableCollection<MemberListItem>(_originalItems);
                Loaded = true;
            });
        });
    }

    private async Task PermissionUpdate()
    {
        foreach (var i in _originalItems)
        {
            i.Update(services.State.Socket.CurrentServer.Member);
        }
    }

    private async Task MemberLeft(RestServer server, RestMember member)
    {
        if (services.State.Socket.CurrentServer?.Server?.Id != server.Id)
            return;

        var item = _originalItems.FirstOrDefault(x => x.id == member.Id);
        if (item == null)
            return;

        _originalItems.Remove(item);
        Items = new ObservableCollection<MemberListItem>(_originalItems);
    }

    private async Task MemberJoin(RestServer server, RestMember member)
    {
        if (services.State.Socket.CurrentServer?.Server?.Id != server.Id)
            return;

        _originalItems.Add(new MemberListItem(services, member));
        Items = new ObservableCollection<MemberListItem>();
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
                UpdateTotal();
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
            UpdateTotal();
        });
    }

    private void OnItemsChanged(object? sender, PropertyChangedEventArgs e)
    {
        var selectedAll = Items.All(item => item.IsSelected);
        var notSelectedCount = Items.Count(item => !item.IsSelected);

        if (selectedAll)
        {
            SelectAll = true;
        }
        else if (notSelectedCount == Items.Count)
        {
            SelectAll = false;
        }
        else
        {
            SelectAll = null;
        }

        UpdateTotal();
    }

    private void UpdateTotal()
    {
        TotalCount = Items.Count;
        SelectedCount = Items.Count(item => item.IsSelected);
    }

    [ObservableProperty]
    private string _searchString = string.Empty;

    [ObservableProperty]
    private bool _isSearching;

    [ObservableProperty]
    private bool? _selectAll = false;

    [RelayCommand]
    private void ToggleSelection(bool? selectAll)
    {
        foreach (var item in Items)
        {
            item.IsSelected = selectAll ?? false;
        }
    }

    [ObservableProperty]
    private int _selectedCount;

    [ObservableProperty]
    private int _totalCount;

    [ObservableProperty]
    private ObservableCollection<MemberListItem> _items;

}
public partial class MemberListItem : ObservableObject
{
    private ServiceManager services;

    public MemberListItem(ServiceManager sv, RestMember member)
    {
        services = sv;
        id = member.User.Id;
        Update(member);
    }

    public void Update(RestMember member)
    {
        Name = member.GetCurrentNameDiscrim();
        CreatedAt = member.User.CreatedAt.ToLocalTime().ToString("d MMMM yyyy");
        JoinedAt = member.JoinedAt.Value.ToLocalTime().ToString("d MMMM yyyy");

        bool CanManage = false;
        if (member.User.Id != services.State.Socket.CurrentServer.Server.OwnerId)
        {
            CanBanMember = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ModPermission.BanMembers);
            CanKickUser = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ModPermission.KickMembers);
            CanTimeoutUser = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ModPermission.TimeoutMembers);
        }
        if (services.Client.CurrentId == member.User.Id)
        {
            CanBanMember = false;
            CanKickUser = false;
            CanTimeoutUser = false;
        }
        BanItemText = $"Ban {_name}";
        KickItemText = $"Kick {_name}";
        if (member.Timeout.HasValue)
            TimeoutItemText = $"Remove Timeout {_name}";
        else
            TimeoutItemText = $"Timeout {_name}";

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

    public string id;

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
        services.CopyText(id);
    }

    [RelayCommand]
    public async Task BanUser()
    {
        try
        {
            await services.Rest.BanMemberAsync(services.Socket.State.CurrentServer.Server.Id, id);
        }
        catch { }
    }

    [RelayCommand]
    public async Task KickUser()
    {
        try
        {
            await services.Rest.KickMemberAsync(services.Socket.State.CurrentServer.Server.Id, id);
        }
        catch { }
    }

    [RelayCommand]
    public async Task TimeoutUser()
    {
        try
        {
            if (timeoutItemText.StartsWith("Remove"))
                await services.Rest.TimeoutMemberAsync(services.Socket.State.CurrentServer.Server.Id, id, null);
            else
                await services.Rest.TimeoutMemberAsync(services.Socket.State.CurrentServer.Server.Id, id, DateTime.UtcNow.AddSeconds(60));
        }
        catch { }
    }
}
