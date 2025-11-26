using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Views;
using System.Threading.Tasks;

namespace LunarChatApp.Components;

public partial class MessageItemModel : ViewModelBase
{
    public MessageItemModel(ServiceManager sv, string id, string author)
    {
        services = sv;
        messageId = id;
        authorId = author;
        IsAuthor = author == sv.State.Socket.CurrentId;
    }

    private ServiceManager services;
    public readonly string messageId;
    public readonly string authorId;

    [ObservableProperty]
    private bool _isAuthor;

    [RelayCommand]
    public async Task Delete()
    {
        await services.Rest.DeleteAsync($"/channels/{services.State.Socket.CurrentChannel!.Id}/messages/{messageId}");
    }
}
