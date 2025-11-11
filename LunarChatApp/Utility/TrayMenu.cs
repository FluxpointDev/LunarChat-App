using Avalonia;
using Avalonia.Controls;
using LunarChatApp.ViewModels;
using LunarChatApp.Views;
using System;
using System.Linq;

namespace LunarChatApp.Utility;

public static class TrayMenu
{
    public static void RegisterTrayIconsEvents(this Application app, MainWindow window, MainViewModel viewModel)
    {
        var trayIcons = TrayIcon.GetIcons(app);
        if (trayIcons is null || !trayIcons.Any()) return;
        var trayIcon = trayIcons[0];

        if (trayIcon.Menu is null)
            return;

        trayIcon.Clicked += (_, _) =>
        {
            if (window.WindowState == WindowState.Minimized)
            {
                window.RestoreWindowState();
                window.Show();
            }

            window.Activate();
        };

        var items = trayIcon.Menu.Items.OfType<NativeMenuItem>().ToList();
        var openMenu = items.First(x =>
            x.Header != null && x.Header.Contains("Open", StringComparison.CurrentCultureIgnoreCase));
        //var aboutMenu = items.First(x =>
        //    x.Header != null && x.Header.Contains("About", StringComparison.CurrentCultureIgnoreCase));
        var exitMenu = items.First(x =>
            x.Header != null && x.Header.Contains("Exit", StringComparison.CurrentCultureIgnoreCase));

        openMenu.Click += (_, _) =>
        {
            if (window.WindowState == WindowState.Minimized)
            {
                window.RestoreWindowState();
                window.Show();
            }

            window.Activate();
        };

        //aboutMenu.Click += (_, _) =>
        //{
        //    if (window.WindowState == WindowState.Minimized)
        //    {
        //        window.RestoreWindowState();
        //        window.Show();
        //    }

        //    window.Activate();
        //    viewModel.ShowAboutCommand.Execute(null);
        //};

        exitMenu.Click += (_, _) =>
        {
            if (window.WindowState == WindowState.Minimized)
            {
                window.RestoreWindowState();
                window.Show();
            }

            window.Activate();
            Environment.Exit(0);
        };
    }
}
