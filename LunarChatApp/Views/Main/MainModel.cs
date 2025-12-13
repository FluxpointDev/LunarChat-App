using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Utility;
using LunarChatApp.ViewModels.Dialogs;
using LunarChatApp.ViewModels.Main;
using LunarChatApp.Views;
using LunarChatSharp.Core.Channels;
using LunarChatSharp.Core.Users;
using LunarChatSharp.Rest.Channels;
using LunarChatSharp.Rest.Messages;
using LunarChatSharp.Rest.Users;
using ShadUI;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace LunarChatApp.ViewModels;

public partial class MainModel : ViewModelBase
{
    private bool _disposed;
    public ServiceManager services;
    public MainModel(ServiceManager sv)
    {
        services = sv;
        _toastManager = sv.ToastManager;
        services.PageManager.OnSwitchPage += SwitchPage;
        services.Client.OnRelationAdd += RelationAdd;
        services.Client.OnMessageRecieved += MessageRecieve;
        if (SelectedPage == null)
        {
            SelectedPage = new LoginPage
            {
                DataContext = new LoginModel(services, this)
            };
            CurrentDialog = new PopupMask { DataContext = new PopupMaskModel(sv.Dialogs) { } };
            services.Dialogs.OnDialogOpen += OpenDialog;
            services.Dialogs.OnDialogClose += CloseDialog;
        }

    }

    private async Task MessageRecieve(RestChannel channel, RestMessage message)
    {
        if (channel.Type != ChannelType.Direct || message.Author.Id == services.Client.CurrentId || channel.Id == services.Socket.State.CurrentChannel?.Id)
            return;

        services.ToastManager.CreateToast(message.Author.DisplayName ?? message.Author.Username)
                 .WithContent(message.Content)
                 .WithAction("View", () =>
                 {
                     services.State.Socket.CurrentChannel = channel;
                     services.Client.OnSelectChannel?.Invoke(services.State.Socket.CurrentChannel);
                 })
                 .Show();
    }

    private async Task RelationAdd(RestRelation relation)
    {
        if (relation.Type == UserRelationType.FriendRequest)
        {
            if (relation.RequestBy == services.Client.CurrentId)
                return;

            services.ToastManager.CreateToast("Friend Request")
               .WithContent($"{relation.DisplayName ?? relation.Username} would like to add you.")
               .WithAction("View", () => { services.State.TriggerPageSelect(new FriendsList() { DataContext = new FriendsListModel(services) }); })
               .Show();
        }
        else if (relation.Type == UserRelationType.Friend)
        {
            if (relation.RequestBy != services.Client.CurrentId)
                return;

            services.ToastManager.CreateToast("Friend Added")
                 .WithContent($"{relation.DisplayName ?? relation.Username} accepted your request.")
                 .WithAction("View", () => { services.State.TriggerPageSelect(new FriendsList() { DataContext = new FriendsListModel(services) }); })
                 .Show();
        }
    }

    [ObservableProperty]
    private object? _selectedPage;

    [ObservableProperty]
    private PopupMask _currentDialog;

    [ObservableProperty]
    private string _currentRoute = "login";

    [ObservableProperty]
    private ToastManager _toastManager;

    public void OpenDialog(DialogMenu menu)
    {
        (CurrentDialog.DataContext as PopupMaskModel).SetMenu(menu);
    }

    public void CloseDialog()
    {
        (CurrentDialog.DataContext as PopupMaskModel).SetMenu(null);
    }

    private void SwitchPage(UserControl page)
    {
        if (SelectedPage != null && SelectedPage is IDisposable disposablePrevious)
        {
            disposablePrevious.Dispose();
        }
        SelectedPage = page;
    }

    private void SwitchPage(INavigable page, string route = "")
    {
        var pageType = page.GetType();
        if (string.IsNullOrEmpty(route)) route = pageType.GetCustomAttribute<PageAttribute>()?.Route ?? "dashboard";
        CurrentRoute = route;

        if (SelectedPage == page) return;

        if (SelectedPage != null && SelectedPage is IDisposable disposablePrevious)
        {
            disposablePrevious.Dispose();
        }

        //_previousPage = SelectedPage;
        SelectedPage = page;
        CurrentRoute = route;
        page.Initialize();
    }

    public ThemeMode _currentTheme;

    public ThemeMode CurrentTheme
    {
        get => _currentTheme;
        private set => SetProperty(ref _currentTheme, value);
    }

    [RelayCommand]
    private void SwitchTheme()
    {
        CurrentTheme = CurrentTheme switch
        {
            ThemeMode.System => ThemeMode.Light,
            ThemeMode.Light => ThemeMode.Dark,
            _ => ThemeMode.System
        };

        services.ThemeWatcher.SwitchTheme(CurrentTheme);
    }

    [RelayCommand]
    public void EscapeHotKey()
    {
        if (SelectedPage != null && (SelectedPage is IEscapeHotKey))
        {
            services.PageManager.OnSwitchPage(services.State.CachedServersPage);
        }
    }

    public override void Dispose()
    {
        base.Dispose();

        if (_disposed) return;

        if (SelectedPage is IDisposable disposableCurrent)
        {
            disposableCurrent.Dispose();
        }

        //if (_previousPage is IDisposable disposablePrevious)
        //{
        //    disposablePrevious.Dispose();
        //}

        //DialogManager.Dispose();

        _disposed = true;
        GC.SuppressFinalize(this);
    }

    ~MainModel()
    {
        Dispose();
    }
}
