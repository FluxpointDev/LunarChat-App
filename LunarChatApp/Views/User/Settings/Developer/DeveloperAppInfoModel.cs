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

public partial class DeveloperAppInfoModel : ViewModelBase
{
    private ServiceManager services;
    private Action backAction;
    private RestApp app;
    public DeveloperAppInfoModel(ServiceManager sv, RestApp app, Action back)
    {
        services = sv;
        backAction = back;
        this.app = app;
        _id = app.Id;
        _name = app.Name;
        _type = app.IsPublic.GetValueOrDefault() ? "Public App" : "Private App";
        _description = app.Description;
        _website = app.Website;
        _terms = app.Terms;
        _privacy = app.Privacy;
        if (!string.IsNullOrEmpty(app.AvatarId))
            Icon = new Uri(app.GetAvatarUrl());
    }

    [ObservableProperty]
    private string _id;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _type;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private string? _website;

    [ObservableProperty]
    private string? _terms;

    [ObservableProperty]
    private string? _privacy;

    [ObservableProperty]
    private Uri? icon;

    [RelayCommand]
    public void Back()
    {
        backAction.Invoke();
    }

    [RelayCommand]
    public void UpdateApp()
    {
        services.Dialogs.Create(new UpdateAppDialog(), new UpdateAppDialogModel(app), "Update App").WithSubmit(SubmitApp).Open();
    }

    [RelayCommand]
    public void InviteApp()
    {
        services.Dialogs.Create(new InviteAppDialog(), new InviteAppDialogModel(services, app.Id), "Invite " + app.Name).WithSubmit(InviteApp).Open();
    }

    [RelayCommand]
    public async Task UploadIcon()
    {
        var files = await services.FilePicker();
        if (!files.Any())
            return;

        try
        {
            RestApp? getApp = null;
            using (Stream stream = await files.First().OpenReadAsync())
            {
                getApp = await services.Rest.EditAppAsync(Id, new EditAppRequest
                {
                    Avatar = Utils.GetImageBase64(stream)
                });
            }
            app?.AvatarId = getApp.AvatarId;
            Icon = new Uri(getApp.GetAvatarUrl());
        }
        catch { }
    }

    [RelayCommand]
    public async Task ClearIcon()
    {
        try
        {
            await services.Rest.EditAppAsync(Id, new EditAppRequest
            {
                Avatar = ""
            });
            app?.AvatarId = null;
            Icon = null;
        }
        catch { }
    }

    public async Task InviteApp(UserControl control)
    {
        InviteAppDialogModel? model = control.DataContext as InviteAppDialogModel;
        if (model == null)
            return;

        if (model.SelectedServer != null)
        {
            try
            {

                await services.Rest.AddServerAppAsync(model.SelectedServer.id, app.Id);
            }
            catch { }
        }
        if (model.SelectedGroup != null)
        {
            try
            {

                await services.Rest.AddGroupAppAsync(model.SelectedGroup.id, app.Id);
            }
            catch { }
        }
    }

    public async Task SubmitApp(UserControl control)
    {

        try
        {
            UpdateAppDialogModel? model = control.DataContext as UpdateAppDialogModel;
            var getApp = await services.Rest.EditAppAsync(Id, new EditAppRequest
            {
                Name = model.Name,
                Description = model.Description ?? "",
                IsPublic = model.IsPublic,
                Privacy = model.Privacy ?? "",
                Terms = model.Terms ?? "",
                Website = model.Website ?? ""
            });

            Name = getApp.Name;
            app.Name = getApp.Name;

            Description = getApp.Description;
            app.Description = getApp.Description;

            Type = getApp.IsPublic.GetValueOrDefault() ? "Public App" : "Private App";
            app.IsPublic = getApp.IsPublic;

            Privacy = getApp.Privacy;
            app.Privacy = getApp.Privacy;

            Terms = getApp.Terms;
            app.Terms = getApp.Terms;

            Website = getApp.Website;
            app.Website = getApp.Website;
        }
        catch { }
    }

    [RelayCommand]
    public async Task DeleteApp()
    {
        try
        {
            await services.Rest.DeleteAppAsync(Id);
            backAction.Invoke();
        }
        catch { }
    }
}
