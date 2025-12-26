using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;

namespace LunarChatApp.Views.User.Settings;

public partial class SettingsDebugModel : ViewModelBase
{
    private ServiceManager services;
    public Control player { get; set; }

    public SettingsDebugModel(ServiceManager sv)
    {
        services = sv;
        player = MediaService.VideoPlayer.CreateControl();
    }

    [RelayCommand]
    public void Play()
    {
        if (Design.IsDesignMode)
        {
            return;
        }

        try
        {
            MediaService.VideoPlayer.Play("https://lunar.fluxpoint.dev/demo/media/test_video.mp4");
        }
        catch { }
    }

    [RelayCommand]
    public void Stop()
    {
        try
        {
            MediaService.VideoPlayer.Stop();
        }
        catch { }
    }
}
