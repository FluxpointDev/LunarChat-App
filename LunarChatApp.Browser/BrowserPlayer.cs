using Avalonia.Browser;
using Avalonia.Controls;
using Avalonia.Platform;
using LunarChatApp.Services;
using System.Runtime.InteropServices.JavaScript;

namespace LunarChatApp.Browser;

internal class BrowserControl : NativeControlHost
{
    internal JSObject iframe;
    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
    {
        iframe = JSRuntime.CreateElement("iframe");
        iframe.SetProperty("frameBorder", 0);
        return new JSObjectControlHandle(iframe);
    }

    protected override void DestroyNativeControlCore(IPlatformHandle control)
    {
        base.DestroyNativeControlCore(control);
    }
}

public class BrowserPlayer : IMediaPlayer
{
    internal BrowserControl control;
    public Control CreateControl()
    {
        control = new BrowserControl();
        return control;
    }

    public void Play(string uri)
    {
        control.iframe.SetProperty("src", "https://lunar.fluxpoint.dev/demo/media/test_video.mp4");
    }

    public void Stop()
    {
        control.iframe.SetProperty("src", "");
    }
}