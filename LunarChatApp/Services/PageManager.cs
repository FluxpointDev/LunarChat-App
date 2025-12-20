using Avalonia.Controls;
using LunarChatSharp.Rest.Channels;
using LunarChatSharp.Rest.Servers;
using System;
using System.Linq;
using System.Reflection;

namespace LunarChatApp.Services;

public sealed class PageManager(ServiceProvider serviceProvider)
{
    public void Navigate<T>() where T : INavigable
    {
        var attr = typeof(T).GetCustomAttribute<PageAttribute>();
        if (attr is null) throw new InvalidOperationException("Not a valid page type, missing PageAttribute");

        var page = serviceProvider.GetService<T>();
        if (page is null) throw new InvalidOperationException("Page not found");

        OnNavigate?.Invoke(page, attr.Route);
    }

    private Action<INavigable, string>? _onNavigate;

    public Action<INavigable, string>? OnNavigate
    {
        private get => _onNavigate;
        set
        {
            if (_onNavigate is not null)
            {
                throw new InvalidOperationException("OnNavigate is already set");
            }

            _onNavigate = value;
        }
    }

    public Action<UserControl> OnSwitchPage;

    public void SwitchServer(ServiceManager services, RestServer? server, RestChannel? channel = null)
    {
        if (server == null)
            return;

        if (server.Id != services.State.CurrentServer?.Server.Id)
        {
            services.State.CurrentServer = services.State.Socket.Servers[server.Id];
            services.Client.OnSelectServer?.Invoke(server);
        }

        if (services.State.CurrentServer != null)
        {
            if (channel == null && services.State.CurrentServer.Channels.Any())
                channel = services.State.CurrentServer.Channels.FirstOrDefault().Value;

            if (channel != null && services.State.CurrentChannel?.Id != channel.Id)
            {
                services.State.CurrentChannel = channel;
                services.Client.OnSelectChannel?.Invoke(channel);
            }
        }
    }

    public void SwitchServerChannel(ServiceManager services, RestChannel? channel = null)
    {
        if (channel != null)
        {
            services.State.CurrentChannel = channel;
            services.Client.OnSelectChannel?.Invoke(channel);
        }
        else
        {
            services.State.CurrentChannel = null;
            services.Client.OnSelectChannel?.Invoke(null);
        }
    }
}

public interface INavigable
{
    void Initialize()
    {
    }
}

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class PageAttribute(string route) : Attribute
{
    public string Route { get; } = route;
}