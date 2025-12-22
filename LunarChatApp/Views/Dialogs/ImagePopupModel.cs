using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.ViewModels;
using System;
using System.Threading.Tasks;

namespace LunarChatApp.Views.Dialogs;

public partial class ImagePopupModel : ViewModelBase
{
    public ImagePopupModel(ServiceManager sv, Uri source)
    {
        services = sv;
        Source = source;
    }

    [ObservableProperty]
    private Uri source;

    private ServiceManager services;

    [RelayCommand]
    public async Task Save()
    {

    }

    [RelayCommand]
    public void OpenLink()
    {
        services.OpenLink(source);
    }

    [RelayCommand]
    public void Close()
    {
        MainView? view = null;
        if (services.MainControl is MainWindow window)
            view = window.Content as MainView;
        else
            view = services.MainControl as MainView;

        if (view == null)
            return;

        (view.DataContext as MainModel)!.CurrentImage = null;
    }
}
