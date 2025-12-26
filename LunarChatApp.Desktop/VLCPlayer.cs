using Avalonia.Controls;
using LibVLCSharp.Avalonia;
using LibVLCSharp.Shared;
using LunarChatApp.Services;
using System;

namespace LunarChatApp.Desktop;

public class VLCPlayer : IMediaPlayer
{
    private MediaPlayer MainMediaPlayer { get; set; }

    public Control CreateControl()
    {
        // Create player view
        MainMediaPlayer = new(MediaService.VLC);

        // Create player control
        VideoView videoView = new()
        {
            MediaPlayer = MainMediaPlayer
        };

        return videoView;
    }

    public void Play(string uri)
    {
        // Create media
        var media = new Media(MediaService.VLC, new Uri(uri));

        // Play media
        MainMediaPlayer.Media = media;
        MainMediaPlayer.Play();
    }

    public void Stop()
    {
        MainMediaPlayer.Stop();
    }
}