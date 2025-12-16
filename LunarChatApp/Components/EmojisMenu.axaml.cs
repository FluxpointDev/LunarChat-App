using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace LunarChatApp;

public partial class EmojisMenu : UserControl
{
    public EmojisMenu()
    {
        InitializeComponent();
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            var Top = TopLevel.GetTopLevel(this);
            ChannelView Channel = (Top.Parent.Parent.Parent.Parent.Parent.Parent as ChannelView);

            int StartIndex = Channel.ChatBox.SelectionStart;
            if (Channel.ChatBox.Text == null)
            {
                Channel.ChatBox.Text += (e.Source as Button).Content as string;
                Channel.ChatBox.SelectionStart = 2;
                Channel.ChatBox.SelectionEnd = 2;

            }
            else
            {
                Channel.ChatBox.Text = Channel.ChatBox.Text.Insert(Channel.ChatBox.SelectionStart, (e.Source as Button).Content as string);
                Channel.ChatBox.SelectionStart = StartIndex + 2;
                Channel.ChatBox.SelectionEnd = StartIndex + 2;
            }
            Channel.ChatBox.Focus();
            (Top.Parent as Popup).Close();
        }
        catch { }
    }
}