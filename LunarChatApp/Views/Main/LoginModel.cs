using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Validators;
using LunarChatApp.Views;
using LunarChatSharp;
using LunarChatSharp.Rest.Accounts;
using LunarChatSharp.Rest.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace LunarChatApp.ViewModels;

public partial class LoginModel(ServiceManager services, MainModel main) : ViewModelBase
{
    [ObservableProperty]
    public int _currentTab;

    [ObservableProperty]
    private string? _email;

    [ObservableProperty]
    private string? _password;

    [ObservableProperty]
    private string? _confirmPassword;


    [ObservableProperty]
    private string? _username = ServiceManager.IsDev ? "builderb" : null;

    [Required(ErrorMessage = "Email is required")]
    [EmailValidation]
    public string CheckEmail { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [MinLength(2, ErrorMessage = "Password must be at least 2 characters long")]
    public string CheckPassword { get; set; }

    [Required(ErrorMessage = "Confirm password is required")]
    [IsMatchWith(nameof(CheckPassword), ErrorMessage = "Passwords do not match")]
    public string CheckConfirmPassword { get; set; }

    [Required(ErrorMessage = "Username is required")]
    [MaxLength(32, ErrorMessage = "Username must be less than 32 characters long")]
    public string CheckUsername { get; set; }

    public bool HasErrors = false;

    public void SetProperties()
    {
        CheckEmail = Email;
        CheckUsername = Username;
        CheckPassword = Password;
        CheckConfirmPassword = ConfirmPassword;
    }

    [RelayCommand]
    private async Task SubmitDemo()
    {
        HasErrors = false;
        SetProperties();
        ValidateProperty(CheckUsername, nameof(CheckUsername));
        if (HasErrors)
            return;

        string? CleanUsername = services.State.CleanUsername(Username);
        if (string.IsNullOrEmpty(CleanUsername))
            return;

        try
        {


            RestUser Json = await services.Rest.CreateDemoAccount(new CreateDemoAccountRequest
            {
                Username = CleanUsername,
            });

            await services.Client.LoginAsync(Json.Id);

            services.State.Socket = services.Socket.State;
            services.State.CurrentDisplayName = Json.DisplayName ?? Json.Username;
            services.State.DisplayName = Json.DisplayName;
            services.State.AboutMe = Json.AboutMe;
            services.State.Username = Json.Username;
            services.State.CachedServersPage = new ServersPage
            {
                DataContext = new ServersModel(services, main)
            };
            services.PageManager.OnSwitchPage(services.State.CachedServersPage);

            if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS())
                main.NavBarVisible = true;

            _ = Task.Run(async () =>
            {
                _ = services.Socket.SetupWebsocket();
            });
        }
        catch { }
    }


    [RelayCommand]
    private void SubmitLogin()
    {
        HasErrors = false;
        SetProperties();
        ValidateProperty(CheckEmail, nameof(CheckEmail));
        ValidateProperty(CheckPassword, nameof(CheckPassword));
        if (HasErrors)
            return;
    }


    [RelayCommand]
    private void SubmitRegister()
    {
        HasErrors = false;
        SetProperties();
        ValidateProperty(CheckEmail, nameof(CheckEmail));
        ValidateProperty(CheckUsername, nameof(CheckUsername));
        ValidateProperty(CheckPassword, nameof(CheckPassword));
        ValidateProperty(CheckConfirmPassword, nameof(CheckConfirmPassword));
        if (HasErrors)
            return;
    }

    protected void ValidateProperty<T>(T value, string propertyName)
    {
        var validationContext = new ValidationContext(this)
        {
            MemberName = propertyName
        };
        var validationResults = new List<ValidationResult>();

        if (Validator.TryValidateProperty(value, validationContext, validationResults))
            return;

        if (validationResults.Count != 0)
            HasErrors = true;
    }
}
