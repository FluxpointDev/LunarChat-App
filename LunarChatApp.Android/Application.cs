using Android.App;
using Android.Runtime;
using Avalonia;
using Avalonia.Android;
using LunarChatApp.Services;

namespace LunarChatApp.Android;

[Application]
public class Application : AvaloniaAndroidApplication<App>
{
    protected Application(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
    {
    }


    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        MediaService.VideoPlayer = new ExoPlayer();
        return base.CustomizeAppBuilder(builder)
            .With(new SkiaOptions { MaxGpuResourceSizeBytes = 8096000 })
            .WithInterFont();
    }
}