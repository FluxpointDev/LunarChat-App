using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using LunarChatApp.Services;
using LunarChatApp.Views.Dialogs;
using LunarChatSharp.Rest.Roles;
using LunarChatSharp.Rest.Servers;
using LunarChatSharp.Websocket.Events.Roles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace LunarChatApp.Views.Servers.Settings;

public partial class ServerSettingsRolesModel : ViewModelBase
{
    private readonly List<RoleListItem> _originalItems = new List<RoleListItem>();

    private readonly Timer? _searchTimer;
    private ServiceManager services;
    private Action openRoles;
    private Action<RestRole> openInfo;
    public ServerSettingsRolesModel(ServiceManager sv, Action openRole, Action<RestRole> openInfo)
    {
        services = sv;
        this.openRoles = openRole;
        this.openInfo = openInfo;
        services.State.Socket.OnRoleCreate += RoleCreated;
        services.State.Socket.OnRoleUpdate += RoleUpdated;
        services.State.Socket.OnRoleDelete += RoleDeleted;
        _searchTimer = new Timer(500); // 500ms debounce
        _searchTimer.Elapsed += SearchTimerElapsed;
        _searchTimer.AutoReset = false;

        _originalItems = services.State.Socket.CurrentServer.Roles.Values.Select(x => new RoleListItem(services, openInfo)
        {
            Color = x.Color ?? "#99AAB5",
            Name = x.Name,
            Id = x.Id,
        }).ToList();

        PropertyChanged += OnPropertyChanged;

        foreach (var i in _originalItems)
            i.PropertyChanged += OnItemsChanged;
        Items = new ObservableCollection<RoleListItem>(_originalItems);
    }

    private async Task RoleDeleted(RestRole role)
    {
        RoleListItem? item = _originalItems.FirstOrDefault(x => x.Id == role.Id);
        if (item == null)
            return;

        item.PropertyChanged -= OnItemsChanged;
        _originalItems.Remove(item);
        UpdateList();
    }

    private async Task RoleUpdated(RoleUpdateEvent ev, RestRole role)
    {
        RoleListItem? item = _originalItems.FirstOrDefault(x => x.Id == role.Id);
        if (item == null)
            return;

        item.Name = ev.Name!;
        item.Color = ev.Color ?? "#99AAB5";
        UpdateList();
    }

    private async Task RoleCreated(RestServer server, RestRole role)
    {
        RoleListItem item = new RoleListItem(services, openInfo)
        {
            Color = role.Color ?? "#99AAB5",
            Id = role.Id,
            Name = role.Name,
        };
        _originalItems.Add(item);
        item.PropertyChanged += OnItemsChanged;
        UpdateList();
    }

    public void UpdateList()
    {
        _searchTimer?.Stop();
        Items.Clear();
        if (string.IsNullOrEmpty(SearchString))
            Items.AddRange(_originalItems);
        else
        {
            var filteredItems = _originalItems
                .Where(item => item.Name.Contains(SearchString, StringComparison.OrdinalIgnoreCase))
                .ToList();

            Items.AddRange(filteredItems);
        }
        UpdateTotal();
    }

    [RelayCommand]
    public void CreateRole()
    {
        services.Dialogs.Create(new CreateNameDialog(), new CreateNameDialogModel
        {
        }, "Create Role").WithSubmit(SubmitRole).Open();
    }

    public async Task SubmitRole(UserControl control)
    {
        CreateNameDialogModel? model = control.DataContext as CreateNameDialogModel;
        if (model.Name == null)
            model.Name = "";

        await services.Rest.PostAsync($"/servers/{services.State.Socket.CurrentServer?.Server.Id}/roles", new CreateRoleRequest
        {
            Name = model.Name
        });
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
    private ObservableCollection<RoleListItem> _items;
}
public partial class RoleListItem(ServiceManager services, Action<RestRole> openInfo) : ObservableObject
{
    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _color;

    [ObservableProperty]
    private string _id;

    [RelayCommand]
    public void OpenRole()
    {
        if (services.State.Socket.Roles.TryGetValue(Id, out var role))
            openInfo.Invoke(role);
    }

    [RelayCommand]
    public void CopyId()
    {
        services.CopyText(Id);
    }

    [RelayCommand]
    public async Task DeleteRole()
    {
        await services.Rest.DeleteAsync($"/servers/{services.State.Socket.CurrentServer?.Server.Id}/roles/{Id}");
    }
}
