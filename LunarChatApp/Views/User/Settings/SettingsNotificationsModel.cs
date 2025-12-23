using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using System.Threading.Tasks;

namespace LunarChatApp.Views.User.Settings;

public partial class SettingsNotificationsModel : ViewModelBase
{
    private readonly ServiceManager services;
    public SettingsNotificationsModel(ServiceManager sv)
    {
        services = sv;
    }

    [RelayCommand]
    public async Task PlayMessageSound()
    {
        await services.MediaService.PlaySoundAsync("notification");
    }

    [RelayCommand]
    public async Task PlayCallSound()
    {
        await services.MediaService.PlaySoundAsync("call_sound");
    }
}
