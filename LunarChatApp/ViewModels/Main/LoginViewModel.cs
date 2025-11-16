using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Validators;
using ShadUI;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LunarChatApp.ViewModels;

public partial class LoginViewModel(PageManager pageManager, RestClient rest, TestState state, ThemeWatcher themeWatcher, MainViewModel main) : ViewModelBase
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
    private string? _username;

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
    private void SubmitDemo()
    {
        HasErrors = false;
        SetProperties();
        ValidateProperty(CheckUsername, nameof(CheckUsername));
        if (HasErrors)
            return;

        state.DisplayName = Username;
        state.Username = Username;
        state.CachedServersPage = new ServersPage
        {
            DataContext = new ServersViewModel(pageManager, state, themeWatcher, main, rest)
        };
        pageManager.OnSwitchPage(state.CachedServersPage);
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
