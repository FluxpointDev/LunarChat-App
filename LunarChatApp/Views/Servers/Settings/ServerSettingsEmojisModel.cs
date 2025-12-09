using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using LunarChatApp.Services;
using LunarChatApp.Views.Dialogs;
using LunarChatSharp;
using LunarChatSharp.Core.Servers;
using LunarChatSharp.Rest.Messages;
using LunarChatSharp.Rest.Servers;
using LunarChatSharp.Websocket.Events.Servers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace LunarChatApp.Views.Servers.Settings;

public partial class ServerSettingsEmojisModel : ViewModelBase
{
    private readonly List<EmojiListItem> _originalItems = new List<EmojiListItem>();

    private readonly Timer? _searchTimer;
    private ServiceManager services;
    public ServerSettingsEmojisModel(ServiceManager sv)
    {
        services = sv;
        services.Client.OnEmojiCreate += EmojiCreated;
        services.Client.OnEmojiUpdate += EmojiUpdated;
        services.Client.OnEmojiDelete += EmojiDeleted;
        services.State.Socket.CurrentServer.OnPermissionUpdate += PermissionUpdate;
        _canCreate = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ServerPermission.CreateExpressions);
        bool CanManage = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ServerPermission.ManageExpressions);
        _searchTimer = new Timer(500); // 500ms debounce
        _searchTimer.Elapsed += SearchTimerElapsed;
        _searchTimer.AutoReset = false;

        PropertyChanged += OnPropertyChanged;

        _originalItems = services.State.Socket.CurrentServer.Emojis.Values.Select(x => new EmojiListItem(services)
        {
            Id = x.Id,
            Name = x.Name,
            Creator = x.CreatedBy,
            CanManage = x.CreatedBy == services.Client.CurrentId || CanManage
        }).ToList();

        foreach (var i in _originalItems) i.PropertyChanged += OnItemsChanged;
        Items = new ObservableCollection<EmojiListItem>(_originalItems);
    }

    private async Task PermissionUpdate()
    {
        CanCreate = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ServerPermission.CreateExpressions);
        bool CanManage = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ServerPermission.ManageExpressions);
        foreach (var i in _originalItems)
        {
            i.CanManage = i.Creator == services.Client.CurrentId || CanManage;
        }
    }

    [ObservableProperty]
    private bool _canCreate;

    private async Task EmojiDeleted(RestServer server, RestEmoji emoji)
    {
        EmojiListItem? item = _originalItems.FirstOrDefault(x => x.Id == emoji.Id);
        if (item == null)
            return;

        item.PropertyChanged -= OnItemsChanged;
        _originalItems.Remove(item);
        UpdateList();
    }

    private async Task EmojiUpdated(RestServer server, RestEmoji emoji, EmojiUpdateEvent ev)
    {
        EmojiListItem? item = _originalItems.FirstOrDefault(x => x.Id == emoji.Id);
        if (item == null)
            return;

        item.Name = ev.Name!;
        UpdateList();
    }

    private async Task EmojiCreated(RestServer server, RestEmoji emoji)
    {
        bool CanManage = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ServerPermission.ManageExpressions);
        EmojiListItem item = new EmojiListItem(services)
        {
            Id = emoji.Id,
            Creator = emoji.CreatedBy,
            Name = emoji.Name,
            CanManage = emoji.CreatedBy == services.Client.CurrentId || CanManage
        };
        _originalItems.Add(item);
        item.PropertyChanged += OnItemsChanged;
        UpdateList();
    }

    [RelayCommand]
    public void CreateEmote()
    {
        services.Dialogs.Create(new CreateNameDialog(), new CreateNameDialogModel
        {
        }, "Create Emoji").WithSubmit(SubmitEmoji).Open();
    }

    public async Task SubmitEmoji(UserControl control)
    {
        CreateNameDialogModel? model = control.DataContext as CreateNameDialogModel;
        if (model.Name == null)
            model.Name = "";

        try
        {
            await services.Rest.CreateEmojiAsync(services.State.Socket.CurrentServer?.Server.Id, new CreateEmojiRequest
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
    private ObservableCollection<EmojiListItem> _items;

}
public partial class EmojiListItem(ServiceManager services) : ObservableObject
{
    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private string _name;

    public string Id;

    [ObservableProperty]
    private string _creator;

    [ObservableProperty]
    private bool _canManage;

    [RelayCommand]
    public void EditEmoji()
    {
        services.Dialogs.Create(new CreateNameDialog(), new CreateNameDialogModel
        {
            Name = Name
        }, "Edit Emoji").WithSubmit(SubmitEmoji).Open();
    }
    public async Task SubmitEmoji(UserControl control)
    {
        CreateNameDialogModel? model = control.DataContext as CreateNameDialogModel;

        try
        {
            await services.Rest.EditEmojiAsync(services.State.Socket.CurrentServer?.Server.Id, Id, new EditEmojiRequest
            {
                Name = model.Name
            });
        }
        catch { }
    }

    [RelayCommand]
    public void CopyId()
    {
        services.CopyText(Id);
    }

    [RelayCommand]
    public async Task DeleteEmoji()
    {
        try
        {
            await services.Rest.DeleteEmojiAsync(services.State.Socket.CurrentServer?.Server.Id, Id);
        }
        catch { }
    }
}