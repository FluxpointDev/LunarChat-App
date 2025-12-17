using CommunityToolkit.Mvvm.ComponentModel;
using LunarChatSharp.Rest.Servers;
using System.Globalization;

namespace LunarChatApp.Views.Dialogs;

public partial class CreateInviteDialogModel : ViewModelBase
{
    public CreateInviteDialogModel()
    {
        format = new NumberFormatInfo()
        {
            CurrencyDecimalDigits = 0,
        };
    }

    [ObservableProperty]
    private int selectedExpireIndex = 2;

    [ObservableProperty]
    private int selectedUsesIndex = 1;

    [ObservableProperty]
    private int maxUses = 1;

    [ObservableProperty]
    private NumberFormatInfo format;

    public CreateInviteRequest CreateRequest()
    {
        CreateInviteRequest req = new CreateInviteRequest();
        int MintesADay = 1440;
        switch (SelectedExpireIndex)
        {
            case 1:
                req.MaxAge = MintesADay * 30;
                break;
            case 2:
                req.MaxAge = MintesADay * 7;
                break;
            case 3:
                req.MaxAge = MintesADay * 1;
                break;
        }
        switch (SelectedUsesIndex)
        {
            case 0:
                req.MaxUses = MaxUses;
                break;
            case 2:
                req.MaxUses = 1000;
                break;
            case 3:
                req.MaxUses = 100;
                break;
            case 4:
                req.MaxUses = 50;
                break;
            case 5:
                req.MaxUses = 25;
                break;
            case 6:
                req.MaxUses = 10;
                break;
            case 7:
                req.MaxUses = 1;
                break;
        }
        return req;
    }
}
