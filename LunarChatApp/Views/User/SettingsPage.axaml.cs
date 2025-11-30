using Avalonia.Controls;
using LunarChatApp.Services;
using LunarChatApp.Utility;

namespace LunarChatApp;

[Page("settings")]
public partial class SettingsPage : UserControl, IEscapeHotKey
{
    public SettingsPage()
    {
        InitializeComponent();
    }
}