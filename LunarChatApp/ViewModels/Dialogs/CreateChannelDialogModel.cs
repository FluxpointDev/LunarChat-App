using CommunityToolkit.Mvvm.ComponentModel;
using LunarChatApp.Shared.Core.Channels;

namespace LunarChatApp.ViewModels.Dialogs;

public partial class CreateChannelDialogModel : ViewModelBase
{
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private ChannelType type;
}
