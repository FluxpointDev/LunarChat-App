using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using LunarChatApp.Services;
using LunarChatSharp;
using LunarChatSharp.Core.Servers;
using LunarChatSharp.Rest.Channels;
using LunarChatSharp.Rest.Dev;
using LunarChatSharp.Rest.Servers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace LunarChatApp.Views.Servers.Settings;

public partial class ServerSettingsAppsModel : ViewModelBase
{
    public List<AppListItem> _originalItems = new List<AppListItem>();

    private readonly Timer? _searchTimer;
    private readonly ServiceManager services;
    public ServerSettingsAppsModel(ServiceManager sv, bool isGroup)
    {
        services = sv;
        IsGroup = isGroup;
        _searchTimer = new Timer(500); // 500ms debounce
        _searchTimer.Elapsed += SearchTimerElapsed;
        _searchTimer.AutoReset = false;
        if (IsGroup)
        {
            canManage = sv.State.CurrentChannel?.GroupSettings?.OwnerId == services.Client.CurrentId;
        }
        else
        {
            if (services.State.CurrentServer != null)
            {
                canManage = services.State.CurrentServer.HasPermission(services.State.CurrentServer.Member, ServerPermission.ManageApps);
                services.State.CurrentServer.OnPermissionUpdate += PermissionUpdate;
            }
        }
        sv.Client.OnAppAdd += AppAdd;
        sv.Client.OnAppUpdate += AppUpdate;
        sv.Client.OnAppRemove += AppRemove;
        PropertyChanged += OnPropertyChanged;

        _ = Task.Run(async () =>
        {
            var apps = IsGroup ? await sv.Rest.GetGroupAppsAsync(sv.State.CurrentChannel?.Id!) :
            await sv.Rest.GetServerAppsAsync(sv.State.CurrentServer?.Server.Id!);
            if (apps != null)
            {
                foreach (var i in apps)
                {
                    _originalItems.Add(new AppListItem(sv, this)
                    {
                        Name = i.Name,
                        Id = i.Id,
                    });
                }
                Dispatcher.UIThread.Post(() =>
                {
                    Items = new ObservableCollection<AppListItem>(_originalItems);
                    IsLoaded = true;
                });
            }
            else
            {
                Dispatcher.UIThread.Post(() =>
                {
                    IsLoaded = true;
                });
            }
        });


    }

    public bool IsGroup;

    private async Task AppRemove(RestServer? server, RestChannel? channel, RestApp app)
    {
        if (IsGroup)
        {
            if (services.State.CurrentChannel?.Id != channel?.Id)
                return;
        }
        else
        {
            if (services.State.CurrentServer?.Server?.Id != server?.Id)
                return;
        }

        var item = _originalItems.FirstOrDefault(x => x.Id == app.Id);
        if (item == null)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            _originalItems.Remove(item);
            Items = new ObservableCollection<AppListItem>(_originalItems);
        });


    }

    private async Task AppUpdate(RestServer? server, RestChannel? channel, RestApp app, EditAppRequest changed)
    {
        if (IsGroup)
        {
            if (services.State.CurrentChannel?.Id != channel?.Id)
                return;
        }
        else
        {
            if (services.State.CurrentServer?.Server?.Id != server?.Id)
                return;
        }

        if (string.IsNullOrEmpty(changed.Name))
            return;

        var item = _originalItems.FirstOrDefault(x => x.Id == app.Id);
        if (item == null)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (changed.Name != null)
                item.Name = changed.Name;
        });

    }

    private async Task AppAdd(RestServer? server, RestChannel? channel, RestApp app)
    {
        if (IsGroup)
        {
            if (services.State.CurrentChannel?.Id != channel?.Id)
                return;
        }
        else
        {
            if (services.State.CurrentServer?.Server?.Id != server?.Id)
                return;
        }

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            _originalItems.Add(new AppListItem(services, this)
            {
                Id = app.Id,
                Name = app.Name
            });
            Items = new ObservableCollection<AppListItem>(_originalItems);
        });

    }

    private async Task PermissionUpdate(RestServer server)
    {
        if (services.State.CurrentServer == null)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            CanManage = services.State.CurrentServer.HasPermission(services.State.CurrentServer.Member, ServerPermission.ManageApps);
        });

    }

    [ObservableProperty]
    private bool _isLoaded;

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
    private ObservableCollection<AppListItem> _items;
}
public partial class AppListItem(ServiceManager services, ServerSettingsAppsModel apps) : ObservableObject
{
    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private string _name;

    public string Id;

    [RelayCommand]
    public void CopyId()
    {
        services.CopyText(Id);
    }

    [RelayCommand]
    public async Task DeleteApp()
    {
        try
        {
            if (apps.IsGroup)
            {
                if (services.State.CurrentChannel == null)
                    return;

                await services.Rest.RemoveGroupAppAsync(services.State.CurrentChannel.Id, Id);
            }
            else
            {
                if (services.State.CurrentServer == null)
                    return;

                await services.Rest.RemoveServerAppAsync(services.State.CurrentServer.Server.Id, Id);
            }

            var app = apps._originalItems.FirstOrDefault(x => x.Id == Id);
            if (app != null)
            {
                apps._originalItems.Remove(app);
                apps.Items.Remove(app);
            }
        }
        catch { }

    }
}