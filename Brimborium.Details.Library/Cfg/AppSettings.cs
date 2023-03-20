using Microsoft.Extensions.Configuration;

namespace Brimborium.Details.Cfg;

public class AppSettings {
    public string DetailsConfiguration { get; set; } = string.Empty;
    public string DetailsRoot { get; set; } = string.Empty;
    public bool Watch { get; set; }

    public void Configure(IConfiguration configuration) {
        if (!string.IsNullOrEmpty(this.DetailsConfiguration)) {
            if (string.IsNullOrEmpty(this.DetailsRoot) || (this.DetailsRoot == ".")) {
                this.DetailsConfiguration = Path.GetFullPath(this.DetailsConfiguration);
                this.DetailsRoot = Path.GetDirectoryName(this.DetailsConfiguration) ?? throw new InvalidOperationException();
            } else {
                this.DetailsRoot = Path.GetFullPath(this.DetailsRoot);
                this.DetailsConfiguration = Path.GetFullPath(
                    Path.Combine(this.DetailsRoot, this.DetailsConfiguration));
            }
        } else {
            if (string.IsNullOrEmpty(this.DetailsRoot)) {
                this.DetailsRoot = Environment.CurrentDirectory;
            } else {
                this.DetailsRoot = Path.GetFullPath(this.DetailsRoot);
            }
            this.DetailsConfiguration = Path.Combine(this.DetailsRoot, "details.json");
        }
    }

    public static void ConfigureAppSettings(
        IConfigurationBuilder configurationBuilder,
        IConfiguration configuration,
        AppSettings appSettings) {
        appSettings.Configure(configuration);
        var detailsConfiguration = appSettings.DetailsConfiguration.Trim();
        if (!string.IsNullOrEmpty(detailsConfiguration)) {
            if (File.Exists(detailsConfiguration)) {
                System.Console.WriteLine($"File found: '{detailsConfiguration}'");
            }
            if (!File.Exists(detailsConfiguration)) {
                throw new Exception($"File not found: '{detailsConfiguration}'");
            }
            configurationBuilder.AddJsonFile(detailsConfiguration, optional: false, reloadOnChange: true);
            configuration = configurationBuilder.Build();
            configuration.Bind(appSettings);
            appSettings.DetailsConfiguration = detailsConfiguration;
            appSettings.DetailsRoot = Path.GetFullPath(
                Path.Combine(
                    Path.GetDirectoryName(detailsConfiguration) ?? throw new Exception("GetDirectoryName is null"),
                    appSettings.DetailsRoot));
        }
    }


    public void ConfigureAfterBind() {
        if (!string.IsNullOrEmpty(this.DetailsConfiguration)
            && (string.IsNullOrEmpty(this.DetailsRoot)
            || !System.IO.Path.IsPathFullyQualified(this.DetailsRoot))) {
            this.DetailsRoot = Path.GetFullPath(
                Path.Combine(
                    Path.GetDirectoryName(this.DetailsConfiguration) ?? throw new Exception("GetDirectoryName is null"),
                    this.DetailsRoot));
        }
    }

    public SolutionData? ValidateConfiguration(
        IConfiguration configuration
        ) {

        if (string.IsNullOrEmpty(this.DetailsRoot)) {
            Console.Error.WriteLine("no DetailsRoot");
            return null;
        }

        var loadedSolutionData = SolutionDataUtility.LoadSolutionData(configuration);

        if (string.IsNullOrEmpty(loadedSolutionData.SolutionFile)) {
            Console.Error.WriteLine("empty SolutionFile - please specify");
            return null;
        }

        if (string.IsNullOrEmpty(loadedSolutionData.DetailsFolder)) {
            Console.Error.WriteLine("empty DetailsFolder - please specify");
            return null;
        }

        var solutionData = loadedSolutionData.PostLoad(this.DetailsRoot);
        Console.Out.WriteLine($"Final Values:");
        Console.Out.WriteLine($"DetailsRoot: {solutionData.DetailsRoot}");
        Console.Out.WriteLine($"SolutionFile: {solutionData.SolutionFile}");
        Console.Out.WriteLine($"DetailsFolder: {solutionData.DetailsFolder}");
        return solutionData;
    }

}

