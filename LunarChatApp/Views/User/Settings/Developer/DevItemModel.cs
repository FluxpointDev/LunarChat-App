using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatSharp.Rest.Dev;
using System;
using System.Threading.Tasks;

namespace LunarChatApp.Views.User.Settings.Developer;

public partial class DevItemModel : ViewModelBase
{
    public DevItemModel(ServiceManager sv, RestTeam team, Func<DevItemModel, Task> ac)
    {
        services = sv;
        IsTeam = true;
        Id = team.Id;
        Name = team.Name;
        Fallback = GetFallback(team.Name);
        if (!string.IsNullOrEmpty(team.IconId))
            Icon = new Uri(team.GetIconUrl());

        action = ac;
    }

    public DevItemModel(ServiceManager sv, RestApp app, Func<DevItemModel, Task> ac)
    {
        services = sv;
        Id = app.Id;
        Name = app.Name;
        Fallback = GetFallback(app.Name);
        if (!string.IsNullOrEmpty(app.AvatarId))
            Icon = new Uri(app.GetAvatarUrl());

        action = ac;
    }

    private string GetFallback(string name)
    {
        string FallbackName = null;
        foreach (var i in name.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            FallbackName += i.ToUpper()[0];
        }
        return FallbackName!;
    }

    private ServiceManager services;
    public string Id;
    public bool IsTeam;
    private Func<DevItemModel, Task> action;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _fallback;

    [ObservableProperty]
    private Uri? icon;

    [RelayCommand]
    public void Select()
    {
        action.Invoke(this);
    }
}
