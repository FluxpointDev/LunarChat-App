using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using LunarChatApp.Services;
using LunarChatApp.Views.Dialogs;
using LunarChatApp.Views.Dialogs.Servers;
using LunarChatSharp;
using LunarChatSharp.Core.Servers;
using LunarChatSharp.Rest.Messages;
using LunarChatSharp.Rest.Servers;
using LunarChatSharp.Websocket.Events.Servers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace LunarChatApp.Views.Servers.Settings;

public partial class ServerSettingsEmojisModel : ViewModelBase
{
    private readonly List<EmojiListItem> _originalItems = new List<EmojiListItem>();

    private readonly Timer? _searchTimer;
    private readonly ServiceManager services;
    public ServerSettingsEmojisModel(ServiceManager sv)
    {
        services = sv;
        services.Client.OnEmojiCreate += EmojiCreated;
        services.Client.OnEmojiUpdate += EmojiUpdated;
        services.Client.OnEmojiDelete += EmojiDeleted;
        bool CanManage = false;
        if (services.State.CurrentServer != null)
        {
            services.State.CurrentServer.OnPermissionUpdate += PermissionUpdate;
            _canCreate = services.State.CurrentServer.HasPermission(services.State.CurrentServer.Member, ServerPermission.CreateExpressions);
            CanManage = services.State.CurrentServer.HasPermission(services.State.CurrentServer.Member, ServerPermission.ManageExpressions);
        }

        _searchTimer = new Timer(500); // 500ms debounce
        _searchTimer.Elapsed += SearchTimerElapsed;
        _searchTimer.AutoReset = false;

        PropertyChanged += OnPropertyChanged;

        _originalItems = services.State.CurrentServer.Emojis.Values.Select(x => new EmojiListItem(services, x, CanManage)).ToList();

        Items = new ObservableCollection<EmojiListItem>(_originalItems);
    }

    private async Task PermissionUpdate(RestServer server)
    {
        if (services.State.CurrentServer == null)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            CanCreate = services.State.CurrentServer.HasPermission(services.State.CurrentServer.Member, ServerPermission.CreateExpressions);
            bool CanManage = services.State.CurrentServer.HasPermission(services.State.CurrentServer.Member, ServerPermission.ManageExpressions);
            foreach (var i in _originalItems)
            {
                i.CanManage = i.Creator == services.Client.CurrentId || CanManage;
            }
        });

    }

    [ObservableProperty]
    private bool _canCreate;

    private async Task EmojiDeleted(RestServer server, RestEmoji emoji)
    {
        EmojiListItem? item = _originalItems.FirstOrDefault(x => x.id == emoji.Id);
        if (item == null)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            _originalItems.Remove(item);
            UpdateList();
        });

    }

    private async Task EmojiUpdated(RestServer server, RestEmoji emoji, EmojiUpdateEvent ev)
    {
        EmojiListItem? item = _originalItems.FirstOrDefault(x => x.id == emoji.Id);
        if (item == null)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (ev.Name != null)
                item.Name = ev.Name!;
            UpdateList();
        });

    }

    private async Task EmojiCreated(RestServer server, RestEmoji emoji)
    {
        if (services.State.CurrentServer == null)
            return;

        bool CanManage = services.State.CurrentServer.HasPermission(services.State.CurrentServer.Member, ServerPermission.ManageExpressions);
        EmojiListItem item = new EmojiListItem(services, emoji, CanManage);
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            _originalItems.Add(item);
            UpdateList();
        });
    }

    [RelayCommand]
    public void CreateEmote()
    {
        services.Dialogs.Create(new CreateEmojiDialog(), new CreateEmojiDialogModel(services)
        {
        }, "Create Emoji").WithSubmit(SubmitEmoji).Open();
    }

    public async Task SubmitEmoji(UserControl control)
    {
        CreateEmojiDialogModel? model = control.DataContext as CreateEmojiDialogModel;
        if (model == null || services.State.CurrentServer == null || string.IsNullOrEmpty(model.Name) || model.Icon == null)
            return;

        try
        {
            using (var str = new MemoryStream())
            {
                model.Icon.Save(str);
                str.Position = 0;
                await services.Rest.CreateEmojiAsync(services.State.CurrentServer.Server.Id, new CreateEmojiRequest
                {
                    Name = model.Name,
                    Icon = Utils.GetImageBase64(str)
                });
            }
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
    }


    [ObservableProperty]
    private string _searchString = string.Empty;

    [ObservableProperty]
    private bool _isSearching;

    [ObservableProperty]
    private ObservableCollection<EmojiListItem> _items;

}
public partial class EmojiListItem : ObservableObject
{
    private readonly ServiceManager services;
    public EmojiListItem(ServiceManager sv, RestEmoji emoji, bool canManage)
    {
        services = sv;
        id = emoji.Id;
        _creator = emoji.CreatedBy;
        _name = emoji.Name;
        _canManage = emoji.CreatedBy == services.Client.CurrentId || canManage;
        Icon = new Uri(emoji.GetIconUrl()!);
    }

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private Uri icon;

    public string id;

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
        if (model == null)
            return;

        if (services.State.CurrentServer == null)
            return;


        try
        {
            await services.Rest.EditEmojiAsync(services.State.CurrentServer.Server.Id, id, new EditEmojiRequest
            {
                Name = model.Name
            });
        }
        catch { }
    }

    [RelayCommand]
    public void CopyId()
    {
        services.CopyText(id);
    }

    [RelayCommand]
    public async Task DeleteEmoji()
    {
        if (services.State.CurrentServer == null)
            return;

        try
        {
            await services.Rest.DeleteEmojiAsync(services.State.CurrentServer.Server.Id, id);
        }
        catch { }
    }
}