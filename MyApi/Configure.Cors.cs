using ServiceStack;

[assembly: HostingStartup(typeof(MyApi.ConfigureCors))]

namespace MyApi
{
    public class ConfigureCors : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder) => builder
            .ConfigureAppHost(appHost => {
                appHost.Plugins.Add(new CorsFeature());
            });
    }
}