namespace Brimborium.Details;

public record ProjectInfo(
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
public record ProjectInfoPersitence(
    string Name,
    string FilePath,
    string Language,
    string FolderPath
) {
    public List<DocumentInfoPersitence> Documents { get; set; } = new List<DocumentInfoPersitence>();

    public ProjectInfo PostLoad(FileName detailsRoot) {
        return new ProjectInfo(
            this.Name,
            detailsRoot.Create(this.FilePath),
            this.Language,
            detailsRoot.Create(this.FolderPath));
    }
}
