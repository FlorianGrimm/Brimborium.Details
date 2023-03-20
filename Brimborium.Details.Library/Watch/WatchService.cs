namespace Brimborium.Details.Watch;

public interface IWatchService {
    Task StartAsync(
        IRootRepository rootRepository,
        IWatchServiceConfigurator watchServiceConfigurator,
        CancellationToken cancellationToken);
}

[Brimborium.Registrator.Singleton]
public class WatchService : IWatchService {
    private readonly IFileSystem _FileSystem;
    private readonly ILogger<WatchService> _Logger;
    private WatchServiceStarted? _WatchServiceStarted;

    public WatchService(
        IFileSystem fileSystem,
        ILogger<WatchService> logger
    ) {
        this._FileSystem = fileSystem;
        this._Logger = logger;
    }

    public Task StartAsync(
        IRootRepository rootRepository,
        IWatchServiceConfigurator watchServiceConfigurator,
        CancellationToken cancellationToken) {
        WatchServiceStarted result;
        using (var old = this._WatchServiceStarted) {
            result = new WatchServiceStarted(this, rootRepository, this._FileSystem, this._Logger);
            this._WatchServiceStarted = result;
            result.Initialize(watchServiceConfigurator);
            return result.Start(cancellationToken);
        }
    }
}

public class WatchServiceStarted
    : IWatchService
    , IDisposable
    , IFileWatcherReceiver {
    private readonly WatchService _WatchService;
    private readonly IRootRepository _RootRepository;
    private readonly IFileSystem _FileSystem;

    //private readonly IWatchServiceConfigurator _WatchServiceConfigurator;
    private readonly ILogger<WatchService> _Logger;
    private IFileWatcher? _FileWatcher;

    public WatchServiceStarted(
        WatchService watchService,
        IRootRepository rootRepository,
        IFileSystem fileSystem,
        //IWatchServiceConfigurator watchServiceConfigurator,
        ILogger<WatchService> logger
    ) {
        this._WatchService = watchService;
        this._RootRepository = rootRepository;
        this._FileSystem = fileSystem;
        //this._WatchServiceConfigurator = watchServiceConfigurator;
        this._Logger = logger;
        this._FileWatcher = fileSystem.CreateFileWatcher(rootRepository.GetSolutionData().DetailsRoot);
    }

    public Task StartAsync(IRootRepository rootRepository, IWatchServiceConfigurator watchServiceConfigurator, CancellationToken cancellationToken)
        => this._WatchService.StartAsync(rootRepository, watchServiceConfigurator, cancellationToken);

    public void Initialize(IWatchServiceConfigurator watchServiceConfigurator) {
        // WatchServiceConfigurator
    }

    public Task Start(
        CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    private void Dispose(bool disposing) {
        using (var fileWatcher = this._FileWatcher) {
            if (fileWatcher is not null) {
                fileWatcher.SetReceiver(null);
            }
            this._FileWatcher = null;
        }
    }

    ~WatchServiceStarted() {
        this.Dispose(disposing: false);
    }

    public void Dispose() {
        this.Dispose(disposing: true);
        System.GC.SuppressFinalize(this);
    }

    public void OnDeleted(FileSystemEventArgs e) {
        throw new NotImplementedException();
    }

    public void OnCreated(FileSystemEventArgs e) {
        throw new NotImplementedException();
    }

    public void OnRenamed(RenamedEventArgs e) {
        throw new NotImplementedException();
    }

    public void OnError(ErrorEventArgs e) {
        using (var fileWatcher = this._FileWatcher) {
            if (fileWatcher is not null) {
                fileWatcher.SetReceiver(null);
            }
            this._FileWatcher = null;
        }
        this._FileWatcher = this._FileSystem.CreateFileWatcher(this._RootRepository.GetSolutionData().DetailsRoot);
    }
}

/*
[Singleton]
public class WatchService2 : IWatchService {
    private readonly IServiceProvider _ServiceProvider;
    private readonly List<IFileChangeReceiver> _LstFileChangeReceiver;
    private readonly List<ReceiverGlobPattern> _LstReceiverGlobPattern;
    private IWatchServiceConfigurator? _WatchServiceConfigurator;

    public WatchService2(
        IServiceProvider serviceProvider,
        ILogger<WatchService> logger
        ) {
        this._ServiceProvider = serviceProvider;
        this._LstFileChangeReceiver = new List<IFileChangeReceiver>();
        this._LstReceiverGlobPattern = new List<ReceiverGlobPattern>();
    }

    public Task Initialize(
        RootRepository rootRepository,
        CancellationToken cancellationToken) {
        this._LstFileChangeReceiver.AddRange(
            this._ServiceProvider.GetServices<IFileChangeReceiver>());
        foreach (var fileChangeReceiver in this._LstFileChangeReceiver) {
            foreach (var globPattern in fileChangeReceiver.GetLstGlobPattern()) {
                this._LstReceiverGlobPattern.Add(
                    new ReceiverGlobPattern(
                        //rootRepository,
                        fileChangeReceiver,
                        globPattern));
            }
        }
        foreach (var rg in this._LstReceiverGlobPattern) {
            rg.Initialize();
        }
        return Task.CompletedTask;
    }

    public void Start(
        RootRepository rootRepository,
        IWatchServiceConfigurator watchServiceConfigurator,
        CancellationToken cancellationToken) {
        //watchServiceConfigurator.
        foreach (var rg in this._LstReceiverGlobPattern) {
            rg.Start(cancellationToken);
        }
    }
}

public record ReceiverGlobPattern(
    //SolutionData solutionInfo,
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
*/