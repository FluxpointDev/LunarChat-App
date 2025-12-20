using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Views.Dialogs;
using LunarChatSharp;
using LunarChatSharp.Rest.Dev;
using System;
using System.IO;
using System.Linq;
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
        if (!string.IsNullOrEmpty(team.IconId))
            Icon = new Uri(team.GetIconUrl());
    }

    [ObservableProperty]
    private string _id;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private Uri? icon;

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
            await services.Rest.EditTeamAsync(Id, new EditTeamRequest
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

    [RelayCommand]
    public async Task UploadIcon()
    {
        var files = await services.FilePicker();
        if (!files.Any())
            return;

        try
        {
            RestTeam? getTeam = null;
            using (Stream stream = await files.First().OpenReadAsync())
            {
                getTeam = await services.Rest.EditTeamAsync(Id, new EditTeamRequest
                {
                    Icon = Utils.GetImageBase64(stream)
                });
            }
            Icon = new Uri(getTeam.GetIconUrl());
            team?.IconId = getTeam.IconId;
        }
        catch { }
    }

    [RelayCommand]
    public async Task ClearIcon()
    {
        try
        {
            await services.Rest.EditTeamAsync(Id, new EditTeamRequest
            {
                Icon = ""
            });
            Icon = null;
            team?.IconId = null;
        }
        catch { }
    }
}
