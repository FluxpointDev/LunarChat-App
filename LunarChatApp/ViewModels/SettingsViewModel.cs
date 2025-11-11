using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using System;

namespace LunarChatApp.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private PageManager pageManager;

    public SettingsViewModel(PageManager page)
    {
        pageManager = page;
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
            DataContext = new ServersViewModel(pageManager)
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
                SelectedPage = new SettingsProfile();
                break;
            case SettingsPageType.Connections:
                SelectedPage = new SettingsConnections();
                break;
            case SettingsPageType.Theme:
                SelectedPage = new SettingsTheme();
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
