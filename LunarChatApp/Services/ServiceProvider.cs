using Avalonia;
using Jab;
using LunarChatSharp;
using ShadUI;

namespace LunarChatApp.Services;

[ServiceProvider]
[Singleton(typeof(LunarClient), Factory = nameof(ClientFactory))]
[Singleton(typeof(TestState))]
[Singleton(typeof(DialogService))]
[Singleton(typeof(PageManager), Factory = nameof(PageManagerFactory))]
[Singleton(typeof(ThemeWatcher), Factory = nameof(ThemeWatcherFactory))]
[Singleton(typeof(ServiceManager), Factory = nameof(ServiceManagerFactory))]
public partial class ServiceProvider
{
    public PageManager PageManagerFactory()
    {
        return new PageManager(this);
    }

    public LunarClient ClientFactory()
    {
        return new LunarChatSharp.LunarClient(ClientMode.WebSocket, new LunarChatSharp.ClientConfig
        {
            ApiUrl = ServiceManager.IsDev ? "https://localhost:7216/" : "https://lunar.fluxpoint.dev/api/"
        });
    }

    public ThemeWatcher ThemeWatcherFactory()
    {
        return new ThemeWatcher(Application.Current!);
    }

    public ServiceManager ServiceManagerFactory()
    {
        return new ServiceManager(GetService<PageManager>(), GetService<TestState>(), GetService<LunarClient>(), GetService<ThemeWatcher>(), GetService<DialogService>());
    }
}
