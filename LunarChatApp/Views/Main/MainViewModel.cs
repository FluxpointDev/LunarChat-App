using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.ViewModels.Dialogs;
using LunarChatApp.Views;
using ShadUI;
using System;
using System.Reflection;

namespace LunarChatApp.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private bool _disposed;
    private ServiceManager services;
    public MainViewModel(ServiceManager sv)
    {
        services = sv;
        services.PageManager.OnSwitchPage += SwitchPage;
        if (SelectedPage == null)
        {
            SelectedPage = new LoginPage
            {
                DataContext = new LoginViewModel(services, this)
            };
            CurrentDialog = new PopupMask { DataContext = new PopupMaskModel(sv.Dialogs) { } };
            services.Dialogs.OnDialogOpen += OpenDialog;
            services.Dialogs.OnDialogClose += CloseDialog;
        }

    }

    [ObservableProperty]
    private object? _selectedPage;

    [ObservableProperty]
    private PopupMask _currentDialog;

    [ObservableProperty]
    private string _currentRoute = "login";

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
        if (SelectedPage != null && (SelectedPage!.GetType() == typeof(SettingsPage) || SelectedPage!.GetType() == typeof(ServerSettings)))
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

    ~MainViewModel()
    {
        Dispose();
    }
}
