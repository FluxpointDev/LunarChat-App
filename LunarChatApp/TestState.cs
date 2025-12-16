using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using LunarChatSharp.Core.Users;
using LunarChatSharp.Websocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LunarChatApp;


public partial class TestState : ObservableObject
{
    public string? CleanUsername(string username)
    {
        char[] characters = username.ToLower().Where(x =>
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
    public SocketState Socket;

    [ObservableProperty]
    private string _username;

    [ObservableProperty]
    private string _currentDisplayName;

    [ObservableProperty]
    private string? _displayName;

    public string? AboutMe { get; set; }

    public delegate void PageEventHandler(UserControl control);
    public event PageEventHandler? OnPageSelect;
    public Func<bool?, Task>? OnExpandChannels;

    public void TriggerPageSelect(UserControl control)
    {
        OnPageSelect?.Invoke(control);
    }
}