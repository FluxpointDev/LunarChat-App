using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Shared.Rest.Apps;
using LunarChatApp.Views.Dialogs;
using System;
using System.Threading.Tasks;

namespace LunarChatApp.Views.User.Settings.Developer;

public partial class DeveloperTeamInfoModel : ViewModelBase
{
    private ServiceManager services;
    private Action backAction;
    public DeveloperTeamInfoModel(ServiceManager sv, TeamJson team, Action back)
    {
        services = sv;
        backAction = back;
        id = team.id;
        Name = team.name;
    }

    private string id;

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
            await services.Rest.PatchAsync<TeamJson>("/teams/" + id, new CreateTeamRequest
            {
                name = model.Name
            });
            Name = model.Name;
        }
        catch { }
    }

    [RelayCommand]
    public async Task DeleteTeam()
    {
        await services.Rest.DeleteAsync("/teams/" + id);
        backAction.Invoke();
    }
}
