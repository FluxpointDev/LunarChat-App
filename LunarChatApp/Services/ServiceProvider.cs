using Avalonia;
using Jab;
using LunarChatApp.Shared.Rest;
using ShadUI;

namespace LunarChatApp.Services;

[ServiceProvider]
[Singleton(typeof(RestClient))]
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

    public ThemeWatcher ThemeWatcherFactory()
    {
        return new ThemeWatcher(Application.Current!);
    }

    public ServiceManager ServiceManagerFactory()
    {
        return new ServiceManager(GetService<PageManager>(), GetService<TestState>(), GetService<RestClient>(), GetService<ThemeWatcher>(), GetService<DialogService>());
    }
}
