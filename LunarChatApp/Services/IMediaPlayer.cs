using Avalonia.Controls;

namespace LunarChatApp.Services;

public interface IMediaPlayer
{
    Control CreateControl();
    void Play(string uri);
    void Stop();
}
