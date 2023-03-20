namespace Brimborium.Details.Utility;

public interface IFileWatcher : IDisposable {
    void SetReceiver(IFileWatcherReceiver? receiver);
}

public interface IFileWatcherReceiver {
    void OnDeleted(FileSystemEventArgs e);
    void OnCreated(FileSystemEventArgs e);
    void OnRenamed(RenamedEventArgs e);
    void OnError(ErrorEventArgs e);
}

public class FileWatcher : IFileWatcher, IDisposable {
    public static FileWatcher Create(
        FileSystemWatcher watcher
        ) {
        var result = new FileWatcher(watcher);
        watcher.Deleted += result.OnDeleted;
        watcher.Created += result.OnCreated;
        watcher.Changed += result.OnChanged;
        watcher.Renamed += result.OnRenamed;
        watcher.Error += result.OnError;
        watcher.IncludeSubdirectories = true;
        return result;
    }

    private FileSystemWatcher? _Watcher;
    private IFileWatcherReceiver? _Receiver;

    public FileWatcher(FileSystemWatcher watcher) {
        this._Watcher = watcher;
    }

    public void SetReceiver(IFileWatcherReceiver? receiver) {
        this._Receiver = receiver;
        if (this._Watcher is not null) {
            this._Watcher.EnableRaisingEvents = receiver is not null;
        }
    }

    private void OnDeleted(object sender, FileSystemEventArgs e) {
        if (this._Receiver is not null){
            this._Receiver.OnDeleted(e);
        }
    }

    private void OnCreated(object sender, FileSystemEventArgs e) {
        if (this._Receiver is not null){
            this._Receiver.OnCreated(e);
        }
    }

    private void OnChanged(object sender, FileSystemEventArgs e) {
        Console.WriteLine($"Created: {e.FullPath}");
    }

    private void OnRenamed(object sender, RenamedEventArgs e) {
         if (this._Receiver is not null){
            this._Receiver.OnRenamed(e);
        }
    }

    private void OnError(object sender, ErrorEventArgs e) {
         if (this._Receiver is not null){
            this._Receiver.OnError(e);
        }
    }

    private void Dispose(bool disposing) {
        using (var watcher = this._Watcher) {
            if (watcher is not null) {
                watcher.EnableRaisingEvents = false;
            }
            this._Watcher = null;
        }
    }

    public void Dispose() {
        this.Dispose(disposing: true);
        System.GC.SuppressFinalize(this);
    }

}
