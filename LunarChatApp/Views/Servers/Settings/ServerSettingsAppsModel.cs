using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using LunarChatApp.Services;
using LunarChatSharp;
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

    public ServerSettingsAppsModel(ServiceManager sv)
    {
        _searchTimer = new Timer(500); // 500ms debounce
        _searchTimer.Elapsed += SearchTimerElapsed;
        _searchTimer.AutoReset = false;

        PropertyChanged += OnPropertyChanged;

        _ = Task.Run(async () =>
        {
            var apps = await sv.Rest.GetServerAppsAsync(sv.State.Socket.CurrentServer?.Server.Id);
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
                foreach (var i in _originalItems) i.PropertyChanged += OnItemsChanged;
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

    [ObservableProperty]
    private bool _isLoaded;

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
            await services.Rest.RemoveServerAppAsync(services.State.Socket.CurrentServer?.Server.Id, Id);
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