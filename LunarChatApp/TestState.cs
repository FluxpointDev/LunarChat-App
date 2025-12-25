using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using LunarChatApp.Components;
using LunarChatSharp.Core.Users;
using LunarChatSharp.Rest.Channels;
using LunarChatSharp.Websocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LunarChatApp;


public partial class TestState : ObservableObject
{
    public string? CleanUsername(string username)
    {
        char[] characters = username.ToLower().Where(static x =>
        {
            if (x == '_' || x == '.' || char.IsLetterOrDigit(x))
                return true;
            return false;
        }).ToArray();

        if (characters.Length == 0)
            return null;

        if (characters.Length > 32)
            return string.Join("", characters).Substring(0, 32);

        return string.Join("", characters);
    }

    public UserStatusType StatusType;
    public string? StatusText;
    public ServersPage? CachedServersPage;
    public SocketServerState? CurrentServer;
    public RestChannel? CurrentChannel;
    public SocketState Socket;

    [ObservableProperty]
    private string _username;

    [ObservableProperty]
    private string _currentDisplayName;

    [ObservableProperty]
    private EmojisMenu emojisMenu;

    [ObservableProperty]
    private string? _displayName;
    public string? AboutMe { get; set; }

    [ObservableProperty]
    private Bitmap? avatar;

    public delegate void PageEventHandler(UserControl control);
    public event PageEventHandler? OnPageSelect;
    public Func<bool?, Task>? OnExpandChannels;
    public Func<Task>? OpenEmojiMenu;
    public Func<EmojiListItemModel, Task>? UseEmoji;

    public void TriggerPageSelect(UserControl control)
    {
        OnPageSelect?.Invoke(control);
    }
}