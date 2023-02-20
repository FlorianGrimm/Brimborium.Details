namespace Brimborium.Details;

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
        if (host.Services.GetService<SolutionInfo>() is null) { return; }

        var ctsMain = new CancellationTokenSource();
        await host.StartAsync(ctsMain.Token);

        System.Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e) {
            ctsMain.Cancel();
            e.Cancel = true;
        };

        await host.Services.GetRequiredService<MarkdownService>().ParseDetail(ctsMain.Token);
        // await host.Services.GetRequiredService<CSharpService>().ParseCSharp(ctsMain.Token);
        // await host.Services.GetRequiredService<TypeScriptService>().ParseTypeScript(ctsMain.Token);

        //await host.Services.GetRequiredService<MarkdownService>().WriteDetail(ctsMain.Token);


        if (appSettings.Watch) {
            host.Services.GetRequiredService<WatchService>().Start(ctsMain.Token);
            System.Console.Out.WriteLine("Watching");
            await Task.Delay(-1, ctsMain.Token);
        } else {
            //ctsMain.Cancel();
        }

        var ctsMainStop = new CancellationTokenSource();
        await host.StopAsync(ctsMainStop.Token);

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
            configurationBuilder.AddJsonFile(appSettings.DetailsConfiguration, optional: false, reloadOnChange: true);
            configuration = configurationBuilder.Build();
            configuration.Bind(appSettings);
        }
    }
}
