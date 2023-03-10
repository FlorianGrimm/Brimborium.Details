namespace Brimborium.Details.Watch;

public interface IFileChangeReceiver {
    List<GlobPattern> GetLstGlobPattern();
    Task OnFileChangedAsync(string filePath, CancellationToken cancellationToken);
}
