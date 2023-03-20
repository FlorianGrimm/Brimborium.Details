namespace Brimborium.Details;

// § todo.md § https://github.com/mrlacey/CommentLinks §
public class Program {
    public static async Task Main(string[] args) {
        System.Console.Out.WriteLine("Brimborium.Details");
        System.Console.Out.WriteLine(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        var appSettings = new AppSettings();
        var hostBuilder = Host.CreateDefaultBuilder(args);

#if false
        hostBuilder.ConfigureAppConfiguration(
            (configurationBuilder) => {
                configurationBuilder.AddUserSecrets("Brimborium.Details");
            });
#endif

        hostBuilder.ConfigureAppConfiguration(
            (configurationBuilder) => {
                var configuration = configurationBuilder.Build();
                configuration.Bind(appSettings);
                AppSettings.ConfigureAppSettings(configurationBuilder, configuration, appSettings);

            });

        hostBuilder.ConfigureServices(
            (hostBuilderContext, serviceCollection) => {
                serviceCollection.AddOptions<AppSettings>().Configure(
                    (appSettings) => {
                        hostBuilderContext.Configuration.Bind(appSettings);
                        appSettings.ConfigureAfterBind();
                    });
            });


        hostBuilder.ConfigureServices((hostBuilderContext, serviceCollection) => {
            serviceCollection.AddServicesWithRegistrator(
                selector => {
                    selector.FromDependencyContext(
                        Microsoft.Extensions.DependencyModel.DependencyContext.Default ?? throw new Exception("DependencyContext.Default is null"),
                        (assName) => (assName.Name is string name) && name.StartsWith("Brimborium.")
                    )
                    .AddClasses()
                    .UsingAttributes();
                });
        });

        using IHost host = hostBuilder.Build();
        var logger = host.Services.GetRequiredService<ILogger<Program>>();

        var ctsMain = new CancellationTokenSource();
        await host.StartAsync(ctsMain.Token);

        System.Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e) {
            e.Cancel = true;
            ctsMain.Cancel();

            IHostApplicationLifetime applicationLifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
            applicationLifetime.StopApplication();
        };

        var solutionDataRepository = host.Services.GetRequiredService<ISolutionDataRepository>();
        var solutionData = solutionDataRepository.GetSolutionData();
        if (solutionData is null) {
            logger.LogError("SolutionInfo is not valid");
            System.Console.Error.WriteLine("SolutionInfo is not valid");
            return;
        }
        var rootRepositoryFactory = host.Services.GetRequiredService<RootRepositoryFactory>();
        var rootRepository = rootRepositoryFactory.Get(solutionData);

        var detailsLogicService = host.Services.GetRequiredService<DetailsRunnerService>();
        var wait = await detailsLogicService.ExecuteAsync(rootRepository, ctsMain.Token);
        if (wait) {
            await host.WaitForShutdownAsync(ctsMain.Token);
        } else {
            await host.StopAsync();
        }

#if false
        var extension = System.IO.Path.GetExtension(detailJsonPath);
        var outputDetailJsonPath = detailJsonPath.Substring(0, detailJsonPath.Length - extension.Length) + ".stage" + extension;
        await SolutionInfoUtility.WriteSolutionInfo(outputDetailJsonPath, solutionInfo);
#endif
    } // main
}
