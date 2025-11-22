using CommunityToolkit.Mvvm.ComponentModel;

namespace LunarChatApp.ViewModels.Dialogs;

public partial class ReportServerDialogModel : ViewModelBase
{
    [ObservableProperty]
    private ServerReportType _reportType;

    [ObservableProperty]
    private string? _details;
}
public enum ServerReportType
{
    Other, Scam
}