using CommunityToolkit.Mvvm.ComponentModel;
using LunarChatApp.Services;
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
        source = new Uri(ServiceManager.IsDev ? $"https://localhost:7216/attachments/{attachment.Id}/{attachment.FileName}" :
            $"https://lunar.fluxpoint.dev/api/attachments/{attachment.Id}/{attachment.FileName}");
    }
}
