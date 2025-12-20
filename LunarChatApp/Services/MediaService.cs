using LibVLCSharp.Shared;
using System;
using System.Diagnostics;

namespace LunarChatApp.Services;

public class MediaService
{
    public LibVLC VLC;

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

    private void VLC_Log(object? sender, LogEventArgs e)
    {
        Debug.WriteLine(e.FormattedLog);
    }
}
