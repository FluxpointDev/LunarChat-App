using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using System;
using System.Threading.Tasks;

namespace LunarChatApp.Views.User.Settings.Developer;

public partial class DevItemModel : ViewModelBase
{
    public DevItemModel(ServiceManager sv, string id, string name, bool isTeam, Func<DevItemModel, Task> ac)
    {
        services = sv;
        Id = id;
        Name = name;
        Fallback = GetFallback(name);
        IsTeam = isTeam;
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

    [RelayCommand]
    public void Select()
    {
        action.Invoke(this);
    }
}
