using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Views.Dialogs;
using LunarChatSharp;
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
        sv.State.Socket.OnAccountUpdate += AccountUpdate;
        DisplayName = state.DisplayName;
        Username = state.Username;
        Email = state.Socket.Account.Email;
    }

    public async Task AccountUpdate(AccountUpdateEvent ev)
    {
        if (ev.Username != null)
            Username = ev.Username;

        if (ev.DisplayName != null)
            DisplayName = ev.DisplayName;

        if (ev.Email != null)
            Email = ev.Email;

        if (ev.FriendRequestsEveryone.HasValue)
            RequestsEveryone = ev.FriendRequestsEveryone.Value;

        if (ev.FriendRequestsServerMembers.HasValue)
            RequestsServerMembers = ev.FriendRequestsServerMembers.Value;

        if (ev.FriendRequestsMutualFriends.HasValue)
            RequestsMutualFriends = ev.FriendRequestsMutualFriends.Value;
    }

    [ObservableProperty]
    private string _username;

    [ObservableProperty]
    private string _displayName;

    [ObservableProperty]
    private string _email;

    [ObservableProperty]
    private bool requestsEveryone = true;

    [ObservableProperty]
    private bool requestsServerMembers = true;

    [ObservableProperty]
    private bool requestsMutualFriends = true;

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

        await services.Rest.AccountEditDisplayName(model.Name);
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

        await services.Rest.AccountEditUsername(CleanUsername);
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
