using Brimborium.Details.Parse;

namespace Brimborium.Details;

[Brimborium.Registrator.Singleton()]
public partial class DetailsRunnerService {
    private readonly AppSettings _AppSettings;
    private readonly IFileSystem _FileSystem;
    private readonly IWatchService _WatchService;
    private readonly ISolutionAnalyzerFactory _SolutionAnalyzerFactory;
    private readonly bool _WatchEnabled;

    public DetailsRunnerService(
        IFileSystem fileSystem,
        IWatchService watchService,
        ISolutionAnalyzerFactory solutionAnalyzerFactory,
        IOptions<AppSettings> options) {
        this._AppSettings = options.Value;
        this._FileSystem = fileSystem;
        this._WatchService = watchService;
        this._SolutionAnalyzerFactory = solutionAnalyzerFactory;
        this._WatchEnabled = this._AppSettings.Watch;
    }

    public async Task<bool> ExecuteAsync(
        IRootRepository rootRepository,
        CancellationToken stoppingToken) {

        var solutionAnalyzer = this._SolutionAnalyzerFactory.GetSolutionAnalyzer(rootRepository);
        if (this._WatchEnabled) {
            var watchServiceConfigurator = new WatchServiceConfigurator(this._FileSystem);
            //await this._WatchService.Initialize(rootRepository, stoppingToken);
            await solutionAnalyzer.AnalyzeAsync(watchServiceConfigurator, stoppingToken);

            await this._WatchService.StartAsync(rootRepository, watchServiceConfigurator, stoppingToken);

            //System.Console.Out.WriteLine("Watching");
            // await host.WaitForShutdownAsync(ctsMain.Token);
            return true;
        } else {
            await solutionAnalyzer.AnalyzeAsync(DummyWatchServiceConfigurator.Instance, stoppingToken);
            return false;
        }
    }
}
