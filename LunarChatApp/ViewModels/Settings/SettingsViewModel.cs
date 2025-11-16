using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.ViewModels.Settings;
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
public partial class SettingsViewModel : ViewModelBase
{
    private PageManager pageManager;
    private TestState state { get; set; }
    private ThemeWatcher themeWatcher;
    private MainViewModel main;

    public SettingsViewModel(PageManager page, TestState st, ThemeWatcher theme, MainViewModel mainModel)
    {
        pageManager = page;
        state = st;
        themeWatcher = theme;
        main = mainModel;
        if (SelectedPage == null)
            SelectedPage = new SettingsAccount();
    }

    [ObservableProperty]
    private UserControl? _selectedPage;


    [ObservableProperty]
    public string? _selectedTitle = "Account";

    [RelayCommand]
    public void CloseSettings()
    {
        pageManager.OnSwitchPage(new ServersPage
        {
            DataContext = new ServersViewModel(pageManager, state, themeWatcher, main, main.rest)
        });
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
    private void OpenNotifications()
    {
        SwitchPage(SettingsPageType.Notifications);
    }

    [RelayCommand]
    private void OpenDeveloper()
    {
        SwitchPage(SettingsPageType.Developer);
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
                    DataContext = new SettingsSectionModel(state)
                };
                break;
            case SettingsPageType.Connections:
                SelectedPage = new SettingsConnections();
                break;
            case SettingsPageType.Theme:
                SelectedPage = new SettingsTheme()
                {
                    DataContext = new SettingsThemeViewModel(state, themeWatcher, main)
                };
                break;
            case SettingsPageType.Chat:
                SelectedPage = new SettingsChat();
                break;
            case SettingsPageType.Notifications:
                SelectedPage = new SettingsNotifications();
                break;
            case SettingsPageType.Developer:
                SelectedPage = new SettingsDeveloper();
                break;
        }
    }
}
public enum SettingsPageType
{
    Account, Profile, Connections, Theme, Chat, Notifications, Developer
}
