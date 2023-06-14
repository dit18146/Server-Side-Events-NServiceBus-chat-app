using Messages;

namespace MyApi;



static class Program
{
    public static async Task Main(string[] args)
    {
        using var host = CreateHostBuilder(args).Build();
        await host.StartAsync();

        Console.WriteLine("Press any key to shutdown");
        Console.ReadKey();
        await host.StopAsync();
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        #region ContainerConfiguration
        Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                //services.AddScoped<IServerEvents, MemoryServerEvents>();
            })
            .UseNServiceBus(c =>
            {
                var endpointConfiguration = new EndpointConfiguration("MyApi");
                var transport = endpointConfiguration.UseTransport<LearningTransport>();
                var routing = transport.Routing();
                routing.RouteToEndpoint(typeof(LogMessage), "Logger");
                return endpointConfiguration;
            })
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    #endregion
}