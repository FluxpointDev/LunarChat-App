using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Views.Dialogs;
using LunarChatSharp;
using LunarChatSharp.Rest.Accounts;
using LunarChatSharp.Websocket.Events.Account;
using System.Threading.Tasks;

namespace LunarChatApp.Views.User.Settings;

public partial class SettingsProfileModel : ViewModelBase
{
    private ServiceManager services;
    public TestState state { get; set; }

    public SettingsProfileModel(ServiceManager sv)
    {
        services = sv;
        state = sv.State;
        sv.Client.OnAccountUpdate += AccountUpdate;
        _displayName = state.DisplayName;
        _username = state.Username;
        _email = state.Socket.Account.Email;
        aboutMe = state.AboutMe;
    }

    public async Task AccountUpdate(AccountUpdateEvent ev)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (ev.Username != null)
                Username = ev.Username;

            if (ev.DisplayName != null)
                DisplayName = ev.DisplayName;

            if (ev.Email != null)
                Email = ev.Email;
        });

    }

    [ObservableProperty]
    private string _username;

    [ObservableProperty]
    private string _displayName;

    [ObservableProperty]
    private string _email;

    [ObservableProperty]
    private string? aboutMe;

    [RelayCommand]
    public async Task UpdateAboutMe()
    {
        try
        {
            await services.Rest.AccountEdit(new EditAccountRequest
            {
                AboutMe = AboutMe
            });
            state.AboutMe = AboutMe.ToNullOrString();
        }
        catch { }
    }

    [RelayCommand]
    public void EditDisplayName()
    {
        services.Dialogs.Create(new CreateNameDialog(), new CreateNameDialogModel
        {
            Name = DisplayName
        }, "Edit Display Name").WithSubmit(SubmitDisplayName).Open();
    }

    public async Task SubmitDisplayName(UserControl control)
    {
        CreateNameDialogModel? model = control.DataContext as CreateNameDialogModel;
        if (model.Name == null)
            model.Name = "";

        try
        {
            await services.Rest.AccountEditDisplayName(model.Name);
        }
        catch { }

    }

    [RelayCommand]
    public void EditUsername()
    {
        services.Dialogs.Create(new CreateNameDialog(), new CreateNameDialogModel
        {
            Name = Username
        }, "Edit Username").WithSubmit(SubmitUsername).Open();
    }

    public async Task SubmitUsername(UserControl control)
    {
        CreateNameDialogModel? model = control.DataContext as CreateNameDialogModel;
        if (string.IsNullOrEmpty(model.Name))
            return;

        string? CleanUsername = services.State.CleanUsername(model.Name);
        if (string.IsNullOrEmpty(CleanUsername))
            return;

        try
        {
            await services.Rest.AccountEditUsername(CleanUsername);
        }
        catch { }

    }

    [RelayCommand]
    public void EditEmail()
    {

    }

    [RelayCommand]
    public void ChangePassword()
    {

    }

    [RelayCommand]
    public void DisableAccount()
    {

    }

    [RelayCommand]
    public void DeleteAccount()
    {

    }
}
