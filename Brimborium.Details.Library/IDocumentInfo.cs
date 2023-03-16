namespace Brimborium.Details;

[System.Text.Json.Serialization.JsonDerivedType(typeof(MarkdownDocumentInfo), "MarkdownDocumentInfo")]
[System.Text.Json.Serialization.JsonDerivedType(typeof(CSharpDocumentInfo), "CSharpDocumentInfo")]
[System.Text.Json.Serialization.JsonDerivedType(typeof(TypescriptDocumentInfo), "TypescriptDocumentInfo")]
public interface IDocumentInfo {
    FileName FileName { get; }

    FileName GetFileNameProjectRebased(Parse.ProjectData projectInfo);

    List<SourceCodeData>? ListConsumes { get; }

    List<SourceCodeData>? ListProvides { get; }
}

public record MarkdownDocumentInfo(
    FileName FileName,
    string DetailsRelativePath
) : IDocumentInfo {
    public List<string> LstHeading { get; set; } = new();
    public List<SourceCodeData>? ListConsumes { get; set; }
    public List<SourceCodeData> ListProvides { get; set; } = new();

    public List<SourceCodeData> GetLstConsumes() => this.ListConsumes ??= new();

    public List<SourceCodeData> GetLstProvides() => this.ListProvides ??= new();

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
            fileName.Rebase(detailFolder)?.RelativePath?.ToString() ?? throw new InvalidOperationException()
            );
    }

    public List<IReplacementFinder>? LstReplacementFinder { get; set; }
    public List<IReplacementFinder> GetLstReplacementFinder() => this.LstReplacementFinder ??= new();
}

public record CSharpDocumentInfo(
    FileName FileName
) : IDocumentInfo {
    public List<SourceCodeData>? ListConsumes { get; set; }

    public List<SourceCodeData>? ListProvides { get; set; }

    public List<SourceCodeData> GetLstConsumes() => this.ListConsumes ??= new();

    public List<SourceCodeData> GetLstProvides() => this.ListProvides ??= new();

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
    public List<SourceCodeData>? ListConsumes { get; set; }

    public List<SourceCodeData>? ListProvides { get; set; }

    public List<SourceCodeData> GetLstConsumes() => this.ListConsumes ??= new();

    public List<SourceCodeData> GetLstProvides() => this.ListProvides ??= new();

    public FileName GetFileNameProjectRebased(Parse.ProjectData projectInfo) {
        if (this._FileNameProjectRebased is null) {
            this._FileNameProjectRebased = this.FileName.Rebase(projectInfo.FolderPath) ?? throw new InvalidOperationException();
        }
        return this._FileNameProjectRebased;
    }
    private FileName? _FileNameProjectRebased;
}
