using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using LunarChatApp.Services;
using LunarChatSharp;
using LunarChatSharp.Core.Servers;
using LunarChatSharp.Rest.Servers;
using Material.Icons;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace LunarChatApp.Views.Servers.Settings;

public partial class ServerSettingsAuditLogsModel : ViewModelBase
{
    private readonly List<AuditListItem> _originalItems = new List<AuditListItem>();

    private readonly Timer? _searchTimer;
    private ServiceManager services;

    public ServerSettingsAuditLogsModel(ServiceManager sv)
    {
        services = sv;
        _canManage = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ModPermission.ManageRoles);

        services.State.Socket.CurrentServer.OnPermissionUpdate += PermissionUpdate;
        _searchTimer = new Timer(500); // 500ms debounce
        _searchTimer.Elapsed += SearchTimerElapsed;
        _searchTimer.AutoReset = false;

        _canManage = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ModPermission.ViewAuditLogs);

        PropertyChanged += OnPropertyChanged;

        Items = new ObservableCollection<AuditListItem>(_originalItems);
        _ = Task.Run(async () =>
        {
            try
            {
                var AuditLogs = await services.Rest.GetAuditLogsAsync(services.Socket.State.CurrentServer.Server.Id);
                if (AuditLogs == null)
                    return;


                Dispatcher.UIThread.Post(() =>
                {
                    _originalItems.AddRange(AuditLogs.Select(x => new AuditListItem(x)));
                    Items = new ObservableCollection<AuditListItem>(_originalItems);
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        });
    }

    private async Task PermissionUpdate()
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            CanManage = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ModPermission.ViewAuditLogs);

        });

    }

    [ObservableProperty]
    private bool _canManage;

    public void UpdateList()
    {
        _searchTimer?.Stop();
        Items.Clear();
        if (string.IsNullOrEmpty(SearchString))
            Items.AddRange(_originalItems);
        else
        {
            var filteredItems = _originalItems
                .Where(item => item.Text.Contains(SearchString, StringComparison.OrdinalIgnoreCase))
                .ToList();

            Items.AddRange(filteredItems);
        }
    }

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
                .Where(item => item.Text.Contains(SearchString, StringComparison.OrdinalIgnoreCase))
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
    private ObservableCollection<AuditListItem> _items;

}
public partial class AuditListItem : ObservableObject
{
    public AuditListItem(RestAuditLog audit)
    {
        try
        {
            actionUser = audit.UserName;
            targetName = audit.TargetName;
            date = audit.ActionAt.Value.ToLocalTime().ToString("hh:mm tt");
            switch (audit.ActionType)
            {
                case ActionType.ServerUpdate:
                    text = "Updated Server";
                    icon = MaterialIconKind.HomeEdit;
                    break;
            }
            changes = new ObservableCollection<AuditLogChangeItemModel>();
            foreach (var i in audit.Changes)
            {
                changes.Add(new AuditLogChangeItemModel
                {
                    Text = $"Changed name from {i.OldValue} to {i.NewValue}"
                });
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    [ObservableProperty]
    public MaterialIconKind icon;

    [ObservableProperty]
    private string actionUser;

    [ObservableProperty]
    private string targetName;

    [ObservableProperty]
    public string text;

    [ObservableProperty]
    public string date;

    [ObservableProperty]
    private bool showChanges;

    [RelayCommand]
    public void ToggleChanges()
    {
        ShowChanges = !ShowChanges;
        if (ShowChanges)
            Arrow = MaterialIconKind.KeyboardArrowUp;
        else
            Arrow = MaterialIconKind.KeyboardArrowDown;
    }

    [ObservableProperty]
    private MaterialIconKind arrow;

    [ObservableProperty]
    public ObservableCollection<AuditLogChangeItemModel> changes;
}