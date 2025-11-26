using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Shared.Core.Channels;
using LunarChatApp.Views;
using System.Threading.Tasks;

namespace LunarChatApp.ViewModels.Dialogs;

public partial class CreateChannelDialogModel(ServiceManager services) : ViewModelBase
{
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private ChannelType type;

    [RelayCommand]
    public async Task Submit()
    {

    }
}
