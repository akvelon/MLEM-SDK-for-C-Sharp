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

        var resultIris = await myService.RunIris();
        Console.WriteLine(resultIris);

        Console.WriteLine("-----------------------------------------------------------------------------------------");

        var resultSvm = await myService.RunSvm();
        Console.WriteLine(String.Join(',', resultSvm));
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error Occured: {ex}");
    }
}
