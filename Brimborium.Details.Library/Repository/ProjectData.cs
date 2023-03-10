namespace Brimborium.Details.Repository;
public record ProjectData2(
    string Name,
    FileName FilePath,
    string Language,
    FileName FolderPath
) {
    //public List<FileName> LstDocumentFileName { get; set; } = new();

    //public List<DocumentInfo> LstDocument { get; set; } = new();

    //public List<TypescriptDocumentInfo> LstTypescriptDocumentInfo { get; set; } = new();

    //public List<MarkdownDocumentInfo> LstMarkdownDocumentInfo { get; set; } = new();

    //public List<CSharpDocumentInfo> LstCSharpDocumentInfo { get; set; } = new();

    //public ProjectInfoPersitence PreSave(FileName detailsRoot) {
    //    return new ProjectInfoPersitence(
    //        this.Name,
    //        this.FilePath.Rebase(detailsRoot)?.RelativePath ?? this.FilePath.ToString(),
    //        this.Language,
    //        this.FolderPath.Rebase(detailsRoot)?.RelativePath ?? this.FolderPath.ToString());
    //}
    public Parse.ProjectData ToProjectInfo() {
        var result = new Parse.ProjectData(
            this.Name,
            this.FilePath,
            this.Language,
            this.FolderPath
            );
        return result;
    }
}
