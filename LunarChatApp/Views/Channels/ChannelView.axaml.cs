using Avalonia.Controls;
using LunarChatApp.Services;
using System;
using System.Threading.Tasks;

namespace LunarChatApp;

public partial class ChannelView : UserControl
{
    private readonly ServiceManager services;
    public ChannelView(ServiceManager sv)
    {
        services = sv;
        InitializeComponent();
        services.State.OpenEmojiMenu += OpenEmojiMenu;
        //EmojiMenuButton.Flyout.Closed += EmojiMenuClosed;
    }

    private void EmojiMenuClosed(object? sender, EventArgs e)
    {
        services.State.EmojisMenu.ReactionMessage = null;
    }

    private async Task OpenEmojiMenu()
    {
        EmojiMenuButton.Flyout!.ShowAt(EmojiMenuButton);
    }
}