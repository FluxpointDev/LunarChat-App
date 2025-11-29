using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Shared.Rest.Accounts;
using LunarChatApp.Shared.Rest.Users;
using LunarChatApp.Shared.WebSocket;
using LunarChatApp.Validators;
using LunarChatApp.Views;
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
    private string? _username = services.IsDev ? "builderb" : null;

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

        StoatUser Json = await services.Rest.PostAsync<StoatUser>("/accounts/test", new CreateAccountRequest
        {
            username = Username!.ToLower()
        });
        services.Rest.Http.DefaultRequestHeaders.Add("Auth-Id", Json._id);

        LunarSocketClient socket = new LunarSocketClient(
            services.IsDev ? "ws://localhost:5156/gateway" : "wss://lunar.fluxpoint.dev/api/gateway", Json._id);
        services.State.Socket = socket.State;
        services.State.Socket.WebSocket = socket;
        services.State.Socket.CurrentId = Json._id;
        services.State.DisplayName = Username.ToLower();
        services.State.Username = Username.ToLower();
        services.State.CachedServersPage = new ServersPage
        {
            DataContext = new ServersModel(services, main)
        };
        services.PageManager.OnSwitchPage(services.State.CachedServersPage);

        if (services.State.Socket.APIEnabled)
            _ = services.State.Socket.WebSocket.SetupWebsocket();
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
