namespace Brimborium.Details;

//public interface IWatchService {}

[Brimborium.Registrator.Singleton]
public class WatchService {
    private readonly IServiceProvider _ServiceProvider;
    private readonly List<IFileChangeReceiver> _LstFileChangeReceiver;
    private readonly List<ReceiverGlobPattern> _LstReceiverGlobPattern;
    private SolutionInfo? _SolutionInfo;

    public WatchService(
        IServiceProvider serviceProvider,
        ILogger<WatchService> logger
        ) {
        this._ServiceProvider = serviceProvider;
        this._LstFileChangeReceiver = new List<IFileChangeReceiver>();
        this._LstReceiverGlobPattern = new List<ReceiverGlobPattern>();
    }

    public Task Initialize(SolutionInfo solutionInfo, CancellationToken cancellationToken) {
        this._SolutionInfo = solutionInfo;
        this._LstFileChangeReceiver.AddRange(
            this._ServiceProvider.GetServices<IFileChangeReceiver>());
        foreach (var fileChangeReceiver in this._LstFileChangeReceiver) {
            foreach (var globPattern in fileChangeReceiver.GetLstGlobPattern()) {
                this._LstReceiverGlobPattern.Add(
                    new ReceiverGlobPattern(
                        solutionInfo,
                        fileChangeReceiver,
                        globPattern));
            }
        }
        foreach (var rg in this._LstReceiverGlobPattern) {
            rg.Initialize();
        }
        return Task.CompletedTask;
    }

    public void Start(CancellationToken cancellationToken) {
        foreach (var rg in this._LstReceiverGlobPattern) {
            rg.Start(cancellationToken);
        }
    }
}

public record ReceiverGlobPattern(
    SolutionInfo solutionInfo,
    IFileChangeReceiver FileChangeReceiver,
    GlobPattern GlobPattern) {

    public void Initialize() {

        //solutionInfo.DetailsFolder,        this.GlobPattern.RelativePath
        //var fsw = new FileSystemWatcher(
            
        //    );
    }

    public void Start(CancellationToken cancellationToken) {
    }
}
