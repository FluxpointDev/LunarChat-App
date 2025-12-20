using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using LunarChatApp.Services;
using LunarChatApp.Views.Dialogs;
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

public partial class ServerSettingsRolesModel : ViewModelBase
{
    private readonly List<RoleListItem> _originalItems = new List<RoleListItem>();

    private readonly Timer? _searchTimer;
    private readonly ServiceManager services;
    private readonly Action openRoles;
    private readonly Action<RestRole> openInfo;
    public ServerSettingsRolesModel(ServiceManager sv, Action openRole, Action<RestRole> openInfo)
    {
        services = sv;
        this.openRoles = openRole;
        this.openInfo = openInfo;
        if (services.State.CurrentServer != null)
        {
            _canManage = services.State.CurrentServer.HasPermission(services.State.CurrentServer.Member, ModPermission.ManageRoles);
            services.State.CurrentServer.OnPermissionUpdate += PermissionUpdate;
        }

        services.Client.OnRoleCreate += RoleCreated;
        services.Client.OnRoleUpdate += RoleUpdated;
        services.Client.OnRoleDelete += RoleDeleted;

        _searchTimer = new Timer(500); // 500ms debounce
        _searchTimer.Elapsed += SearchTimerElapsed;
        _searchTimer.AutoReset = false;

        _originalItems = services.State.CurrentServer.Roles.Values.OrderByDescending(x => x.Position).Select(x => new RoleListItem(services, x, _canManage, openInfo)).ToList();

        PropertyChanged += OnPropertyChanged;

        Items = new ObservableCollection<RoleListItem>(_originalItems);
    }

    private async Task PermissionUpdate(RestServer server)
    {
        if (services.State.CurrentServer == null)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            CanManage = services.State.CurrentServer.HasPermission(services.State.CurrentServer.Member, ModPermission.ManageRoles);
            foreach (var i in _originalItems)
            {
                i.CanManage = CanManage;
            }
        });

    }

    [ObservableProperty]
    private bool _canManage;

    private async Task RoleDeleted(RestServer server, RestRole role)
    {
        RoleListItem? item = _originalItems.FirstOrDefault(x => x.Id == role.Id);
        if (item == null)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            _originalItems.Remove(item);
            UpdateList();
        });

    }

    private async Task RoleUpdated(RestServer server, RestRole role, EditRoleRequest ev)
    {
        RoleListItem? item = _originalItems.FirstOrDefault(x => x.Id == role.Id);
        if (item == null)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (ev.Name != null)
                item.Name = ev.Name!;

            if (ev.Color != null)
                item.Color = ev.Color ?? "#99AAB5";

            if (ev.Icon != null)
                item.Icon = string.IsNullOrEmpty(ev.Icon) ? null : new Uri(ev.GetIconUrl()!);
            UpdateList();
        });

    }

    private async Task RoleCreated(RestServer server, RestRole role)
    {
        if (services.State.CurrentServer == null)
            return;

        RoleListItem item = new RoleListItem(services, role, services.State.CurrentServer.HasPermission(services.State.CurrentServer.Member, ModPermission.ManageRoles), openInfo);
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            _originalItems.Add(item);
            UpdateList();
        });

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
                .Where(item => item.Name.Contains(SearchString, StringComparison.OrdinalIgnoreCase)).OrderByDescending(x => x.Position)
                .ToList();

            Items.AddRange(filteredItems);
        }
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
        if (model == null || services.State.CurrentServer == null)
            return;

        if (model.Name == null)
            model.Name = "";

        try
        {
            await services.Rest.CreateRoleAsync(services.State.CurrentServer.Server.Id, new CreateRoleRequest
            {
                Name = model.Name
            });
        }
        catch { }
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

    [RelayCommand]
    public void DefaultMembersRole()
    {
        if (services.State.CurrentServer == null)
            return;


        openInfo.Invoke(new RestRole
        {
            Id = "0",
            CreatedAt = DateTime.Now,
            Name = "Default Members Role",
            Permissions = services.State.CurrentServer.Server.DefaultPermissions
        });
    }

    [ObservableProperty]
    private ObservableCollection<RoleListItem> _items;
}
public partial class RoleListItem : ObservableObject
{
    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private string _name;

    public int Position;

    [ObservableProperty]
    private string _color;

    [ObservableProperty]
    private string _id;

    [ObservableProperty]
    private Uri? icon;

    [ObservableProperty]
    private bool _canManage;

    private readonly ServiceManager services;
    private readonly Action<RestRole> openInfo;

    public RoleListItem(ServiceManager sv, RestRole role, bool canManage, Action<RestRole> openInfo)
    {
        services = sv;
        this.openInfo = openInfo;
        _color = role.Color ?? "#99AAB5";
        _name = role.Name;
        _id = role.Id;
        Position = role.Position;
        Icon = string.IsNullOrEmpty(role.IconId) ? null : new Uri(role.GetIconUrl()!);
        _canManage = canManage;
    }

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
        if (services.State.CurrentServer == null)
            return;

        try
        {
            await services.Rest.DeleteRoleAsync(services.State.CurrentServer.Server.Id, Id);
        }
        catch { }
    }
}
