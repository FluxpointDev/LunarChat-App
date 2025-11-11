using Jab;

namespace LunarChatApp.Services;

[ServiceProvider]
[Singleton(typeof(PageManager), Factory = nameof(PageManagerFactory))]
public partial class ServiceProvider
{
    public PageManager PageManagerFactory()
    {
        return new PageManager(this);
    }
}
