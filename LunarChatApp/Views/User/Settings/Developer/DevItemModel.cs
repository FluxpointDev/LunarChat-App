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
        if (team.IconId.HasValue)
            Icon = new Uri(team.GetIconUrl()!);

        action = ac;
    }

    public DevItemModel(ServiceManager sv, RestApp app, Func<DevItemModel, Task> ac)
    {
        services = sv;
        Id = app.Id;
        Name = app.Name;
        Fallback = GetFallback(app.Name);
        if (app.AvatarId.HasValue)
            Icon = new Uri(app.GetAvatarUrl()!);

        action = ac;
    }

    private string GetFallback(string name)
    {
        string? FallbackName = null;
        foreach (var i in name.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            FallbackName += i.ToUpper()[0];
        }
        return FallbackName!;
    }

    private readonly ServiceManager services;
    public ulong Id;
    public bool IsTeam;
    private readonly Func<DevItemModel, Task> action;

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
