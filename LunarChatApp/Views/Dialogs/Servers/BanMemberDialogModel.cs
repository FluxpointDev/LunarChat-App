using CommunityToolkit.Mvvm.ComponentModel;
using LunarChatSharp.Rest.Servers;

namespace LunarChatApp.Views.Dialogs.Servers;

public partial class BanMemberDialogModel : ViewModelBase
{
    [ObservableProperty]
    private string? reason;

    [ObservableProperty]
    private int selectedMaxDaysIndex;

    public CreateBanRequest CreateRequest()
    {
        int MaxDays = 0;
        switch (SelectedMaxDaysIndex)
        {
            case 1:
                MaxDays = 30;
                break;
            case 2:
                MaxDays = 14;
                break;
            case 3:
                MaxDays = 7;
                break;
            case 4:
                MaxDays = 3;
                break;
            case 5:
                MaxDays = 1;
                break;
        }
        return new CreateBanRequest
        {
            Reason = Reason,
            MaxDays = MaxDays,
        };
    }
}
