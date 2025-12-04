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

public partial class DeveloperAppInfoModel : ViewModelBase
{
    private ServiceManager services;
    private Action backAction;
    public DeveloperAppInfoModel(ServiceManager sv, RestApp app, Action back)
    {
        services = sv;
        backAction = back;
        id = app.Id;
        Name = app.Name;
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
    public void UpdateApp()
    {
        services.Dialogs.Create(new CreateNameDialog(), new CreateNameDialogModel { Name = Name }, "Update App").WithSubmit(SubmitApp).Open();
    }

    public async Task SubmitApp(UserControl control)
    {
        CreateNameDialogModel? model = control.DataContext as CreateNameDialogModel;
        try
        {
            await services.Rest.EditAppAsync(id, new CreateAppRequest
            {
                Name = model.Name
            });
            Name = model.Name;
        }
        catch { }
    }

    [RelayCommand]
    public async Task DeleteApp()
    {
        try
        {
            await services.Rest.DeleteAppAsync(id);
            backAction.Invoke();
        }
        catch { }


    }
}
