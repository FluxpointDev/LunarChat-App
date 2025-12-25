using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LunarChatApp.Services;

public partial class MediaService
{
    public LibVLC VLC;
    private MediaPlayer currentPlayer;
    public Dictionary<string, Media> Sounds;
    public MediaService()
    {
        if (!OperatingSystem.IsBrowser())
        {
            //Core.Initialize();
            Sounds = new Dictionary<string, Media>();
            VLC = new LibVLC(enableDebugLogs: ServiceManager.IsDev);
            if (ServiceManager.IsDev)
            {
                VLC.Log += VLC_Log;
            }
        }
    }

    internal void StopSound()
    {
        if (OperatingSystem.IsBrowser())
            JSRuntime.StopSound();
        else
        {
            if (currentPlayer != null)
                currentPlayer.Stop();
        }
    }

    internal async Task PlaySoundAsync(string name)
    {
        string Url = $"https://lunar.fluxpoint.dev/demo/{name}.mp3";
        if (OperatingSystem.IsBrowser())
        {
            try
            {
                JSRuntime.PlaySound(name);
            }
            catch { }
        }
        else
        {
            if (!Sounds.TryGetValue(name, out Media? media))
            {
                media = new Media(VLC, Url);
                Sounds.Add(name, media);
            }
            try
            {
                var currentSound = new Media(VLC, new Uri(Url));
                if (currentPlayer != null)
                {
                    currentPlayer.Stop();
                    currentPlayer.Dispose();
                }
                currentPlayer = new MediaPlayer(VLC);
                currentPlayer.Play(currentSound);
            }
            catch (Exception ex)
            {

            }
        }
    }

    private void VLC_Log(object? sender, LogEventArgs e)
    {
        Debug.WriteLine(e.FormattedLog);
    }
}
