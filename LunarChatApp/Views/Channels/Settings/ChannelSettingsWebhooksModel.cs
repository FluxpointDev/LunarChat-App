using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using LunarChatApp.Services;
using LunarChatApp.Views.Dialogs;
using LunarChatSharp.Core.Channels;
using LunarChatSharp.Core.Servers;
using LunarChatSharp.Rest.Channels;
using LunarChatSharp.Rest.Helpers;
using LunarChatSharp.Rest.Webhooks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace LunarChatApp.Views.Channels.Settings;

public partial class ChannelSettingsWebhooksModel : ViewModelBase
{
    private ServiceManager services;
    private List<WebhookListItem> _originalItems = new List<WebhookListItem>();
    private readonly Timer? _searchTimer;
    private Action openWebhook;
    private Action<RestWebhook> openInfo;

    public ChannelSettingsWebhooksModel(ServiceManager sv, Action openWebhook, Action<RestWebhook> openInfo)
    {
        services = sv;
        this.openWebhook = openWebhook;
        this.openInfo = openInfo;

        if (services.State.Socket.CurrentChannel.Type == ChannelType.Group)
        {
            _canManage = services.Client.CurrentId == services.State.Socket.CurrentChannel.GroupSettings?.OwnerId;

        }
        else
        {
            if (services.State.Socket.CurrentServer != null)
            {
                _canManage = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ChannelPermission.ManageWebhooks);
                services.State.Socket.CurrentServer.OnPermissionUpdate += PermissionUpdate;
            }
        }

        services.Client.OnWebhookCreate += WebhookCreate;
        services.Client.OnWebhookUpdate += WebhookUpdate;
        services.Client.OnWebhookDelete += WebhookDelete;

        _searchTimer = new Timer(500); // 500ms debounce
        _searchTimer.Elapsed += SearchTimerElapsed;
        _searchTimer.AutoReset = false;

        PropertyChanged += OnPropertyChanged;

        foreach (var i in _originalItems)
            i.PropertyChanged += OnItemsChanged;
        Items = new ObservableCollection<WebhookListItem>();

        _ = Task.Run(async () =>
        {
            var webhooks = await services.Rest.GetWebhooksAsync(services.State.Socket.CurrentChannel?.Id);
            if (webhooks == null)
                return;

            Dispatcher.UIThread.Post(() =>
            {
                _originalItems = webhooks.Select(x => new WebhookListItem(services, openInfo)
                {
                    Name = x.Name,
                    Id = x.Id,
                    CanManage = _canManage,
                    channelId = x.ChannelId,
                    token = x.Token
                }).ToList();

                Items = new ObservableCollection<WebhookListItem>(_originalItems);
            });
        });



    }

    [RelayCommand]
    public void CreateWebhook()
    {
        services.Dialogs.Create(new CreateNameDialog(), new CreateNameDialogModel(), "Create Webhook").WithSubmit(SubmitWebhook).Open();
    }

    public async Task SubmitWebhook(UserControl control)
    {
        CreateNameDialogModel? model = control.DataContext as CreateNameDialogModel;
        if (model.Name == null)
            model.Name = "";

        try
        {
            await services.Rest.CreateWebhookAsync(services.State.Socket.CurrentChannel?.Id, new CreateWebhookRequest
            {
                Name = model.Name
            });
        }
        catch { }
    }


    private async Task WebhookDelete(RestChannel server, string webhookId)
    {
        if (services.State.Socket.CurrentChannel?.Id != server.Id)
            return;

        WebhookListItem? item = _originalItems.FirstOrDefault(x => x.Id == webhookId);
        if (item == null)
            return;

        item.PropertyChanged -= OnItemsChanged;
        _originalItems.Remove(item);
        UpdateList();
    }

    private async Task WebhookUpdate(RestChannel server, string webhookId, EditWebhookRequest ev)
    {
        if (services.State.Socket.CurrentChannel?.Id != server.Id)
            return;

        WebhookListItem? item = _originalItems.FirstOrDefault(x => x.Id == webhookId);
        if (item == null)
            return;

        item.Name = ev.Name;
        UpdateList();
    }

    private async Task WebhookCreate(RestChannel server, RestWebhook webhook)
    {
        if (services.State.Socket.CurrentChannel?.Id != server.Id)
            return;

        WebhookListItem item = new WebhookListItem(services, openInfo)
        {
            Id = webhook.Id,
            Name = webhook.Name,
            channelId = webhook.ChannelId,
            CanManage = CanManage,
            token = webhook.Token
        };
        _originalItems.Add(item);
        item.PropertyChanged += OnItemsChanged;
        UpdateList();
    }

    private async Task PermissionUpdate()
    {
        CanManage = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ChannelPermission.ManageWebhooks);
        foreach (var i in _originalItems)
        {
            i.CanManage = CanManage;
        }
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
                .Where(item => item.Name.Contains(SearchString, StringComparison.OrdinalIgnoreCase))
                .ToList();

            Items.AddRange(filteredItems);
        }
        UpdateTotal();
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
    private ObservableCollection<WebhookListItem> _items;
}

public partial class WebhookListItem(ServiceManager services, Action<RestWebhook> openInfo) : ObservableObject
{
    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _id;

    public string token;

    public string channelId;

    [ObservableProperty]
    private bool _canManage;

    [RelayCommand]
    public async Task OpenWebhook()
    {
        services.Dialogs.Create(new CreateNameDialog(), new CreateNameDialogModel { Name = Name }, "Edit Webhook").WithSubmit(SubmitWebhook).Open();
        //try
        //{
        //    var webhook = await services.Rest.GetWebhookAsync(channelId, Id);
        //    if (webhook == null)
        //        return;

        //    openInfo.Invoke(webhook);
        //}
        //catch { }
    }

    public async Task SubmitWebhook(UserControl control)
    {
        CreateNameDialogModel? model = control.DataContext as CreateNameDialogModel;
        if (model.Name == null)
            model.Name = "";

        try
        {
            await services.Rest.EditWebhookAsync(services.State.Socket.CurrentChannel?.Id, Id, new EditWebhookRequest
            {
                Name = model.Name ?? "",
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
    public void CopyLink()
    {
        string Url = ServiceManager.IsDev ? "https://localhost:7216/" : "https://lunar.fluxpoint.dev/api/";
        services.CopyText($"{Url}webhooks/{Id}/{token}");
    }

    [RelayCommand]
    public async Task DeleteWebhook()
    {
        try
        {
            await services.Rest.DeleteWebhookAsync(services.State.Socket.CurrentChannel?.Id, Id);
        }
        catch { }
    }
}
