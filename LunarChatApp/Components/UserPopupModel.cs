using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveMarkdown.Avalonia;
using LunarChatApp.Services;
using LunarChatApp.Views;
using LunarChatSharp;
using LunarChatSharp.Rest.Users;
using Material.Icons;
using System;
using System.Threading.Tasks;

namespace LunarChatApp.Components;

public partial class UserPopupModel : ViewModelBase
{
    private ServiceManager services;

    public UserPopupModel(ServiceManager sv, string userId)
    {
        services = sv;

        IsLoading = true;
        _ = Task.Run(async () =>
        {
            user = await services.Rest.GetUserAsync(userId);
            if (user == null)
                return;

            Dispatcher.UIThread.Post(() =>
            {
                Update(user);
                IsLoading = false;
            });
        });
    }

    public void Update(RestUser user)
    {
        Name = user.DisplayName ?? user.Username;
        Username = user.Username;
        AboutMe = new ObservableStringBuilder();
        AboutMe.Append(user.AboutMe);
        if (!string.IsNullOrEmpty(user.AvatarId))
            Avatar = new Uri(user.GetAvatarUrl());
        else
            fallback = user.GetFallback();
    }

    [RelayCommand]
    public void LinkClicked(InlineHyperlinkClickedEventArgs args)
    {
        services.OpenLink(args.HRef);
    }

    private RestUser? user;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private string username;

    [ObservableProperty]
    private Uri? avatar;

    [ObservableProperty]
    private string fallback;

    [ObservableProperty]
    private ObservableStringBuilder aboutMe;

    [ObservableProperty]
    private MaterialIconKind _statusIcon = MaterialIconKind.Circle;

    [ObservableProperty]
    private string _statusColor = "#FF00C853";

    [RelayCommand]
    public void ToggleLoading()
    {
        IsLoading = !IsLoading;
    }

    [RelayCommand]
    public void CopyId()
    {
        services.CopyText(user.Id);
    }
}
