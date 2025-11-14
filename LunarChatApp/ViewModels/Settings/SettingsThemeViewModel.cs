using CommunityToolkit.Mvvm.ComponentModel;
using ShadUI;

namespace LunarChatApp.ViewModels.Settings;

public partial class SettingsThemeViewModel : SettingsSectionModel
{
    private ThemeWatcher themeWatcher;
    private MainViewModel main;
    public SettingsThemeViewModel(TestState st, ThemeWatcher theme, MainViewModel mainModel) : base(st)
    {
        themeWatcher = theme;
        main = mainModel;
        switch (mainModel.CurrentTheme)
        {
            case ThemeMode.Dark:
                SelectedIndex = 1;
                break;
            case ThemeMode.Light:
                SelectedIndex = 2;
                break;
        }
    }

    [ObservableProperty]
    private int _selectedIndex;

    private object? _selectedItem;

    public object? SelectedItem
    {
        get => _selectedItem;
        set { SetProperty(ref _selectedItem, value); ChangeThemeMode(); }
    }

    public bool FirstTrigger = false;

    public void ChangeThemeMode()
    {
        if (!FirstTrigger)
        {
            FirstTrigger = true;
            return;
        }

        switch (SelectedIndex)
        {
            case 0:
                {
                    main._currentTheme = ThemeMode.System;
                    themeWatcher.SwitchTheme(ThemeMode.System);
                }
                break;
            case 1:
                main._currentTheme = ThemeMode.Dark;
                themeWatcher.SwitchTheme(ThemeMode.Dark);
                break;
            case 2:
                main._currentTheme = ThemeMode.Light;
                themeWatcher.SwitchTheme(ThemeMode.Light);
                break;
        }
    }
}
