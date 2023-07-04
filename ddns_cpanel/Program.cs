using ddns_cpanel;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostCtx, services) =>
    {
        IConfiguration configuration = hostCtx.Configuration;
        services.AddHostedService<Worker>();
    })
    .ConfigureAppConfiguration((hostCtx, configBuilder) =>
    {
        configBuilder.AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{hostCtx.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
    })
    .Build();

await host.RunAsync();

