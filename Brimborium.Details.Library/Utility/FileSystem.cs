namespace Brimborium.Details.Utility;

public interface IFileSystem {
    IEnumerable<FileName> EnumerateFiles(FileName path, string searchPattern, SearchOption searchOption);

    DateTime GetLastWriteTimeUtc(FileName filePath);

    Task<string[]> ReadAllLinesAsync(FileName path, Encoding encoding, CancellationToken cancellationToken = default);

    Task<string> ReadAllTextAsync(FileName path, Encoding encoding, CancellationToken cancellationToken = default);

    Task WriteAllTextAsync(FileName path, Encoding encoding, string content, CancellationToken cancellationToken = default);

    IFileWatcher CreateFileWatcher(FileName path);
}

[Singleton]
public class FileSystem : IFileSystem {
    public IEnumerable<FileName> EnumerateFiles(FileName path, string searchPattern, SearchOption searchOption) {
        var lstFiles = Directory.EnumerateFiles(
            path.AbsolutePath ?? throw new InvalidOperationException("path.AbsolutePath is null"),
            searchPattern,
            searchOption);
        var result = new List<FileName>();
        foreach (var item in lstFiles) {
            if (path.RootFolder is not null) {
                result.Add(path.RootFolder.Create(item));
            } else {
                result.Add(path.Create(item));
            }
        }
        return result;
    }

    public DateTime GetLastWriteTimeUtc(FileName filePath) {
        return File.GetLastWriteTimeUtc(
            filePath.AbsolutePath ?? throw new InvalidOperationException("filePath.AbsolutePath is null"));
    }

    public async Task<string[]> ReadAllLinesAsync(FileName path, Encoding encoding, CancellationToken cancellationToken = default) {
        return await File.ReadAllLinesAsync(
            path.AbsolutePath ?? throw new InvalidOperationException("path.AbsolutePath is null"),
            encoding,
            cancellationToken);
    }

    public async Task<string> ReadAllTextAsync(FileName path, Encoding encoding, CancellationToken cancellationToken = default) {
        return await File.ReadAllTextAsync(
            path.AbsolutePath ?? throw new InvalidOperationException("path.AbsolutePath is null"),
            encoding,
            cancellationToken);
    }

    public async Task WriteAllTextAsync(FileName path, Encoding encoding, string content, CancellationToken cancellationToken = default) {
        await File.WriteAllTextAsync(
            path.AbsolutePath ?? throw new InvalidOperationException("path.AbsolutePath is null"),
            content,
            encoding,
            cancellationToken
            );
    }

    public IFileWatcher CreateFileWatcher(FileName path) {
        var watcher = new FileSystemWatcher();
        watcher.Path = path.AbsolutePath ?? throw new InvalidOperationException("path.AbsolutePath is null");
        watcher.NotifyFilter =
            NotifyFilters.FileName
            | NotifyFilters.DirectoryName
            | NotifyFilters.Size
            | NotifyFilters.LastWrite
            | NotifyFilters.CreationTime
            ;
        return FileWatcher.Create(watcher);
    }
}
