using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using LunarChatApp.Services;
using LunarChatApp.Views.Dialogs;
using LunarChatSharp;
using LunarChatSharp.Rest.Channels;
using LunarChatSharp.Rest.Users;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace LunarChatApp.Views.Channels.Settings;

public partial class ChannelSettingsGroupUsersModel : ViewModelBase
{
    private readonly List<GroupUserListItem> _originalItems = new List<GroupUserListItem>();

    private readonly Timer? _searchTimer;
    private ServiceManager services;

    public ChannelSettingsGroupUsersModel(ServiceManager sv)
    {
        services = sv;

        services = sv;
        _searchTimer = new Timer(500); // 500ms debounce
        _searchTimer.Elapsed += SearchTimerElapsed;
        _searchTimer.AutoReset = false;
        PropertyChanged += OnPropertyChanged;
        services.Client.OnGroupUpdate += GroupUpdate;
        services.Client.OnGroupAddUser += AddUser;
        services.Client.OnGroupRemoveUser += RemoveUser;
        canManage = services.Client.CurrentId == services.State.CurrentChannel?.GroupSettings?.OwnerId;
        foreach (var i in _originalItems) i.PropertyChanged += OnItemsChanged;
        Items = new ObservableCollection<GroupUserListItem>(_originalItems);

        _ = Task.Run(async () =>
        {
            var Users = await services.Rest.GetAsync<RestUser[]>($"/groups/{services.State.CurrentChannel?.Id}/users");
            if (Users == null)
                return;

            Dispatcher.UIThread.Post(() =>
            {
                _originalItems.AddRange(Users.Select(x => new GroupUserListItem(services, x)));
                Items = new ObservableCollection<GroupUserListItem>(_originalItems);
                Loaded = true;
            });
        });
    }


    private async Task RemoveUser(RestChannel channel, string arg2)
    {
        if (channel.Id != services.State.CurrentChannel?.Id)
            return;

        var item = _originalItems.FirstOrDefault(x => x.id == arg2);
        if (item == null)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            _originalItems.Remove(item);
            Items = new ObservableCollection<GroupUserListItem>(_originalItems);
        });

    }

    private async Task AddUser(RestChannel channel, RestUser user)
    {
        if (channel.Id != services.State.CurrentChannel?.Id)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            _originalItems.Add(new GroupUserListItem(services, user));
            Items = new ObservableCollection<GroupUserListItem>(_originalItems);
        });

    }

    private async Task GroupUpdate(RestChannel channel, UpdateChannelRequest request)
    {
        if (channel.Id != services.State.CurrentChannel?.Id)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            CanManage = services.Client.CurrentId == services.State.CurrentChannel?.GroupSettings?.OwnerId;


            foreach (var i in _originalItems)
            {
                i.Update(i.user);
            }
        });

    }

    [ObservableProperty]
    private bool loaded;

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

    [RelayCommand]
    public void AddFriend()
    {
        services.Dialogs.Create(new AddFriendDialog(), new AddFriendDialogModel(), "Add Friend to Group").WithSubmit(SubmitFriend).Open();
    }

    public async Task SubmitFriend(UserControl control)
    {
        try
        {
            AddFriendDialogModel? data = control.DataContext as AddFriendDialogModel;
            var friend = services.State.Socket.Relations.Values.FirstOrDefault(x => x.UserId == data.Username || x.Username == data.Username);
            await services.Rest.PutAsync($"/groups/{services.State.CurrentChannel?.Id}/users", new GroupAddUserRequest
            {
                UserId = friend.UserId
            });
        }
        catch { }
    }

    [ObservableProperty]
    private int _selectedCount;

    [ObservableProperty]
    private int _totalCount;

    [ObservableProperty]
    private ObservableCollection<GroupUserListItem> _items;
}
public partial class GroupUserListItem : ObservableObject
{
    private ServiceManager services;
    public RestUser user;
    public GroupUserListItem(ServiceManager sv, RestUser u)
    {
        services = sv;
        user = u;
        isBot = u.IsBot;
        id = user.Id;
        Update(user);
    }

    public void Update(RestUser user)
    {
        Name = user.GetCurrentNameDiscrim();
        RemoveItemText = "Remove " + Name;
        CanManage = services.Client.CurrentId == services.State.CurrentChannel?.GroupSettings?.OwnerId && id != services.State.CurrentChannel?.GroupSettings?.OwnerId;
        CanTransferOwner = CanManage && !isBot;
    }

    [ObservableProperty]
    private bool _isSelected;

    public string id;

    [ObservableProperty]
    private string _name;

    private bool isBot;

    [ObservableProperty]
    private string removeItemText;

    [ObservableProperty]
    private bool canManage;

    [ObservableProperty]
    private bool canTransferOwner;

    [RelayCommand]
    public void CopyUserID()
    {
        services.CopyText(id);
    }

    [RelayCommand]
    public async Task TransferOwnership()
    {
        if (isBot)
            return;

        try
        {
            await services.Rest.UpdateChannelAsync(services.State.CurrentChannel.Id, new UpdateChannelRequest
            {
                OwnerId = id
            });
        }
        catch { }
    }

    [RelayCommand]
    public async Task RemoveUser()
    {
        try
        {
            if (isBot)
                await services.Rest.RemoveGroupAppAsync(services.State.CurrentChannel.Id, id);
            else
                await services.Rest.DeleteAsync($"/groups/{services.State.CurrentChannel.Id}/users/{id}");
        }
        catch { }
    }
}
