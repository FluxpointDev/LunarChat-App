using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Views.Dialogs;
using LunarChatSharp;
using LunarChatSharp.Rest.Dev;
using System;
using System.Threading.Tasks;

namespace LunarChatApp.Views.User.Settings.Developer;

public partial class DeveloperTeamInfoModel : ViewModelBase
{
    private ServiceManager services;
    private Action backAction;
    private RestTeam team;
    public DeveloperTeamInfoModel(ServiceManager sv, RestTeam team, Action back)
    {
        services = sv;
        this.team = team;
        backAction = back;
        _id = team.Id;
        Name = team.Name;
    }

    [ObservableProperty]
    private string _id;

    [ObservableProperty]
    private string _name;

    [RelayCommand]
    public void Back()
    {
        backAction.Invoke();
    }

    [RelayCommand]
    public void UpdateTeam()
    {
        services.Dialogs.Create(new CreateNameDialog(), new CreateNameDialogModel { Name = Name }, "Update Team").WithSubmit(SubmitTeam).Open();
    }

    public async Task SubmitTeam(UserControl control)
    {
        CreateNameDialogModel? model = control.DataContext as CreateNameDialogModel;
        try
        {
            await services.Rest.EditTeamAsync(Id, new CreateTeamRequest
            {
                Name = model.Name
            });
            Name = model.Name;
            team.Name = model.Name;
        }
        catch { }
    }

    [RelayCommand]
    public async Task DeleteTeam()
    {
        try
        {
            await services.Rest.DeleteTeamAsync(Id);
            backAction.Invoke();
        }
        catch { }

    }
}
