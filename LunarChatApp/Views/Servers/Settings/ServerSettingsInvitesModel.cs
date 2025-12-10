using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using LunarChatApp.Services;
using LunarChatSharp;
using LunarChatSharp.Core.Servers;
using LunarChatSharp.Rest.Channels;
using LunarChatSharp.Rest.Servers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace LunarChatApp.Views.Servers.Settings;

public partial class ServerSettingsInvitesModel : ViewModelBase
{
    private readonly List<InviteListItem> _originalItems = new List<InviteListItem>();

    private readonly Timer? _searchTimer;
    private readonly ServiceManager services;
    public ServerSettingsInvitesModel(ServiceManager sv)
    {
        services = sv;
        _searchTimer = new Timer(500); // 500ms debounce
        _searchTimer.Elapsed += SearchTimerElapsed;
        _searchTimer.AutoReset = false;
        services.Socket.State.CurrentServer.OnPermissionUpdate += PermissionUpdate;
        services.Client.OnInviteCreate += InviteCreate;
        services.Client.OnInviteDelete += InviteDelete;
        canCreate = services.Socket.State.CurrentServer.HasPermission(services.Socket.State.CurrentServer.Member, ChannelPermission.CreateInvites);
        canManage = services.Socket.State.CurrentServer.HasPermission(services.Socket.State.CurrentServer.Member, ChannelPermission.ManageInvites);
        PropertyChanged += OnPropertyChanged;

        foreach (var i in _originalItems) i.PropertyChanged += OnItemsChanged;
        Items = new ObservableCollection<InviteListItem>(_originalItems);

        _ = Task.Run(async () =>
        {
            var Invites = await services.Rest.GetServerInvitesAsync(services.Socket.State.CurrentServer.Server.Id);
            if (Invites == null)
                return;

            Dispatcher.UIThread.Post(() =>
            {
                _originalItems.AddRange(Invites.Select(x => new InviteListItem
                {
                    services = services,
                    Code = x.Code,
                    Inviter = x.Inviter.GetCurrentNameDiscrim(),
                    Uses = x.Uses,
                    channelId = x.ChannelId,
                    Channel = services.Socket.State.Channels.TryGetValue(x.ChannelId, out var channel) ? "#" + channel.Name : "#invalid-channel"
                }));
                Items = new ObservableCollection<InviteListItem>(_originalItems);
                Loaded = true;
            });
        });
    }

    [ObservableProperty]
    private bool loaded;

    private async Task InviteDelete(RestServer server, string arg2)
    {
        if (server.Id != services.Socket.State.CurrentServer.Server.Id)
            return;

        var item = _originalItems.FirstOrDefault(x => x.Code == arg2);
        if (item == null)
            return;

        _originalItems.Remove(item);
        Items = new ObservableCollection<InviteListItem>(_originalItems);
    }

    private async Task InviteCreate(RestServer server, RestInvite invite)
    {
        if (server.Id != services.Socket.State.CurrentServer.Server.Id)
            return;

        _originalItems.Add(new InviteListItem
        {
            services = services,
            Inviter = invite.Inviter.GetCurrentNameDiscrim(),
            Code = invite.Code,
            Uses = invite.Uses,
            channelId = invite.ChannelId,
            Channel = services.Socket.State.Channels.TryGetValue(invite.ChannelId, out var channel) ? "#" + channel.Name : "#invalid-channel"
        });
        Items = new ObservableCollection<InviteListItem>(_originalItems);
    }

    public async Task PermissionUpdate()
    {
        if (CanCreate.HasValue)
            CanCreate = services.Socket.State.CurrentServer.HasPermission(services.Socket.State.CurrentServer.Member, ChannelPermission.CreateInvites);

        CanManage = services.Socket.State.CurrentServer.HasPermission(services.Socket.State.CurrentServer.Member, ChannelPermission.ManageInvites);
    }

    [ObservableProperty]
    private bool? canCreate;

    [ObservableProperty]
    private bool canManage;

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
                .Where(item => item.Code.Contains(SearchString, StringComparison.OrdinalIgnoreCase))
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
    private ObservableCollection<InviteListItem> _items;

    [RelayCommand]
    public async Task CreateInvite()
    {
        if (!services.Socket.State.CurrentServer.Channels.Any())
            return;

        try
        {
            RestInvite invite = await services.Rest.CreateInviteAsync(services.Socket.State.CurrentServer.Channels.Values.First().Id);
            services.CopyText(invite.Code);
            CanCreate = null;
        }
        catch { }
    }
}
public partial class InviteListItem : ObservableObject
{
    public ServiceManager services;

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private string _inviter;

    [ObservableProperty]
    private string _code;

    [ObservableProperty]
    private string channel;

    public string channelId;

    [ObservableProperty]
    private ulong uses;

    [RelayCommand]
    public void CopyInvite()
    {
        services.CopyText(Code);
    }

    [RelayCommand]
    public async Task DeleteInvite()
    {
        try
        {
            await services.Rest.DeleteInviteAsync(channelId, Code);
        }
        catch { }
    }
}