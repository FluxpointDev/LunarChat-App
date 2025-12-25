using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.ViewModels;
using LunarChatApp.Views.Dialogs;
using LunarChatSharp.Rest.Messages;
using System;

namespace LunarChatApp.Views.Messages;

public partial class ImageItemModel : ViewModelBase
{
    [ObservableProperty]
    private Uri source;

    private ServiceManager services;

    public ImageItemModel(ServiceManager sv, RestAttachment attachment)
    {
        services = sv;
        Source = new Uri(ServiceManager.IsDev ? $"https://localhost:7216/attachments/{attachment.Id}/{attachment.FileName}" :
            $"https://lunar.fluxpoint.dev/api/attachments/{attachment.Id}/{attachment.FileName}");
    }

    [RelayCommand]
    public void OpenImage()
    {
        MainView? view = services.GetMainView();

        if (view == null)
            return;

        (view.DataContext as MainModel)!.CurrentImage = new ImagePopup() { DataContext = new ImagePopupModel(services, Source) };
    }
}
