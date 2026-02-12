using Avalonia.Controls;

namespace LunarChatApp;

public partial class EmojisMenu : UserControl
{
    public EmojisMenu()
    {
        InitializeComponent();
    }

    public ulong? ReactionMessage;
}