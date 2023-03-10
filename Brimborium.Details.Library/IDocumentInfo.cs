namespace Brimborium.Details;

public interface IDocumentInfo {
    FileName FileName { get; }

    FileName GetFileNameProjectRebased(Parse.ProjectData projectInfo);

    List<SourceCodeData>? LstConsumes { get; }

    List<SourceCodeData>? LstProvides { get; }
}

public record MarkdownDocumentInfo(
    FileName FileName,
    string DetailsRelativePath
) : IDocumentInfo {
    public List<string> LstHeading { get; set; } = new();
    public List<SourceCodeData>? LstConsumes { get; set; }
    public List<SourceCodeData> LstProvides { get; set; } = new();

    public List<SourceCodeData> GetLstConsumes() => this.LstConsumes ??= new();

    public List<SourceCodeData> GetLstProvides() => this.LstProvides ??= new();

    public FileName GetFileNameProjectRebased(Parse.ProjectData projectInfo) {
        if (this._FileNameProjectRebased is null) {
            this._FileNameProjectRebased = this.FileName.Rebase(projectInfo.FolderPath) ?? throw new InvalidOperationException();
        }
        return this._FileNameProjectRebased;
    }
    private FileName? _FileNameProjectRebased;

    public static MarkdownDocumentInfo Create(
            FileName fileName,
            FileName detailFolder) {
        return new MarkdownDocumentInfo(
            fileName,
            fileName.Rebase(detailFolder)?.RelativePath ?? throw new InvalidOperationException()
            );
    }

    public List<IReplacementFinder>? LstReplacementFinder { get; set; }
    public List<IReplacementFinder> GetLstReplacementFinder() => this.LstReplacementFinder ??= new();
}

public record CSharpDocumentInfo(
    FileName FileName
) : IDocumentInfo {
    public List<SourceCodeData>? LstConsumes { get; set; }

    public List<SourceCodeData>? LstProvides { get; set; }

    public List<SourceCodeData> GetLstConsumes() => this.LstConsumes ??= new();

    public List<SourceCodeData> GetLstProvides() => this.LstProvides ??= new();

    public FileName GetFileNameProjectRebased(Parse.ProjectData projectInfo) {
        if (this._FileNameProjectRebased is null) {
            this._FileNameProjectRebased = this.FileName.Rebase(projectInfo.FolderPath) ?? throw new InvalidOperationException();
        }
        return this._FileNameProjectRebased;
    }
    private FileName? _FileNameProjectRebased;
}


public record TypescriptDocumentInfo(
    FileName FileName
) : IDocumentInfo {
    public List<SourceCodeData>? LstConsumes { get; set; }

    public List<SourceCodeData>? LstProvides { get; set; }

    public List<SourceCodeData> GetLstConsumes() => this.LstConsumes ??= new();

    public List<SourceCodeData> GetLstProvides() => this.LstProvides ??= new();

    public FileName GetFileNameProjectRebased(Parse.ProjectData projectInfo) {
        if (this._FileNameProjectRebased is null) {
            this._FileNameProjectRebased = this.FileName.Rebase(projectInfo.FolderPath) ?? throw new InvalidOperationException();
        }
        return this._FileNameProjectRebased;
    }
    private FileName? _FileNameProjectRebased;
}
