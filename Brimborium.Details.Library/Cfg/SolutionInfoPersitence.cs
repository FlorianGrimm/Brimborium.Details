namespace Brimborium.Details.Cfg;

public record SolutionInfoPersitence(
     string DetailsRoot,
     string SolutionFile,
     string DetailsFolder
) {
    /*
     var detailsRoot = FileName.FromAbsolutePath(solutionInfoConfiguration.DetailsRoot ?? "");
        var solutionFile = detailsRoot.Create(solutionInfoConfiguration.SolutionFile ?? "");
        var detailsFolder = detailsRoot.Create(solutionInfoConfiguration.DetailsFolder ?? "");
        var solutionInfo = new SolutionInfo(
            detailsRoot, solutionFile, detailsFolder
        );
     */
    public List<string> ListMainProjectName { get; set; } = new List<string>();
    public List<ProjectInfoPersitence> ListMainProjectInfo { get; set; } = new List<ProjectInfoPersitence>();
    public List<ProjectInfoPersitence> ListProject { get; set; } = new List<ProjectInfoPersitence>();


    public SolutionData PostLoad(string detailJsonDirectoryPath) {
        Console.Out.WriteLine($"detailJsonDirectoryPath: {detailJsonDirectoryPath}");

        var detailsRoot = string.IsNullOrEmpty(this.DetailsRoot)
            ? detailJsonDirectoryPath
            : Path.GetFullPath(Path.Combine(detailJsonDirectoryPath, this.DetailsRoot))
            ?? throw new InvalidOperationException();
        Console.Out.WriteLine($"DetailsRoot: {DetailsRoot}");

        var detailsRootFileName = FileName.FromAbsolutePath(detailsRoot);
        var solutionFileFileName = detailsRootFileName.Create(this.SolutionFile ?? "");
        var detailsFolderFileName = detailsRootFileName.Create(this.DetailsFolder ?? "");

        var result = new SolutionData(
            detailsRootFileName, solutionFileFileName, detailsFolderFileName
        )
        //with {
        //ListMainProjectName = this.ListMainProjectName
        //.Select(                item => {
        //return item with {
        //    //FilePath = detailsRootFileName.Create(item.FilePath),
        //    //FolderPath = detailsRootFileName.Create(item.FolderPath),
        //}).ToList(),
        //ListMainProjectInfo = this.ListMainProjectInfo,
        //ListProject = this.ListProject
        //}
        ;
        /*
        var thisRooted = this with { DetailsRoot = detailsRoot };

        var result = thisRooted with {
            SolutionFile = thisRooted.GetFullPath(thisRooted.SolutionFile),
            DetailsFolder = thisRooted.GetFullPath(thisRooted.DetailsFolder),
            ListMainProjectInfo = thisRooted.ListMainProjectInfo.Select(item => {
                return item with {
                    FilePath = thisRooted.GetFullPath(item.FilePath),
                    FolderPath = thisRooted.GetFullPath(item.FolderPath),
                };
            }).ToList(),
            ListProject = thisRooted.ListProject.Select(item => {
                return item with {
                    FilePath = thisRooted.GetFullPath(item.FilePath),
                    FolderPath = thisRooted.GetFullPath(item.FolderPath),
                };
            }).ToList()
        };
        */
        return result;
    }
}
