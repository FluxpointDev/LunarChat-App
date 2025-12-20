using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using LunarChatApp.Services;
using LunarChatApp.Views.User.Settings.Developer;
using LunarChatSharp;
using System.Threading.Tasks;

namespace LunarChatApp.Views.User.Settings;

public partial class SettingsDeveloperModel : ViewModelBase
{
    private readonly ServiceManager services;
    public SettingsDeveloperModel(ServiceManager sv)
    {
        services = sv;
        SelectedPage = new DeveloperList() { DataContext = new DeveloperListModel(services, null, BackAction, SelectDevItem) };
    }

    [ObservableProperty]
    private UserControl? _selectedPage;

    public void BackAction()
    {
        SelectedPage = new DeveloperList() { DataContext = new DeveloperListModel(services, null, BackAction, SelectDevItem) };
    }

    public async Task SelectDevItem(DevItemModel item)
    {
        if (item.IsTeam)
        {
            try
            {
                var Team = await services.Rest.GetTeamAsync(item.Id);
                if (Team == null)
                    return;
                SelectedPage = new DeveloperTeamInfo()
                {
                    DataContext = new DeveloperTeamInfoModel(services, Team, BackAction)
                };
            }
            catch { }
        }
        else
        {
            try
            {
                var App = await services.Rest.GetAppAsync(item.Id);
                if (App == null)
                    return;
                SelectedPage = new DeveloperAppInfo()
                {
                    DataContext = new DeveloperAppInfoModel(services, App, BackAction)
                };
            }
            catch { }

        }
    }
}
