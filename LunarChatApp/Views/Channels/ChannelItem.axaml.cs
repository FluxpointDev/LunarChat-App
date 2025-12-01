using Avalonia;
using Avalonia.Controls;
using LunarChatSharp.Core.Channels;
using Material.Icons;

namespace LunarChatApp;

public partial class ChannelItem : UserControl
{
    public ChannelItem()
    {
        InitializeComponent();
    }
    private ChannelType _channelType;
    public ChannelType ChannelType { get { return _channelType; } set { _channelType = value; Test.Kind = GetIcon(); } }

    public static readonly StyledProperty<string> ChannelNameProperty = AvaloniaProperty.Register<ChannelItem, string>(nameof(ChannelName));

    public string ChannelName
    {
        get { return GetValue(ChannelNameProperty); }
        set { SetValue(ChannelNameProperty, value); }
    }

    public static readonly StyledProperty<MaterialIconKind> ChannelIconProperty = AvaloniaProperty.Register<ChannelItem, MaterialIconKind>(nameof(ChannelIcon), MaterialIconKind.Hashtag);


    public MaterialIconKind ChannelIcon
    {
        get { return GetValue(ChannelIconProperty); }
        set
        {
            SetValue(ChannelIconProperty, value);
        }
    }

    private MaterialIconKind GetIcon()
    {
        switch (ChannelType)
        {
            case ChannelType.Voice:
                return MaterialIconKind.VolumeHigh;
                //case ChannelType.Media:
                //    return MaterialIconKind.Image;
                //case ChannelType.Schedule:
                //    return MaterialIconKind.Calendar;
                //case ChannelType.Rules:
                //    return MaterialIconKind.BookCheck;
        }
        return MaterialIconKind.Hashtag;
    }

    private void Clicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
    }
}