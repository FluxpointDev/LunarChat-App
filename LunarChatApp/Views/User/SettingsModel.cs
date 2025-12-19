using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.ViewModels.Settings;
using LunarChatApp.Views;
using LunarChatApp.Views.User.Settings;
using ShadUI;
using System;

namespace LunarChatApp.ViewModels;

public partial class SettingsSectionModel : ViewModelBase
{
    public TestState state { get; set; }

    public SettingsSectionModel(TestState st)
    {
        state = st;
    }
}
public partial class SettingsModel : ViewModelBase
{
    private PageManager pageManager;
    private TestState state { get; set; }
    private ThemeWatcher themeWatcher;
    private MainModel main;
    private ServiceManager services;
    public SettingsModel(ServiceManager sv, MainModel mainModel)
    {
        services = sv;
        pageManager = sv.PageManager;
        state = sv.State;
        themeWatcher = sv.ThemeWatcher;
        main = mainModel;
        if (SelectedPage == null)
            SelectedPage = new SettingsAccount();

        devMode = ServiceManager.IsDev;
    }

    [ObservableProperty]
    private bool devMode;

    [ObservableProperty]
    private UserControl? _selectedPage;


    [ObservableProperty]
    public string? _selectedTitle = "Account";

    [RelayCommand]
    public void CloseSettings()
    {
        pageManager.OnSwitchPage(state.CachedServersPage);
    }

    [RelayCommand]
    private void OpenAccount()
    {
        SwitchPage(SettingsPageType.Account);
    }

    [RelayCommand]
    private void OpenProfile()
    {
        SwitchPage(SettingsPageType.Profile);
    }

    [RelayCommand]
    private void OpenConnections()
    {
        SwitchPage(SettingsPageType.Connections);
    }

    [RelayCommand]
    private void OpenTheme()
    {
        SwitchPage(SettingsPageType.Theme);
    }

    [RelayCommand]
    private void OpenChat()
    {
        SwitchPage(SettingsPageType.Chat);
    }

    [RelayCommand]
    public void OpenSocial()
    {
        SwitchPage(SettingsPageType.Social);
    }

    [RelayCommand]
    private void OpenNotifications()
    {
        SwitchPage(SettingsPageType.Notifications);
    }

    [RelayCommand]
    private void OpenDeveloper()
    {
        SwitchPage(SettingsPageType.Developer);
    }

    [RelayCommand]
    private void OpenDebug()
    {
        SwitchPage(SettingsPageType.Debug);
    }

    [RelayCommand]
    private void OpenStreamerMode()
    {
        SwitchPage(SettingsPageType.StreamerMode);
    }

    private void SwitchPage(SettingsPageType pageType)
    {
        if (SelectedPage != null && SelectedPage is IDisposable disposablePrevious)
            disposablePrevious.Dispose();

        SelectedTitle = pageType.ToString();

        switch (pageType)
        {
            case SettingsPageType.Account:
                SelectedPage = new SettingsAccount();
                break;
            case SettingsPageType.Profile:
                SelectedPage = new SettingsProfile
                {
                    DataContext = new SettingsProfileModel(services)
                };
                break;
            case SettingsPageType.Social:
                SelectedPage = new SettingsSocial
                {
                    DataContext = new SettingsSocialModel(services)
                };
                break;
            case SettingsPageType.Connections:
                SelectedPage = new SettingsConnections();
                break;
            case SettingsPageType.Theme:
                SelectedPage = new SettingsTheme()
                {
                    DataContext = new SettingsThemeModel(state, themeWatcher, main)
                };
                break;
            case SettingsPageType.Chat:
                SelectedPage = new SettingsChat();
                break;
            case SettingsPageType.Notifications:
                SelectedPage = new SettingsNotifications();
                break;
            case SettingsPageType.Developer:
                SelectedPage = new SettingsDeveloper() { DataContext = new SettingsDeveloperModel(services) };
                break;
            case SettingsPageType.Debug:
                SelectedPage = new SettingsDebug() { DataContext = new SettingsDebugModel(services) };
                break;
            case SettingsPageType.StreamerMode:
                SelectedPage = new SettingsStreamerMode() { DataContext = new SettingsStreamerModeModel(services) };
                break;
        }
    }
}
public enum SettingsPageType
{
    Account, Profile, Connections, Theme, Chat, Notifications, Developer, Social, StreamerMode, Debug
}
