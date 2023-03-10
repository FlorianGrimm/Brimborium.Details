namespace Brimborium.Details.Utility;
public interface IFileSystem {
    IEnumerable<FileName> EnumerateFiles(FileName path, string searchPattern, SearchOption searchOption);

    Task<string[]> ReadAllLinesAsync(FileName path, Encoding encoding, CancellationToken cancellationToken = default);

    Task<string> ReadAllTextAsync(FileName path, Encoding encoding, CancellationToken cancellationToken = default);

    Task WriteAllTextAsync(FileName path, Encoding encoding, string content, CancellationToken cancellationToken = default);
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
}
