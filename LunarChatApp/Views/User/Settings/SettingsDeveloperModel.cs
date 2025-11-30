using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using LunarChatApp.Services;
using LunarChatApp.Views.User.Settings.Developer;
using LunarChatSharp.Rest.Dev;
using System;

namespace LunarChatApp.Views.User.Settings;

public partial class SettingsDeveloperModel : ViewModelBase
{
    private ServiceManager services;
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

    public void SelectDevItem(DevItemModel item)
    {
        if (item.IsTeam)
        {
            SelectedPage = new DeveloperTeamInfo()
            {
                DataContext = new DeveloperTeamInfoModel(services, new RestTeam
                {
                    Id = item.Id,
                    Name = item.Name,
                    OwnerId = null!,
                    CreatedAt = DateTime.Now
                }, BackAction)
            };
        }
        else
        {
            SelectedPage = new DeveloperAppInfo()
            {
                DataContext = new DeveloperAppInfoModel(services, new RestApp
                {
                    Id = item.Id,
                    Name = item.Name,
                    CreatedAt = DateTime.Now,
                    OwnerId = null!
                }, BackAction)
            };
        }
    }
}
