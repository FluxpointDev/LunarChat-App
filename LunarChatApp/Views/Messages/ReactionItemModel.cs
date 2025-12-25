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
    public ReactionItemModel(ServiceManager sv, RestMessage message, RestReaction? reaction)
    {
        services = sv;
        channelId = message.ChannelId;
        messageId = message.Id;

        if (reaction != null)
        {
            emojiId = reaction.Emoji?.Id;
            name = reaction.Emoji?.Name;
            count = reaction.Count;
            selfReaction = reaction.hasReacted;
            if (!string.IsNullOrEmpty(reaction.Emoji?.IconId))
                Source = new Uri(reaction.Emoji.GetIconUrl()!);
        }
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

    [RelayCommand]
    public void OpenEmojiMenu()
    {
        services.State.EmojisMenu.ReactionMessage = messageId;
        services.State.OpenEmojiMenu?.Invoke();
    }
}
