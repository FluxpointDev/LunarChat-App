using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Views;
using LunarChatSharp.Rest.Messages;
using System;
using System.Diagnostics;

namespace LunarChatApp.Components;

public partial class EmojiListItemModel : ViewModelBase
{
    private readonly ServiceManager services;
    public EmojiListItemModel(ServiceManager sv, RestEmoji emoji)
    {
        services = sv;
        emojiId = emoji.Id;
        if (!string.IsNullOrEmpty(emoji.IconId))
            Source = new Uri(emoji.GetIconUrl()!);


        Debug.WriteLine("Render: " + emoji.Name);
    }

    [ObservableProperty]
    private Uri source;

    public string emojiId;

    [RelayCommand]
    public void UseEmoji()
    {
        services.State.UseEmoji?.Invoke(this);
    }
}
