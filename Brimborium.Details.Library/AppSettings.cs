namespace Brimborium.Details;


public class AppSettings {
    public string DetailsConfiguration { get; set; } = string.Empty;
    public string DetailsRoot { get; set; } = string.Empty;
    public bool Watch { get; set; }

    public void Configure(IConfiguration configuration) {
#if false
        if (string.IsNullOrEmpty(this.DetailsConfiguration)) {
            var lstDetailsJsonFileName = System.IO.Directory.EnumerateFiles(
                System.Environment.CurrentDirectory,
                "details.json",
                new EnumerationOptions() {
                    RecurseSubdirectories = false
                })
                .ToList();
            if (lstDetailsJsonFileName.Count == 1) {
                this.DetailsConfiguration = lstDetailsJsonFileName[0];
                this.DetailsRoot = System.Environment.CurrentDirectory;
                return;
            }
        }
        if (string.IsNullOrEmpty(this.DetailsConfiguration)) {
            var lstDetailsJsonFileName = System.IO.Directory.EnumerateFiles(
                System.Environment.CurrentDirectory,
                "details.json",
                new EnumerationOptions() {
                    RecurseSubdirectories = true,
                    MaxRecursionDepth = 2
                })
                .ToList();
            if (lstDetailsJsonFileName.Count == 1) {
                this.DetailsConfiguration = lstDetailsJsonFileName[0];
                this.DetailsRoot = System.Environment.CurrentDirectory;
            }
        }
        if (string.IsNullOrEmpty(this.DetailsConfiguration)) {
            var parentDirectory = System.IO.Path.GetDirectoryName(System.Environment.CurrentDirectory);
            if (parentDirectory is not null) {
                var lstDetailsJsonFileName = System.IO.Directory.EnumerateFiles(
                    parentDirectory,
                    "details.json",
                    new EnumerationOptions() {
                        RecurseSubdirectories = false
                    })
                    .ToList();
                if (lstDetailsJsonFileName.Count == 1) {
                    this.DetailsConfiguration = lstDetailsJsonFileName[0];
                    this.DetailsRoot = parentDirectory;
                }
            }
        }
        if (string.IsNullOrEmpty(this.DetailsConfiguration)) {
            if (string.IsNullOrEmpty(this.DetailsRoot)) {
                this.DetailsRoot = System.Environment.CurrentDirectory;
            }
        }
#endif
        if (!string.IsNullOrEmpty(this.DetailsConfiguration)) {
            if (string.IsNullOrEmpty(this.DetailsRoot)) {
                this.DetailsConfiguration = System.IO.Path.GetFullPath(this.DetailsConfiguration);
                this.DetailsRoot = System.IO.Path.GetDirectoryName(this.DetailsConfiguration) ?? throw new InvalidOperationException();
            } else {
                this.DetailsRoot = System.IO.Path.GetFullPath(this.DetailsRoot);
                this.DetailsConfiguration = System.IO.Path.GetFullPath(
                    System.IO.Path.Combine(this.DetailsRoot, this.DetailsConfiguration));
            }
        } else {
            if (string.IsNullOrEmpty(this.DetailsRoot)) {
                this.DetailsRoot = System.Environment.CurrentDirectory;
            } else {
                this.DetailsRoot = System.IO.Path.GetFullPath(this.DetailsRoot);
            }
            this.DetailsConfiguration = System.IO.Path.Combine(this.DetailsRoot, "details.json");
        }
    }

    public SolutionInfo? ValidateConfiguration(
        IConfiguration configuration
        ) {

        if (string.IsNullOrEmpty(this.DetailsRoot)) {
            System.Console.Error.WriteLine("no DetailsRoot");
            return null;
        }

        var loadedSolutionInfo = SolutionInfoUtility.LoadSolutionInfo(
            configuration);

        if (string.IsNullOrEmpty(loadedSolutionInfo.SolutionFile)) {
            System.Console.Error.WriteLine("empty SolutionFile - please specify");
            return null;
        }

        if (string.IsNullOrEmpty(loadedSolutionInfo.DetailsFolder)) {
            System.Console.Error.WriteLine("empty DetailsFolder - please specify");
            return null;
        }

        var solutionInfo = loadedSolutionInfo.PostLoad(this.DetailsRoot);
        System.Console.Out.WriteLine($"Final Values:");
        System.Console.Out.WriteLine($"DetailsRoot: {solutionInfo.DetailsRoot}");
        System.Console.Out.WriteLine($"SolutionFile: {solutionInfo.SolutionFile}");
        System.Console.Out.WriteLine($"DetailsFolder: {solutionInfo.DetailsFolder}");
        return solutionInfo;
    }
}

