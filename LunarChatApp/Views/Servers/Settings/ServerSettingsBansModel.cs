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
        services.State.CurrentServer!.OnPermissionUpdate += PermissionUpdate;
        canBan = services.State.CurrentServer!.HasPermission(services.State.CurrentServer.Member, ModPermission.BanMembers);
        PropertyChanged += OnPropertyChanged;

        Items = new ObservableCollection<BanListItem>(_originalItems);

        _ = Task.Run(async () =>
        {
            var Bans = await services.Rest.GetBansAsync(services.State.CurrentServer.Server.Id);
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

    private async Task PermissionUpdate(RestServer server)
    {
        if (services.State.CurrentServer == null)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            CanBan = services.State.CurrentServer.HasPermission(services.State.CurrentServer.Member, ModPermission.BanMembers);
        });

    }

    private async Task MemberUnban(RestServer server, RestUser user)
    {
        if (services.State.CurrentServer?.Server?.Id != server.Id)
            return;

        var item = _originalItems.FirstOrDefault(x => x.id == user.Id);
        if (item == null)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            _originalItems.Remove(item);
            Items = new ObservableCollection<BanListItem>(_originalItems);
        });

    }

    private async Task MemberBan(RestServer server, RestMember member, RestBan ban)
    {
        if (services.State.CurrentServer?.Server?.Id != server.Id)
            return;

        var banItem = new BanListItem(services, ban);
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            _originalItems.Add(banItem);
            Items.Add(banItem);
        });

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
        });
    }

    [ObservableProperty]
    private string _searchString = string.Empty;

    [ObservableProperty]
    private bool _isSearching;

    [ObservableProperty]
    private ObservableCollection<BanListItem> _items;
}
public partial class BanListItem : ObservableObject
{
    [ObservableProperty]
    private bool _isSelected;

    public ulong id;

    [ObservableProperty]
    private string _targetName;

    [ObservableProperty]
    private string _actionName;

    [ObservableProperty]
    private string _bannedAt;

    [ObservableProperty]
    private string? reason;

    private readonly ServiceManager services;

    public BanListItem(ServiceManager sv, RestBan x)
    {
        services = sv;
        if (x.TargetUser != null)
        {
            id = x.TargetUser.Id;
            _targetName = x.TargetUser.GetCurrentNameDiscrim();
        }

        if (x.ActionUser != null)
            _actionName = x.ActionUser.GetCurrentNameDiscrim();

        _bannedAt = x.BannedAt.ToLocalTime().ToString("d MMMM yyyy");
        reason = x.Reason;
    }

    [RelayCommand]
    public void CopyUserID()
    {
        services.CopyText(id.ToString());
    }

    [RelayCommand]
    public async Task UnbanUser()
    {
        if (services.State.CurrentServer == null)
            return;

        try
        {
            await services.Rest.UnbanMemberAsync(services.State.CurrentServer.Server.Id, id);
        }
        catch { }
    }
}