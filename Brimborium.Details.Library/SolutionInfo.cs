namespace Brimborium.Details;

public record SolutionInfo(
     FileName DetailsRoot,
     FileName SolutionFile,
     FileName DetailsFolder
) {
    public List<string> ListMainProjectName { get; set; } = new List<string>();
    public List<ProjectInfo> ListMainProjectInfo { get; set; } = new List<ProjectInfo>();
    public List<ProjectInfo> ListProject { get; set; } = new List<ProjectInfo>();

    /*
    public string GetRelativePath(string documentFilePath) {
        if (documentFilePath.StartsWith(this.DetailsRoot)) {
            return GetNormalizedPath(documentFilePath.Substring(this.DetailsRoot.Length + 1));
        } else {
            return GetNormalizedPath(System.IO.Path.GetRelativePath(this.DetailsRoot, documentFilePath));
        }
    }

    public string GetFullPath(string documentFilePath) {
        return System.IO.Path.GetFullPath(
            System.IO.Path.Combine(
                this.DetailsRoot,
                GetOsPath(documentFilePath)))
            ?? throw new System.Exception($"GetFullPath failed for {documentFilePath}");
    }

    public static string GetOsPath(string documentFilePath) {
        return documentFilePath.Replace('/', System.IO.Path.DirectorySeparatorChar);
    }

    public static string GetNormalizedPath(string documentFilePath) {
        return documentFilePath.Replace(System.IO.Path.DirectorySeparatorChar, '/');
    }
    */
    
    public SolutionInfoPersitence PreSave(string detailsJsonFullPath) {
        var detailsJsonDirectoryPath = System.IO.Path.GetDirectoryName(detailsJsonFullPath)
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


    public SolutionInfo PostLoad(string detailJsonDirectoryPath) {
        System.Console.Out.WriteLine($"detailJsonDirectoryPath: {detailJsonDirectoryPath}");

        var detailsRoot = string.IsNullOrEmpty(this.DetailsRoot)
            ? detailJsonDirectoryPath
            : System.IO.Path.GetFullPath(System.IO.Path.Combine(detailJsonDirectoryPath, this.DetailsRoot))
            ?? throw new InvalidOperationException();
        System.Console.Out.WriteLine($"DetailsRoot: {DetailsRoot}");

        var detailsRootFileName = FileName.FromAbsolutePath(detailsRoot);
        var solutionFileFileName = detailsRootFileName.Create(this.SolutionFile ?? "");
        var detailsFolderFileName = detailsRootFileName.Create(this.DetailsFolder ?? "");

        var result = new SolutionInfo(
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
