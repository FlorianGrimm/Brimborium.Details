namespace Brimborium.Details.Parse;

public record ProjectData(
    string Name,
    FileName FilePath,
    string Language,
    FileName FolderPath
) {
    public List<FileName> LstDocumentFileName { get; set; } = new();

    public List<DocumentInfo> LstDocument { get; set; } = new();

    public List<TypescriptDocumentInfo> LstTypescriptDocumentInfo { get; set; } = new();

    public List<MarkdownDocumentInfo> LstMarkdownDocumentInfo { get; set; } = new();

    public List<CSharpDocumentInfo> LstCSharpDocumentInfo { get; set; } = new();

    public ProjectInfoPersitence PreSave(FileName detailsRoot) {
        return new ProjectInfoPersitence(
            this.Name,
            this.FilePath.Rebase(detailsRoot)?.RelativePath ?? this.FilePath.ToString(),
            this.Language,
            this.FolderPath.Rebase(detailsRoot)?.RelativePath ?? this.FolderPath.ToString());
    }
}
