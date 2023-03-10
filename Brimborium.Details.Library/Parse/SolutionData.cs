namespace Brimborium.Details.Parse;

public record SolutionData(
     FileName DetailsRoot,
     FileName SolutionFile,
     FileName DetailsFolder
) {
    public List<string> ListMainProjectName { get; set; } = new List<string>();
    public List<ProjectData> ListMainProjectInfo { get; set; } = new List<ProjectData>();
    public List<ProjectData> ListProject { get; set; } = new List<ProjectData>();

    public SolutionInfoPersitence PreSave(string detailsJsonFullPath) {
        var detailsJsonDirectoryPath = Path.GetDirectoryName(detailsJsonFullPath)
            ?? throw new InvalidOperationException();
        var detailsDirectoryPathFileName = FileName.FromAbsolutePath(detailsJsonDirectoryPath);

        var result = new SolutionInfoPersitence(
            DetailsRoot: this.DetailsRoot.Rebase(detailsDirectoryPathFileName)?.RelativePath ?? string.Empty,
            SolutionFile: this.SolutionFile.Rebase(detailsDirectoryPathFileName)?.RelativePath ?? string.Empty,
            DetailsFolder: this.DetailsFolder.Rebase(detailsDirectoryPathFileName)?.RelativePath ?? string.Empty
        ) {
            ListMainProjectName = this.ListMainProjectName.ToList(),
            ListMainProjectInfo = this.ListMainProjectInfo.Select(item => item.PreSave(this.DetailsRoot)).ToList(),
            ListProject = this.ListProject.Select(item => item.PreSave(this.DetailsRoot)).ToList(),
        };
        return result;
    }
}
