using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using LunarChatApp.Services;
using LunarChatApp.Shared.Rest.Apps;
using LunarChatApp.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LunarChatApp.Views.User.Settings.Developer;

public partial class DeveloperListModel : ViewModelBase
{
    private ServiceManager services;
    public DeveloperListModel(ServiceManager sv, TeamJson? team, System.Action back, Action<DevItemModel> ac)
    {
        services = sv;
        actionSelect = ac;
        Items = new ObservableCollection<TeamListItem>
        {
            new TeamListItem
            {
                Id = "0",
                Name = "All Apps"
            }
        };
        TeamsList = new ObservableCollection<DevItem>();
        AppsList = new ObservableCollection<DevItem>();

        _ = Task.Run(async () =>
        {
            DevJson? dev = await services.Rest.GetAsync<DevJson>("/users/@me/dev");
            if (dev != null)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    if (dev.teams.Any())
                    {
                        Items.AddRange(dev.teams.Select(x => new TeamListItem { Id = x.id, Name = x.name }));
                        TeamsList.AddRange(dev.teams.Select(x => new DevItem { DataContext = new DevItemModel(services, x.id, x.name, true, ac) }));
                    }

                    if (dev.apps.Any())
                        AppsList.AddRange(dev.apps.Select(x => new DevItem { DataContext = new DevItemModel(services, x.id, x.name, false, ac) }));
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

    private Action<DevItemModel> actionSelect;

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
        try
        {
            AppJson app = await services.Rest.PostAsync<AppJson>("/apps", new CreateAppRequest
            {
                name = model.Name
            });
            AppsList.Add(new DevItem { DataContext = new DevItemModel(services, app.id, app.name, false, actionSelect) });

        }
        catch { }
    }

    public async Task SubmitTeam(UserControl control)
    {
        CreateNameDialogModel? model = control.DataContext as CreateNameDialogModel;
        try
        {
            TeamJson team = await services.Rest.PostAsync<TeamJson>("/teams", new CreateTeamRequest
            {
                name = model.Name
            });
            Items.Add(new TeamListItem { Id = team.id, Name = team.name });
            TeamsList.Add(new DevItem { DataContext = new DevItemModel(services, team.id, team.name, true, actionSelect) });

        }
        catch { }
    }

}

public partial class TeamListItem : ObservableObject
{
    public string Id { get; set; }

    [ObservableProperty]
    private string _name;
}

public partial class AppListItem : ObservableObject
{
    public string Id { get; set; }

    [ObservableProperty]
    private string _name;
}
