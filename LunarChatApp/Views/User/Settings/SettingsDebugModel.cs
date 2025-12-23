using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using LunarChatApp.Services;
using System;
using System.Threading.Tasks;

namespace LunarChatApp.Views.User.Settings;

public partial class SettingsDebugModel : ViewModelBase
{
    private ServiceManager services;
    private readonly LibVLC vlc;
    public MediaPlayer player { get; set; }

    public SettingsDebugModel(ServiceManager sv)
    {
        services = sv;
        vlc = sv.MediaService.VLC;
        player = new MediaPlayer(vlc);
    }

    [RelayCommand]
    public void PlaySound()
    {
        _ = Task.Run(() =>
        {
            try
            {
                string Path = System.AppDomain.CurrentDomain.BaseDirectory + "Assets\\Sounds\\notification.mp3";
                var media = new Media(vlc, new Uri(Path));
                var mediaplayer = new MediaPlayer(vlc);
                mediaplayer.Play(media);
            }
            catch (Exception ex)
            {

            }
        });
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
            using var media = new Media(vlc, new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "Assets\\Sounds\\test_video.mp4"));
            player.Play(media);
        }
        catch { }
    }

    [RelayCommand]
    public void Stop()
    {
        try
        {
            player.Stop();
        }
        catch { }
    }
}
