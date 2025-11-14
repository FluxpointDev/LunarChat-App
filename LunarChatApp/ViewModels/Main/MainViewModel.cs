using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using ShadUI;
using System;
using System.Reflection;

namespace LunarChatApp.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private bool _disposed;
    private ThemeWatcher themeWatcher;
    private PageManager pageManager;

    public MainViewModel(ServiceProvider services)
    {
        themeWatcher = services.GetService<ThemeWatcher>();
        pageManager = services.GetService<PageManager>();
        var rest = services.GetService<RestClient>();
        var state = services.GetService<TestState>();
        pageManager.OnSwitchPage += SwitchPage;
        if (SelectedPage == null)
            SelectedPage = new LoginPage
            {
                DataContext = new LoginViewModel(pageManager, rest, state)
            };
    }

    [ObservableProperty]
    private DialogManager _dialogManager;

    [ObservableProperty]
    private object? _selectedPage;

    [ObservableProperty]
    private string _currentRoute = "login";

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

        if (_selectedPage != null && _selectedPage is IDisposable disposablePrevious)
        {
            disposablePrevious.Dispose();
        }

        //_previousPage = SelectedPage;
        SelectedPage = page;
        CurrentRoute = route;
        page.Initialize();
    }

    private ThemeMode _currentTheme;

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

        themeWatcher.SwitchTheme(CurrentTheme);
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

        //_disposed = true;
        GC.SuppressFinalize(this);
    }

    ~MainViewModel()
    {
        Dispose();
    }
}
