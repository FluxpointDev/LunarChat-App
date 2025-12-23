using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatSharp.Rest.Helpers;
using LunarChatSharp.Rest.Messages;
using System;
using System.Threading.Tasks;

namespace LunarChatApp.Views.Messages;

public partial class ReactionItemModel : ViewModelBase
{
    private ServiceManager services;
    private string channelId;
    private string messageId;
    private string? emojiId;
    public ReactionItemModel(ServiceManager sv, RestMessage message, RestEmoji? emoji)
    {
        services = sv;
        channelId = message.ChannelId;
        messageId = message.Id;
        emojiId = emoji?.Id;
        name = emoji?.Name;
        if (!string.IsNullOrEmpty(emoji?.IconId))
            Source = new Uri(emoji.GetIconUrl()!);
    }

    [ObservableProperty]
    public bool selfReaction;

    [ObservableProperty]
    public Uri? source;

    [ObservableProperty]
    public string? name;

    [ObservableProperty]
    public int count;

    [ObservableProperty]
    public bool isAdd;

    [RelayCommand]
    public async Task AddReaction()
    {
        if (SelfReaction)
            await services.Rest.RemoveReactionAsync(channelId, messageId, emojiId);
        else
            await services.Rest.AddReactionAsync(channelId, messageId, emojiId);
    }
}
