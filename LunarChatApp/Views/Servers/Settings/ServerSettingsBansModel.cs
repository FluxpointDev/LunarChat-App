using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Timers;

namespace LunarChatApp.Views.Servers.Settings;

public partial class ServerSettingsBansModel : ViewModelBase
{
    private readonly List<BanListItem> _originalItems = new List<BanListItem>();

    private readonly Timer? _searchTimer;

    public ServerSettingsBansModel()
    {
        _searchTimer = new Timer(500); // 500ms debounce
        _searchTimer.Elapsed += SearchTimerElapsed;
        _searchTimer.AutoReset = false;

        PropertyChanged += OnPropertyChanged;

        foreach (var i in _originalItems) i.PropertyChanged += OnItemsChanged;
        Items = new ObservableCollection<BanListItem>(_originalItems);
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
    private ObservableCollection<BanListItem> _items;
}
public partial class BanListItem : ObservableObject
{
    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string reason;
}