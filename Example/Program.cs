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
        MyApplication myService = services.GetRequiredService<MyApplication>();

        List<long>? resultIris = await myService.RunMultipleIrisCase();
        if (resultIris is null)
        {
            Console.WriteLine("resultIris is null");
        }
        else
        {
            Console.WriteLine("Iris case result: " + string.Join(",", resultIris));
        }

        Console.WriteLine("-----------------------------------------------------------------------------------------");

        List<long>? resultIris2 = await myService.RunSingleIrisCase();
        if (resultIris2 is null)
        {
            Console.WriteLine("resultIris2 is null");
        }
        else
        {
            Console.WriteLine("Iris case 2 result: " + string.Join(",", resultIris2));
        }

        Console.WriteLine("-----------------------------------------------------------------------------------------");

        List<double>? resultSvm = await myService.RunSvm();
        if (resultSvm is null)
        {
            Console.WriteLine("resultSvm is null");
        }
        else
        {
            Console.WriteLine("SVM case result: " + string.Join(',', resultSvm));
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error Occured: {ex}");
    }
}
