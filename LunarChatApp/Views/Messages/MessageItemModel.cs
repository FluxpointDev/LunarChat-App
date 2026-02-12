using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveMarkdown.Avalonia;
using LunarChatApp.Services;
using LunarChatApp.Views;
using LunarChatApp.Views.Channels;
using LunarChatApp.Views.Messages;
using LunarChatSharp;
using LunarChatSharp.Core.Messages;
using LunarChatSharp.Rest.Messages;
using LunarChatSharp.Websocket.Events.Messages;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LunarChatApp.Components;

public partial class MessageItemModel : ViewModelBase
{
    public MessageItemModel(ServiceManager sv, RestMessage message)
    {
        services = sv;
        messageId = message.Id;
        if (message.Author != null)
        {
            authorId = message.Author.Id;
            IsAuthor = message.Author.Id == sv.Client.CurrentId;
            Username = message.Author.GetCurrentName();
            _isBot = message.Author.IsBot;
            CanDelete = sv.Client.CurrentId == authorId;
            if (message.Author.AvatarId.HasValue)
                Avatar = new Uri(message.Author.GetAvatarUrl()!);
            else
                fallback = message.Author.GetFallback();
        }
        if (!CanDelete.GetValueOrDefault() && sv.State.CurrentServer != null)
            CanDelete = sv.State.CurrentServer.Server.OwnerId == sv.Client.CurrentId;

        if (message.Source == MessageSourceType.Webhook)
        {
            isWebhook = true;
            _isBot = false;
        }
        IsSystem = message.SystemMessage != null;
        Message = new ObservableStringBuilder();
        Message.Append(message.Content);
        //NameColor = services.State.Socket.Roles.First().Value.Color;
        Time = message.CreatedAt.ToLocalTime().ToString("hh:mm tt");
        if (message.UpdatedAt.HasValue)
            editedTime = message.UpdatedAt.Value.ToLocalTime().ToString("hh:mm tt");
        if (message.Embeds != null && message.Embeds.Length != 0)
            embedsList = new ObservableCollection<EmbedItem>(message.Embeds.Select(x => new EmbedItem { DataContext = new EmbedItemModel(services, x) }));
        if (message.Attachments != null && message.Attachments.Length != 0)
            imagesList = new ObservableCollection<ImageItem>(message.Attachments.Select(x => new ImageItem
            {
                DataContext = new ImageItemModel(services, x)
            }));

        if (message.Reactions != null && message.Reactions.Any())
        {
            reactionList = new ObservableCollection<ReactionItem>(message.Reactions.Values.Select(x => new ReactionItem()
            {
                DataContext = new ReactionItemModel(services, message, x) { }
            }));
            if (reactionList.Count != 10)
                reactionList.Add(new ReactionItem() { DataContext = new ReactionItemModel(services, message, null) { IsAdd = true } });
        }
        else
        {
            reactionList = new ObservableCollection<ReactionItem>
            {
                new ReactionItem() { DataContext = new ReactionItemModel(services, message, null) { IsAdd = true } }
            };
        }
    }

    private readonly ServiceManager services;
    public readonly ulong messageId;
    public readonly ulong authorId;

    [ObservableProperty]
    private bool _isAuthor;

    [ObservableProperty]
    private bool _isBot;

    [ObservableProperty]
    private bool isWebhook;

    [ObservableProperty]
    private bool _isSystem;

    [ObservableProperty]
    private string _username;

    [ObservableProperty]
    private Uri? avatar;

    [ObservableProperty]
    private string fallback;

    [ObservableProperty]
    private ObservableStringBuilder _message;

    [ObservableProperty]
    private string _time;

    [ObservableProperty]
    private string? editedTime;

    [ObservableProperty]
    private string? _nameColor;

    [ObservableProperty]
    private bool? _canDelete;

    [ObservableProperty]
    private ObservableCollection<EmbedItem>? embedsList;

    [ObservableProperty]
    private ObservableCollection<ImageItem>? imagesList;

    [ObservableProperty]
    private ObservableCollection<ReactionItem>? reactionList;

    [RelayCommand]
    public void LinkClicked(InlineHyperlinkClickedEventArgs args)
    {
        if (args.HRef == null)
            return;
        services.OpenLink(args.HRef);
    }

    [RelayCommand]
    public async Task Delete()
    {
        try
        {
            await services.Rest.DeleteMessageAsync(services.State.CurrentChannel!.Id, messageId);
        }
        catch { }
    }

    [RelayCommand]
    public void CopyText()
    {
        if (Message != null)
            services.CopyText(Message.ToString());
    }

    [RelayCommand]
    public void CopyId()
    {
        services.CopyText(messageId.ToString());
    }

    public void Update(MessageUpdateEvent ev, EditMessageRequest message)
    {
        if (ev.UpdatedAt.HasValue)
            EditedTime = ev.UpdatedAt.Value.ToLocalTime().ToString("hh:mm tt");

        if (message.Content != null)
        {
            var markdownBuilder = new ObservableStringBuilder();
            markdownBuilder.Append(message.Content);
            Message = markdownBuilder;
        }

        if (message.Embeds != null)
        {
            if (message.Embeds != null && message.Embeds.Length != 0)
                EmbedsList = new ObservableCollection<EmbedItem>(message.Embeds.Select(x => new EmbedItem { DataContext = new EmbedItemModel(services, x) }));
            else
                EmbedsList = null;
        }
    }
}
