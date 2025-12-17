using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace LunarChatApp.Views.Dialogs.Servers;

public partial class TimeoutMemberDialogModel : ViewModelBase
{
    [ObservableProperty]
    private int selectedTimeoutIndex = 1;

    [ObservableProperty]
    private string? reason;

    public DateTime GetTimeout()
    {
        switch (SelectedTimeoutIndex)
        {
            case 0:
                return DateTime.UtcNow.AddMinutes(10);
            case 1:
                return DateTime.UtcNow.AddMinutes(30);
            case 2:
                return DateTime.UtcNow.AddMinutes(60);
            case 3:
                return DateTime.UtcNow.AddMinutes(60 * 5);
            case 4:
                return DateTime.UtcNow.AddMinutes(60 * 24);
            case 5:
                return DateTime.UtcNow.AddMinutes((60 * 24) * 3);
            case 6:
                return DateTime.UtcNow.AddMinutes((60 * 24) * 7);
        }
        return DateTime.UtcNow.AddDays(1);
    }
}
