using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using LucideAvalonia.Enum;
using LunarChatApp.Shared.Core.Channels;

namespace LunarChatApp;

public partial class ChannelItem : UserControl
{
    public ChannelItem()
    {
        InitializeComponent();
    }
    private ChannelType _channelType;
    public ChannelType ChannelType { get { return _channelType; } set { _channelType = value; Test.Icon = GetIcon(); Test.StrokeBrush = new SolidColorBrush(Color.Parse("#ffffff")); } }

    public static readonly StyledProperty<string> ChannelNameProperty = AvaloniaProperty.Register<ChannelItem, string>(nameof(ChannelName));

    public string ChannelName
    {
        get { return GetValue(ChannelNameProperty); }
        set { SetValue(ChannelNameProperty, value); }
    }

    public static readonly StyledProperty<LucideIconNames> ChannelIconProperty = AvaloniaProperty.Register<ChannelItem, LucideIconNames>(nameof(ChannelIcon), LucideIconNames.Hash);


    public LucideIconNames ChannelIcon
    {
        get { return GetValue(ChannelIconProperty); }
        set
        {
            SetValue(ChannelIconProperty, value);
        }
    }

    private LucideIconNames GetIcon()
    {
        switch (ChannelType)
        {
            case ChannelType.Voice:
                return LucideIconNames.Volume2;
            case ChannelType.Media:
                return LucideIconNames.Image;
            case ChannelType.Schedule:
                return LucideIconNames.Calendar;
            case ChannelType.Rules:
                return LucideIconNames.BookCheck;
        }
        return LucideIconNames.Hash;
    }

    private void Clicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
    }
}