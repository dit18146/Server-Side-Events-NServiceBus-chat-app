using ServiceStack;

[assembly: HostingStartup(typeof(MyApi.ConfigureServerEvents))]

namespace MyApi
{
    public class ConfigureServerEvents : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder) => builder
            .ConfigureAppHost(appHost => {
                appHost.Plugins.Add(new ServerEventsFeature());
            });
    }
}