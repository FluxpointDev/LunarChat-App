using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Shared.Rest.Apps;
using LunarChatApp.Views.Dialogs;
using System;
using System.Threading.Tasks;

namespace LunarChatApp.Views.User.Settings.Developer;

public partial class DeveloperAppInfoModel : ViewModelBase
{
    private ServiceManager services;
    private Action backAction;
    public DeveloperAppInfoModel(ServiceManager sv, AppJson app, Action back)
    {
        services = sv;
        backAction = back;
        id = app.id;
        Name = app.name;
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
            await services.Rest.PatchAsync("/apps/" + id, new CreateAppRequest
            {
                name = model.Name
            });
            Name = model.Name;
        }
        catch { }
    }

    [RelayCommand]
    public async Task DeleteApp()
    {
        await services.Rest.DeleteAsync("/apps/" + id);
        backAction.Invoke();
    }
}
