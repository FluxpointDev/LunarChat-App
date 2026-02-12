using Avalonia.Layout;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveMarkdown.Avalonia;
using LunarChatApp.Services;
using LunarChatSharp.Rest.Messages;
using System;
using System.Collections.ObjectModel;

namespace LunarChatApp.Views.Channels;

public partial class EmbedItemModel : ViewModelBase
{
    private ServiceManager services;
    public EmbedItemModel(ServiceManager sv, RestEmbed embed)
    {
        services = sv;
        Update(embed);
    }

    public void Update(RestEmbed embed)
    {
        Title = embed.Title;
        Description = new ObservableStringBuilder();
        Description.Append(embed.Description);
        Color = embed.Color;
        if (embed.Author != null && !string.IsNullOrEmpty(embed.Author.Name))
        {
            AuthorName = embed.Author.Name;
            AuthorIcon = new Uri(embed.Author.IconUrl);
        }
        else
        {
            AuthorName = null;
            AuthorIcon = null;
        }
        if (!string.IsNullOrEmpty(embed.ImageUrl))
            Image = new Uri(embed.ImageUrl);

        if (!string.IsNullOrEmpty(embed.ThumbnailUrl))
            Thumbnail = new Uri(embed.ThumbnailUrl);

        if (embed.Footer != null && !string.IsNullOrEmpty(embed.Footer.Text))
        {
            FooterText = embed.Footer.Text;
            FooterIcon = embed.Footer.IconUrl;
        }
        else
        {
            FooterText = null;
            FooterIcon = null;
        }

        if (embed.Fields != null)
        {
            Fields = new ObservableCollection<RestEmbedField>(embed.Fields);
        }
        else
        {
            Fields = null;
        }
        InlineFields = embed.InlineFields.GetValueOrDefault() ? Orientation.Horizontal : Orientation.Vertical;
    }

    [ObservableProperty]
    private string? title;

    [ObservableProperty]
    private string? titleUrl;

    [ObservableProperty]
    private ObservableStringBuilder description;

    [ObservableProperty]
    private string? color;

    [ObservableProperty]
    private string? authorName;

    [ObservableProperty]
    private Uri? authorIcon;

    private string? authorUrl;

    [ObservableProperty]
    private Uri? image;

    [ObservableProperty]
    private Uri? thumbnail;

    [ObservableProperty]
    private string? video;

    [ObservableProperty]
    private string? footerText;

    [ObservableProperty]
    private string? footerIcon;

    [ObservableProperty]
    private ObservableCollection<RestEmbedField>? fields;

    [ObservableProperty]
    private Orientation inlineFields;

    [RelayCommand]
    public void LinkClicked(InlineHyperlinkClickedEventArgs args)
    {
        services.OpenLink(args.HRef);
    }

    [RelayCommand]
    public void TitleClicked()
    {
        if (!string.IsNullOrEmpty(titleUrl))
            services.OpenLink(new Uri(titleUrl));
    }

    [RelayCommand]
    public void AuthorClicked()
    {
        if (!string.IsNullOrEmpty(authorUrl))
            services.OpenLink(new Uri(authorUrl));
    }
}
