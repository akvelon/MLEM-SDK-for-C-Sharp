// See https://aka.ms/new-console-template for more information

using Example;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MlemApi;

var builder = new HostBuilder()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHttpClient<MlemApiClient>()
            .AddHttpMessageHandler<LoggingDelegatingHandler>();

        services.AddTransient<MyApplication>();
        services.AddTransient<IRequestValueSerializer>(sp => new NewtonsoftRequestValueSerializer());
        services.AddTransient<IMlemApiConfiguration>(sp => new MlemApiConfiguration());

        services.AddTransient<LoggingDelegatingHandler>();

        services.AddTransient<ILogger<MlemApiClient>>(sp => LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<MlemApiClient>());
    }).UseConsoleLifetime();

var host = builder.Build();

using (var serviceScope = host.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;

    try
    {
        var myService = services.GetRequiredService<MyApplication>();
        var result = await myService.Run();

        Console.WriteLine(result);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error Occured: {ex}");
    }
}
