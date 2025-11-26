using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Shared.Core.Messages;
using LunarChatApp.Views;
using System.Threading.Tasks;

namespace LunarChatApp.Components;

public partial class MessageItemModel : ViewModelBase
{
    public MessageItemModel(ServiceManager sv, Message message)
    {
        services = sv;
        messageId = message.Id;
        authorId = message.AuthorId;
        IsAuthor = message.AuthorId == sv.State.Socket.CurrentId;
        Username = message.Username;
    }

    private ServiceManager services;
    public readonly string messageId;
    public readonly string authorId;

    [ObservableProperty]
    private bool _isAuthor;

    [ObservableProperty]
    private string _username;

    [RelayCommand]
    public async Task Delete()
    {
        await services.Rest.DeleteAsync($"/channels/{services.State.Socket.CurrentChannel!.Id}/messages/{messageId}");
    }
}
