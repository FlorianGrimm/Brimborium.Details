namespace Brimborium.Details;
// TODO https://github.com/mrlacey/CommentLinks
public static class Program {
    public static async Task Main(string[] args) {
        System.Console.Out.WriteLine("Brimborium.Details");
        System.Console.Out.WriteLine(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        var appSettings = new AppSettings();
        var hostBuilder = Host.CreateDefaultBuilder(args);
        hostBuilder.ConfigureAppConfiguration(
            (configurationBuilder) => {
                configurationBuilder.AddUserSecrets("Brimborium.Details");
            });
        hostBuilder.ConfigureAppConfiguration(
            (configurationBuilder) => {
                var configuration = configurationBuilder.Build();
                configuration.Bind(appSettings);
                configureAppSettings(configurationBuilder, configuration, appSettings);
            });
        hostBuilder.ConfigureServices((hostBuilderContext, serviceCollection) => {
            SolutionInfo? solutionInfo = appSettings.ValidateConfiguration(hostBuilderContext.Configuration);
            if (solutionInfo is not null) {
                serviceCollection.AddSingleton(solutionInfo);
            }
        });
        hostBuilder.ConfigureServices((hostBuilderContext, serviceCollection) => {
            serviceCollection.AddServicesWithRegistrator(
                a => {
                    a.FromDependencyContext(
                        Microsoft.Extensions.DependencyModel.DependencyContext.Default ?? throw new Exception("DependencyContext.Default is null"),
                        (assName) => (assName.Name is string name) && name.StartsWith("Brimborium.")
                    )
                    .AddClasses()
                    .UsingAttributes();
                });
        });
        using IHost host = hostBuilder.Build();
        var solutionInfo = host.Services.GetService<SolutionInfo>();
        if (solutionInfo is null) { return; }
        //host.WaitForShutdown
        var ctsMain = new CancellationTokenSource();
        await host.StartAsync(ctsMain.Token);

        System.Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e) {
            e.Cancel = true;
            ctsMain.Cancel();

            IHostApplicationLifetime applicationLifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
            applicationLifetime.StopApplication();
        };
        if (appSettings.Watch) {
            await host.Services.GetRequiredService<WatchService>().Initialize(solutionInfo, ctsMain.Token);
        }
        await host.Services.GetRequiredService<ISolutionAnalyzer>().AnalyzeAsync(solutionInfo, ctsMain.Token);

        if (appSettings.Watch) {
            host.Services.GetRequiredService<WatchService>().Start(ctsMain.Token);
            System.Console.Out.WriteLine("Watching");
            //await Task.Delay(-1, ctsMain.Token);
            await host.WaitForShutdownAsync(ctsMain.Token);
        }

        await host.StopAsync();

#if false
        var extension = System.IO.Path.GetExtension(detailJsonPath);
        var outputDetailJsonPath = detailJsonPath.Substring(0, detailJsonPath.Length - extension.Length) + ".stage" + extension;
        await SolutionInfoUtility.WriteSolutionInfo(outputDetailJsonPath, solutionInfo);
#endif
    } // main

    private static void configureAppSettings(
        IConfigurationBuilder configurationBuilder,
        IConfiguration configuration,
        AppSettings appSettings) {
        appSettings.Configure(configuration);
        if (!string.IsNullOrEmpty(appSettings.DetailsConfiguration)) {
            if (!System.IO.File.Exists(appSettings.DetailsConfiguration)) {
                throw new Exception($"File not found: {appSettings.DetailsConfiguration}");
            }
            var detailsConfiguration = appSettings.DetailsConfiguration;
            configurationBuilder.AddJsonFile(appSettings.DetailsConfiguration, optional: false, reloadOnChange: true);
            configuration = configurationBuilder.Build();
            configuration.Bind(appSettings);
            appSettings.DetailsConfiguration = detailsConfiguration;
            appSettings.DetailsRoot = System.IO.Path.GetFullPath(
                System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(detailsConfiguration) ?? throw new Exception("GetDirectoryName is null"),
                    appSettings.DetailsRoot));
        }
    }
}
