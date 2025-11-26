using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Shared.Rest.Servers;
using LunarChatApp.ViewModels.Dialogs;
using System.Threading.Tasks;

namespace LunarChatApp.Views.Main;

public partial class HomeViewModel(ServiceManager services) : ViewModelBase
{
    [RelayCommand]
    public void DiscoverServer()
    {

    }

    [RelayCommand]
    public void CreateServer()
    {
        services.Dialogs.Create(new JoinServerDialog(), new JoinServerDialogModel(services), "Server").WithSubmit(SubmitServer).Open();
    }

    public async Task SubmitServer(UserControl control)
    {
        JoinServerDialogModel? model = control.DataContext as JoinServerDialogModel;
        if (model.ShowJoin)
        {

        }
        else if (model.ShowCreate)
        {
            if (string.IsNullOrEmpty(model.Textbox))
                return;

            await services.Rest.PostAsync<CreateServerRequest>("/servers", new CreateServerRequest
            {
                name = model.Textbox
            });
        }
    }
}
