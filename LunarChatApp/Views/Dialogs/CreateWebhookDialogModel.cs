using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LunarChatApp.Views.Dialogs;

public partial class CreateWebhookDialogModel(ServiceManager services) : ViewModelBase
{
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private Bitmap icon;

    [RelayCommand]
    public async Task SetIcon()
    {
        var files = await services.FilePicker();
        if (!files.Any())
            return;

        _ = Task.Run(async () =>
        {
            using (Stream stream = await files.First().OpenReadAsync())
            {
                Icon = new Bitmap(stream);
            }
        });
    }

    [RelayCommand]
    public async Task ClipboardIcon()
    {
        var topLevel = TopLevel.GetTopLevel(services.MainControl)!;

        var data = await topLevel?.Clipboard?.TryGetDataAsync();
        if (data == null)
            return;

        try
        {
            var item = data.Items.FirstOrDefault();
            if (item == null)
                return;

            var file = await item.TryGetBitmapAsync();
            if (file == null)
                return;

            Icon = file;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
        finally
        {
            data.Dispose();
        }
    }
}
