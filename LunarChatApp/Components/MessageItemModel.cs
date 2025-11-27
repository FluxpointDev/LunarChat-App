using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveMarkdown.Avalonia;
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
        Message = new ObservableStringBuilder();
        Message.Append(message.Content);
    }

    private ServiceManager services;
    public readonly string messageId;
    public readonly string authorId;

    [ObservableProperty]
    private bool _isAuthor;

    [ObservableProperty]
    private string _username;

    [ObservableProperty]
    private ObservableStringBuilder _message;

    [RelayCommand]
    public void LinkClicked(InlineHyperlinkClickedEventArgs args)
    {
        //services.Dialogs.Create(new CreateChannelDialog(), new CreateChannelDialogModel(services), "Link: ").Open();
        var launcher = TopLevel.GetTopLevel(services.State.CachedServersPage).Launcher;
        launcher.LaunchUriAsync(args.HRef);
    }

    [RelayCommand]
    public async Task Delete()
    {
        await services.Rest.DeleteAsync($"/channels/{services.State.Socket.CurrentChannel!.Id}/messages/{messageId}");
    }

    public void Update(string content)
    {
        var markdownBuilder = new ObservableStringBuilder();
        markdownBuilder.Append(content);
        Message = markdownBuilder;
    }
}
