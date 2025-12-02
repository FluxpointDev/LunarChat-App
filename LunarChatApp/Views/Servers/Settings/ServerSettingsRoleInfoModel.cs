using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatSharp.Rest.Roles;
using System;
using System.Threading.Tasks;

namespace LunarChatApp.Views.Servers.Settings;

public partial class ServerSettingsRoleInfoModel : ViewModelBase
{
    private ServiceManager services;
    private RestRole role;
    private Action backAction;
    public ServerSettingsRoleInfoModel(ServiceManager sv, RestRole r, Action back)
    {
        services = sv;
        backAction = back;
        role = r;
        _roleName = role.Name;
        _color = role.Color;
    }

    [ObservableProperty]
    private string? _roleName;

    [ObservableProperty]
    private string? _color;

    [RelayCommand]
    public void Back()
    {
        backAction.Invoke();
    }

    [RelayCommand]
    public void ClearColor()
    {
        Color = null;
    }

    [RelayCommand]
    public async Task Save()
    {
        EditRoleRequest req = new EditRoleRequest();
        if (RoleName != role.Name)
            req.Name = RoleName;

        if (Color != role.Color)
            req.Color = Color ?? "";

        await services.Rest.PatchAsync($"/servers/{services.State.Socket.CurrentServer?.Server.Id}/roles/" + role.Id, req);
    }
}
