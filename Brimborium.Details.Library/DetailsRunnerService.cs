using Brimborium.Details.Parse;

namespace Brimborium.Details;

[Brimborium.Registrator.Singleton()]
public partial class DetailsRunnerService{
    private readonly AppSettings _AppSettings;
    private readonly WatchService _WatchService;
    private readonly ISolutionAnalyzerFactory _SolutionAnalyzerFactory;
    private readonly bool _WatchEnabled;

    public DetailsRunnerService(
        WatchService watchService,
        ISolutionAnalyzerFactory solutionAnalyzerFactory,
        IOptions<AppSettings> options) {
        this._AppSettings = options.Value;
        this._WatchService = watchService;
        this._SolutionAnalyzerFactory = solutionAnalyzerFactory;
        this._WatchEnabled = this._AppSettings.Watch;
    }

    public async Task<bool> ExecuteAsync(
        RootRepository rootRepository,
        CancellationToken stoppingToken) {

        var solutionAnalyzer =this._SolutionAnalyzerFactory.GetSolutionAnalyzer(rootRepository);
        if (this._WatchEnabled) {
            await this._WatchService.Initialize(rootRepository, stoppingToken);
            await solutionAnalyzer.AnalyzeAsync(stoppingToken);

            this._WatchService.Start(stoppingToken);
            //System.Console.Out.WriteLine("Watching");
            // await host.WaitForShutdownAsync(ctsMain.Token);
            return true;
        } else {
            await solutionAnalyzer.AnalyzeAsync(stoppingToken);
            return false;
        }
    }
}
