using Example;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MlemApi;

IHostBuilder builder = new HostBuilder()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddTransient<ConsoleApplication>();
        services.AddTransient<IRequestValueSerializer>(sp => new NewtonsoftRequestValueSerializer());
        services.AddTransient(sp => LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<MlemApiClient>());
        services.AddHttpClient<MlemApiClient>().AddHttpMessageHandler<LoggingDelegatingHandler>();
        services.AddTransient<LoggingDelegatingHandler>();
    }).UseConsoleLifetime();

using IServiceScope serviceScope = builder.Build().Services.CreateScope();
IServiceProvider services = serviceScope.ServiceProvider;

try
{
    ConsoleApplication consoleService = services.GetRequiredService<ConsoleApplication>();

    await consoleService.RunTestCaseAsync(TestCases.MultipleIris);
}
catch (Exception ex)
{
    Console.WriteLine($"Error Occured: {ex}");
}
