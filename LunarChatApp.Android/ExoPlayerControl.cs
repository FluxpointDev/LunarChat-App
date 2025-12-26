using AndroidX.Media3.ExoPlayer;
using Avalonia.Android;
using Avalonia.Controls;
using Avalonia.Platform;

namespace LunarChatApp.Android;

public class ExoPlayerControl : NativeControlHost
{
    public IExoPlayer MainExoPlayer { get; private set; }

    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
    {
        var parentContext = (parent as AndroidViewControlHandle)?.View.Context ?? global::Android.App.Application.Context;

        // Create player
        ExoPlayerBuilder exoPlayerBuilder = new(parentContext);
        MainExoPlayer = exoPlayerBuilder.Build();

        // Create player view
        AndroidX.Media3.UI.PlayerView exoPlayerView = new(parentContext)
        {
            Player = MainExoPlayer
        };

        return new AndroidViewControlHandle(exoPlayerView);
    }
}