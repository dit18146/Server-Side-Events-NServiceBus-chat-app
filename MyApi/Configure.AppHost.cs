using Funq;
using ServiceStack;
using MyApi.ServiceInterface;

[assembly: HostingStartup(typeof(MyApi.AppHost))]

namespace MyApi;

public class AppHost : AppHostBase, IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices(services => {
            // Configure ASP.NET Core IOC Dependencies
        });

    public AppHost() : base("MyApi", typeof(ServerEventsServices).Assembly) {}

    public override void Configure(Container container)
    {
        // Configure ServiceStack only IOC, Config & Plugins
        SetConfig(new HostConfig {
            UseSameSiteCookies = true,
        });

        container.RegisterAutoWiredAs<MemoryChatHistory, IChatHistory>();
    }
}
