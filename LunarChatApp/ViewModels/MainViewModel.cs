using CommunityToolkit.Mvvm.ComponentModel;
using LunarChatApp.Services;
using ShadUI;
using System;
using System.Reflection;

namespace LunarChatApp.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private DialogManager _dialogManager;

    [ObservableProperty]
    private object? _selectedPage = new SettingsPage
    {
        DataContext = new SettingsViewModel()
    };

    [ObservableProperty]
    private string _currentRoute = "servers";

    private void SwitchPage(INavigable page, string route = "")
    {
        var pageType = page.GetType();
        if (string.IsNullOrEmpty(route)) route = pageType.GetCustomAttribute<PageAttribute>()?.Route ?? "dashboard";
        CurrentRoute = route;

        if (SelectedPage == page) return;

        //if (_previousPage is IDisposable disposablePrevious)
        //{
        //    disposablePrevious.Dispose();
        //}

        if (_selectedPage != null && _selectedPage is IDisposable disposablePrevious)
        {
            disposablePrevious.Dispose();
        }

        //_previousPage = SelectedPage;
        SelectedPage = page;
        CurrentRoute = route;
        page.Initialize();
    }
}
