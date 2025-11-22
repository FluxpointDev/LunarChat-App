using Avalonia;
using Avalonia.Controls;
using LunarChatApp.Shared.Core.Users;

namespace LunarChatApp;

public partial class FriendListItem : UserControl
{
    public FriendListItem()
    {
        InitializeComponent();
    }

    public FriendListItem(User user)
    {
        InitializeComponent();
        DisplayName = user.DisplayName;
        UserName = user.Username;
    }

    public static readonly StyledProperty<string> DisplayNameProperty = AvaloniaProperty.Register<ChannelItem, string>(nameof(DisplayName));

    public string DisplayName
    {
        get { return GetValue(DisplayNameProperty); }
        set { SetValue(DisplayNameProperty, value); }
    }

    public static readonly StyledProperty<string> UserNameProperty = AvaloniaProperty.Register<ChannelItem, string>(nameof(UserName));

    public string UserName
    {
        get { return GetValue(UserNameProperty); }
        set { SetValue(UserNameProperty, value); }
    }
}