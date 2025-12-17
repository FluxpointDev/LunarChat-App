using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveMarkdown.Avalonia;
using LunarChatApp.Services;

namespace LunarChatApp.Views.Dialogs;

public partial class ChannelTopicDialogModel : ViewModelBase
{
    private ServiceManager services;
    public ChannelTopicDialogModel(ServiceManager sv, string tx)
    {
        services = sv;
        text = new ObservableStringBuilder();
        text.Append(tx);
    }

    [ObservableProperty]
    private ObservableStringBuilder text;

    [RelayCommand]
    public void LinkClicked(InlineHyperlinkClickedEventArgs args)
    {
        services.OpenLink(args.HRef);
    }
}
