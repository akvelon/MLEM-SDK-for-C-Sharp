using Example;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MlemApi;
using MlemApi.Serializing;

IHostBuilder builder = new HostBuilder()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddTransient<ConsoleApplication>();
        services.AddTransient<IRequestValuesSerializer>(sp => new DefaultRequestValueSerializer());
        services.AddTransient(sp => LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<MlemApiClient>());
        services.AddHttpClient<MlemApiClient>().AddHttpMessageHandler<LoggingDelegatingHandler>();
        services.AddTransient<LoggingDelegatingHandler>();
    }).UseConsoleLifetime();

using IServiceScope serviceScope = builder.Build().Services.CreateScope();
IServiceProvider services = serviceScope.ServiceProvider;

try
{
    ConsoleApplication consoleService = services.GetRequiredService<ConsoleApplication>();
    var cases = new List<TestCases>()
    {
        TestCases.SingleIris,
        TestCases.MultipleIris,
        TestCases.IrisFileLogger,
        TestCases.IrisRequestCheckInvalidArgument,
        TestCases.IrisRequestCheckMissingColumn,
        TestCases.IrisRequestCheckUnknownColumn,
        TestCases.ClassGeneration,
    };
    
    foreach (var currentCase in cases)
    {
        try
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n\n####    Test case : {currentCase}\n\n");

            Console.ForegroundColor = ConsoleColor.White;
            await consoleService.RunTestCaseAsync(currentCase);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"\n\n## >>>>>> {ex}");
            Console.Error.WriteLine($"## >>>>>> Error occured for case {currentCase}: {ex}\n\n");
        }
    }

}
catch (Exception ex)
{
    Console.WriteLine($"Error Occured: {ex}");
}
