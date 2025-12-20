using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using LunarChatApp.Services;
using LunarChatApp.Views.Dialogs;
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
        if (services.State.CurrentServer != null)
        {
            services.State.CurrentServer.OnPermissionUpdate += PermissionUpdate;
            canCreate = services.State.CurrentServer.HasPermission(services.State.CurrentServer.Member, ChannelPermission.CreateInvites);
            canManage = services.State.CurrentServer.HasPermission(services.State.CurrentServer.Member, ChannelPermission.ManageInvites);
        }

        services.Client.OnInviteCreate += InviteCreate;
        services.Client.OnInviteDelete += InviteDelete;

        PropertyChanged += OnPropertyChanged;

        foreach (var i in _originalItems) i.PropertyChanged += OnItemsChanged;
        Items = new ObservableCollection<InviteListItem>(_originalItems);

        _ = Task.Run(async () =>
        {
            if (services.State.CurrentServer == null)
                return;

            var Invites = await services.Rest.GetServerInvitesAsync(services.State.CurrentServer.Server.Id);
            if (Invites == null)
                return;

            Dispatcher.UIThread.Post(() =>
            {
                _originalItems.AddRange(Invites.Select(x => new InviteListItem(services, x)));
                Items = new ObservableCollection<InviteListItem>(_originalItems);
                Loaded = true;
            });
        });
    }

    [ObservableProperty]
    private bool loaded;

    private async Task InviteDelete(RestServer server, string arg2)
    {
        if (services.State.CurrentServer == null || server.Id != services.State.CurrentServer.Server.Id)
            return;

        var item = _originalItems.FirstOrDefault(x => x.Code == arg2);
        if (item == null)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            _originalItems.Remove(item);
            Items = new ObservableCollection<InviteListItem>(_originalItems);
        });

    }

    private async Task InviteCreate(RestServer server, RestInvite invite)
    {
        if (services.State.CurrentServer == null || server.Id != services.State.CurrentServer.Server.Id)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            _originalItems.Add(new InviteListItem(services, invite));
            Items = new ObservableCollection<InviteListItem>(_originalItems);
        });

    }

    public async Task PermissionUpdate(RestServer server)
    {
        if (services.State.CurrentServer == null)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (CanCreate.HasValue)
                CanCreate = services.State.CurrentServer.HasPermission(services.State.CurrentServer.Member, ChannelPermission.CreateInvites);

            CanManage = services.State.CurrentServer.HasPermission(services.State.CurrentServer.Member, ChannelPermission.ManageInvites);
        });

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
    public void CreateInvite()
    {
        if (services.State.CurrentServer == null)
            return;

        if (services.State.CurrentServer.Channels.IsEmpty)
            return;

        services.Dialogs.Create(new CreateInviteDialog(), new CreateInviteDialogModel(), "Create Invite").WithSubmit(SubmitInvite).Open();
    }

    public async Task SubmitInvite(UserControl control)
    {
        CreateInviteDialogModel? model = control.DataContext as CreateInviteDialogModel;
        if (model == null || services.State.CurrentServer == null)
            return;

        CreateInviteRequest req = model.CreateRequest();
        try
        {
            var Channel = services.State.CurrentServer.Channels.Values.First().Id;
            RestInvite invite = await services.Rest.CreateInviteAsync(Channel, req);
            services.CopyText(invite.Code);
        }
        catch { }
    }
}
public partial class InviteListItem : ObservableObject
{
    public ServiceManager services;

    public InviteListItem(ServiceManager sv, RestInvite invite)
    {
        services = sv;
        _code = invite.Code;
        if (invite.Inviter != null)
            _inviter = invite.Inviter.GetCurrentNameDiscrim();
        uses = invite.MaxUses != 0 ? $"{invite.Uses}/{invite.MaxUses}" : invite.Uses.ToString();

        expires = invite.ExpiresAt.HasValue ? invite.ExpiresAt.Value.ToLocalTime().ToString("dd/MM/yyyy (hh:mm tt)") : null;
        channelId = invite.ChannelId;
        Channel = services.Socket.State.Channels.TryGetValue(invite.ChannelId, out var channel) ? "#" + channel.Name : "#invalid-channel";
    }

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
    private string uses;

    [ObservableProperty]
    private string expires;



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