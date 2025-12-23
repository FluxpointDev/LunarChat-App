using LibVLCSharp.Shared;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LunarChatApp.Services;

public partial class MediaService
{
    public LibVLC VLC;
    private MediaPlayer currentPlayer;
    public MediaService()
    {
        if (!OperatingSystem.IsBrowser())
        {
            Core.Initialize();
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
        if (OperatingSystem.IsBrowser())
            JSRuntime.PlaySound(name);
        else
        {
            try
            {
                string Path = AppDomain.CurrentDomain.BaseDirectory + $"wwwroot\\media\\{name}.mp3";
                var currentSound = new Media(VLC, new Uri(Path));
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
