using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using System.Collections.ObjectModel;

namespace LunarChatApp.ViewModels;

public partial class ServersViewModel : ViewModelBase
{
    private PageManager pageManager;

    public ServersViewModel(PageManager page)
    {
        pageManager = page;
        CrockeryList = new ObservableCollection<MessageItem>
        {
            new MessageItem(),
            new MessageItem(),
            new MessageItem(),
            new MessageItem(),
            new MessageItem(),
        };
    }

    public ObservableCollection<MessageItem> CrockeryList { get; set; }


    [RelayCommand]
    public void OpenSettings()
    {
        pageManager.OnSwitchPage(new SettingsPage
        {
            DataContext = new SettingsViewModel(pageManager)
        });
    }
}
