using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using LunarChatApp.Services;
using LunarChatSharp;
using LunarChatSharp.Core.Servers;
using LunarChatSharp.Rest.Servers;
using LunarChatSharp.Rest.Users;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace LunarChatApp.Views.Servers.Settings;

public partial class ServerSettingsBansModel : ViewModelBase
{
    private readonly List<BanListItem> _originalItems = new List<BanListItem>();

    private readonly Timer? _searchTimer;
    private readonly ServiceManager services;
    public ServerSettingsBansModel(ServiceManager sv)
    {
        services = sv;
        _searchTimer = new Timer(500); // 500ms debounce
        _searchTimer.Elapsed += SearchTimerElapsed;
        _searchTimer.AutoReset = false;
        services.Client.OnMemberBan += MemberBan;
        services.Client.OnMemberUnban += MemberUnban;
        services.Socket.State.CurrentServer!.OnPermissionUpdate += PermissionUpdate;
        canBan = services.State.Socket.CurrentServer!.HasPermission(services.State.Socket.CurrentServer.Member, ModPermission.BanMembers);
        PropertyChanged += OnPropertyChanged;

        foreach (var i in _originalItems) i.PropertyChanged += OnItemsChanged;
        Items = new ObservableCollection<BanListItem>(_originalItems);

        _ = Task.Run(async () =>
        {
            var Bans = await services.Rest.GetBansAsync(services.Socket.State.CurrentServer.Server.Id);
            if (Bans == null)
                return;

            Dispatcher.UIThread.Post(() =>
            {
                _originalItems.AddRange(Bans.Select(x => new BanListItem(services, x)));
                Items = new ObservableCollection<BanListItem>(_originalItems);
                Loaded = true;
            });
        });
    }

    private async Task PermissionUpdate()
    {
        CanBan = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ModPermission.BanMembers);
    }

    private async Task MemberUnban(RestServer server, RestUser user)
    {
        if (services.State.Socket.CurrentServer?.Server?.Id != server.Id)
            return;

        var item = _originalItems.FirstOrDefault(x => x.id == user.Id);
        if (item == null)
            return;

        _originalItems.Remove(item);
        Items = new ObservableCollection<BanListItem>(_originalItems);
    }

    private async Task MemberBan(RestServer server, RestMember member, RestBan ban)
    {
        if (services.State.Socket.CurrentServer?.Server?.Id != server.Id)
            return;

        var banItem = new BanListItem(services, ban);
        _originalItems.Add(banItem);
        Items.Add(banItem);
    }

    [ObservableProperty]
    private bool loaded;

    [ObservableProperty]
    private bool canBan;

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
                .Where(item => item.TargetName.Contains(SearchString, StringComparison.OrdinalIgnoreCase))
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
    private ObservableCollection<BanListItem> _items;
}
public partial class BanListItem : ObservableObject
{
    [ObservableProperty]
    private bool _isSelected;

    public string id;

    [ObservableProperty]
    private string _targetName;

    [ObservableProperty]
    private string _actionName;

    [ObservableProperty]
    private string _bannedAt;

    [ObservableProperty]
    private string reason;

    private ServiceManager services;

    public BanListItem(ServiceManager sv, RestBan x)
    {
        services = sv;
        id = x.TargetUser.Id;
        _bannedAt = x.BannedAt.ToLocalTime().ToString("d MMMM yyyy");
        _targetName = x.TargetUser.GetCurrentNameDiscrim();
        _actionName = x.ActionUser.GetCurrentNameDiscrim();
        reason = x.Reason;
    }

    [RelayCommand]
    public void CopyUserID()
    {
        services.CopyText(id);
    }

    [RelayCommand]
    public async Task UnbanUser()
    {
        try
        {
            await services.Rest.UnbanMemberAsync(services.Socket.State.CurrentServer.Server.Id, id);
        }
        catch { }
    }
}