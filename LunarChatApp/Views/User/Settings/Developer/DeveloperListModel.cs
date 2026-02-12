using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using LunarChatApp.Services;
using LunarChatApp.Views.Dialogs;
using LunarChatSharp;
using LunarChatSharp.Rest.Dev;
using LunarChatSharp.Rest.Users;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LunarChatApp.Views.User.Settings.Developer;

public partial class DeveloperListModel : ViewModelBase
{
    private readonly ServiceManager services;
    public DeveloperListModel(ServiceManager sv, RestTeam? team, System.Action back, Func<DevItemModel, Task> ac)
    {
        services = sv;
        actionSelect = ac;
        Items = new ObservableCollection<TeamListItem>
        {
            new TeamListItem
            {
                Id = 0,
                Name = "All Apps"
            }
        };
        TeamsList = new ObservableCollection<DevItem>();
        AppsList = new ObservableCollection<DevItem>();

        _ = Task.Run(async () =>
        {
            RestDev? dev = await services.Rest.GetDevAsync();
            if (dev != null)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    if (dev.Teams.Length != 0)
                    {
                        Items.AddRange(dev.Teams.Select(x => new TeamListItem { Id = x.Id, Name = x.Name }));
                        TeamsList.AddRange(dev.Teams.Select(x => new DevItem { DataContext = new DevItemModel(services, x, ac) }));
                    }

                    if (dev.Apps.Length != 0)
                        AppsList.AddRange(dev.Apps.Select(x => new DevItem { DataContext = new DevItemModel(services, x, ac) }));
                    Loaded = true;
                });
            }
            else
            {
                Dispatcher.UIThread.Post(() =>
                {
                    Loaded = true;
                });
            }
        });


        SelectedTeamItem = Items.First();
    }

    private Func<DevItemModel, Task> actionSelect;

    [ObservableProperty]
    private bool _loaded;

    [ObservableProperty]
    private ObservableCollection<TeamListItem> _items;

    [ObservableProperty]
    private TeamListItem? _selectedTeamItem;

    [ObservableProperty]
    private ObservableCollection<DevItem> _teamsList;

    [ObservableProperty]
    private ObservableCollection<DevItem> _appsList;

    [RelayCommand]
    public void CreateApp()
    {
        services.Dialogs.Create(new CreateNameDialog(), new CreateNameDialogModel(), "Create App").WithSubmit(SubmitApp).Open();
    }

    [RelayCommand]
    public void CreateTeam()
    {
        services.Dialogs.Create(new CreateNameDialog(), new CreateNameDialogModel(), "Create Team").WithSubmit(SubmitTeam).Open();
    }

    public async Task SubmitApp(UserControl control)
    {
        CreateNameDialogModel? model = control.DataContext as CreateNameDialogModel;
        if (model == null)
            return;

        try
        {
            RestApp app = await services.Rest.CreateAppAsync(new CreateAppRequest
            {
                Name = model.Name
            });
            AppsList.Add(new DevItem { DataContext = new DevItemModel(services, app, actionSelect) });

        }
        catch { }
    }

    public async Task SubmitTeam(UserControl control)
    {
        CreateNameDialogModel? model = control.DataContext as CreateNameDialogModel;
        if (model == null)
            return;

        try
        {
            RestTeam team = await services.Rest.CreateTeamAsync(new CreateTeamRequest
            {
                Name = model.Name
            });
            Items.Add(new TeamListItem { Id = team.Id, Name = team.Name });
            TeamsList.Add(new DevItem { DataContext = new DevItemModel(services, team, actionSelect) });

        }
        catch { }
    }

}

public partial class TeamListItem : ObservableObject
{
    public ulong Id { get; set; }

    [ObservableProperty]
    private string _name;
}

public partial class AppListItem : ObservableObject
{
    public string Id { get; set; }

    [ObservableProperty]
    private string _name;
}
