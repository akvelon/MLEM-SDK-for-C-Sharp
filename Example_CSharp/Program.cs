using Example;
using Example.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MlemApi;
using MlemApi.Serializing;

// Configure the application
IHostBuilder builder = new HostBuilder()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddTransient<ConsoleApplication>();
        services.AddTransient<IRequestValuesSerializer>(sp => new DefaultRequestValueSerializer());
        services.AddTransient(sp => LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information)).CreateLogger<MlemApiClient>());
        services.AddHttpClient<MlemApiClient>().AddHttpMessageHandler<LoggingDelegatingHandler>();
        services.AddTransient<LoggingDelegatingHandler>();
    }).UseConsoleLifetime();

using IServiceScope serviceScope = builder.Build().Services.CreateScope();
IServiceProvider services = serviceScope.ServiceProvider;
ConsoleApplication consoleService = services.GetRequiredService<ConsoleApplication>();

// Configure necessary console test cases running one by one
List<TestCases> cases = new()
{
    TestCases.SingleIris,
    TestCases.XGBoost,
    TestCases.MultipleIris,
    TestCases.IrisFileLogger,
    TestCases.IrisRequestCheckInvalidArgument,
    TestCases.IrisRequestCheckMissingColumn,
    TestCases.IrisRequestCheckUnknownColumn,
    TestCases.ClassGeneration,
    TestCases.CustomConsoleLoggerCase,
    TestCases.TextModel,
    TestCases.Wine
};

// Run one or several test cases
foreach (TestCases currentCase in cases)
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
