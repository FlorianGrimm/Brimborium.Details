namespace Brimborium.Details.Cfg;

public record ProjectInfoPersitence(
    string Name,
    string FilePath,
    string Language,
    string FolderPath
) {
    public List<DocumentInfoPersitence> Documents { get; set; } = new List<DocumentInfoPersitence>();

    public Parse.ProjectData PostLoad(FileName detailsRoot) {
        return new Parse.ProjectData(
            this.Name,
            detailsRoot.Create(this.FilePath),
            this.Language,
            detailsRoot.Create(this.FolderPath));
    }
}
