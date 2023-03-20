namespace Brimborium.Details.Watch;

public interface IWatchServiceConfigurator {
    void AddDirectory(ProjectData project, FileName folderPath);
    void AddFile(ProjectData project, FileName document);
}
public interface IWatchServiceChanges {
}

public record WatchServiceConfiguratorFileData(
    DateTime LastWriteTimeUtc,
    string ProjectFilePath
);

public class DummyWatchServiceConfigurator : IWatchServiceConfigurator {
    public static DummyWatchServiceConfigurator? _Instance;
    public static IWatchServiceConfigurator Instance => _Instance ?? new DummyWatchServiceConfigurator();
    public DummyWatchServiceConfigurator() {
    }

    public void AddDirectory(ProjectData project, FileName folderPath) {
    }

    public void AddFile(ProjectData project, FileName document) {
    }
}
[Brimborium.Registrator.Transient]
public class WatchServiceConfigurator : IWatchServiceConfigurator {
    private readonly Dictionary<string, ProjectData> _DictProjectDataFile;
    private readonly Dictionary<string, ProjectData> _DictProjectDataFolder;
    private readonly Dictionary<string, WatchServiceConfiguratorFileData> _DictFileNameLastWriteTime;
    private readonly IFileSystem _FileSystem;

    //
    public WatchServiceConfigurator(
        IFileSystem fileSystem) {
        this._DictProjectDataFile = new Dictionary<string, ProjectData>(StringComparer.OrdinalIgnoreCase);
        this._DictProjectDataFolder = new Dictionary<string, ProjectData>(StringComparer.OrdinalIgnoreCase);
        this._DictFileNameLastWriteTime = new Dictionary<string, WatchServiceConfiguratorFileData>(StringComparer.OrdinalIgnoreCase);
        this._FileSystem = fileSystem;
    }

    public void AddDirectory(ProjectData project, FileName folderPath) {
        var absoluteFilePath = project.FilePath.AbsolutePath;
        if (absoluteFilePath is null) {
            throw new ArgumentException(nameof(project));
        }
        var absoluteFolderPath = folderPath.AbsolutePath;
        if (absoluteFolderPath is null) {
            throw new ArgumentException(nameof(absoluteFolderPath));
        }
        this._DictProjectDataFile[absoluteFilePath] = project;
        this._DictProjectDataFolder[absoluteFolderPath] = project;
    }

    public void AddFile(ProjectData project, FileName document) {
        var absolutePath = project.FilePath.AbsolutePath;
        if (absolutePath is null) {
            throw new ArgumentException(nameof(project));
        }
        var lastWriteTimeUtc = this._FileSystem.GetLastWriteTimeUtc(project.FilePath);
        this._DictFileNameLastWriteTime[absolutePath] = new WatchServiceConfiguratorFileData(lastWriteTimeUtc, project.FilePath.AbsolutePath!);
    }
}
