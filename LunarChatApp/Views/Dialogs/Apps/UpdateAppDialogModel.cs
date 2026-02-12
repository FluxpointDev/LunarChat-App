using CommunityToolkit.Mvvm.ComponentModel;
using LunarChatSharp.Rest.Dev;

namespace LunarChatApp.Views.Dialogs;

public partial class UpdateAppDialogModel : ViewModelBase
{
    public UpdateAppDialogModel(RestApp app)
    {
        _name = app.Name;
        _description = app.Description;
        _isPublic = app.IsPublic;
        _website = app.WebsiteUrl;
        _terms = app.TermsUrl;
        _privacy = app.PrivacyUrl;
    }

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private bool _isPublic;

    [ObservableProperty]
    private string? _website;

    [ObservableProperty]
    private string? _terms;

    [ObservableProperty]
    private string? _privacy;
}
