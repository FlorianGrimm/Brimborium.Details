namespace Brimborium.Details;

public class SolutionInfoConfiguration {
    public SolutionInfoConfiguration() {
        this.ListMainProjectName = new List<string>();
        this.ListMainProjectInfo = new List<ProjectInfo>();
        this.ListProject = new List<ProjectInfo>();
    }
    public string? DetailsRoot { get; set; }
    public string? SolutionFile { get; set; }
    public string? DetailsFolder { get; set; }
    public List<string> ListMainProjectName { get; set; }
    public List<ProjectInfo> ListMainProjectInfo { get; set; }
    public List<ProjectInfo> ListProject { get; set; }
}

public record SolutionInfo(
     string DetailsRoot,
     string SolutionFile,
     string DetailsFolder
) {
    public List<string> ListMainProjectName { get; set; } = new List<string>();
    public List<ProjectInfo> ListMainProjectInfo { get; set; } = new List<ProjectInfo>();
    public List<ProjectInfo> ListProject { get; set; } = new List<ProjectInfo>();

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

    public SolutionInfo PostLoad(string detailJsonDirectoryPath) {
        System.Console.Out.WriteLine($"detailJsonDirectoryPath: {detailJsonDirectoryPath}");

        var detailsRoot = string.IsNullOrEmpty(this.DetailsRoot)
            ? detailJsonDirectoryPath
            : System.IO.Path.GetFullPath(System.IO.Path.Combine(detailJsonDirectoryPath, this.DetailsRoot))
            ?? throw new InvalidOperationException();
        System.Console.Out.WriteLine($"DetailsRoot: {DetailsRoot}");

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
        return result;
    }

    public SolutionInfo PreSave(string detailJsonFullPath) {
        var detailDirectoryPath = System.IO.Path.GetDirectoryName(detailJsonFullPath)
            ?? throw new InvalidOperationException();

        var detailsRoot = string.Equals(detailDirectoryPath, this.DetailsRoot, StringComparison.InvariantCultureIgnoreCase)
            ? string.Empty
            : System.IO.Path.GetRelativePath(detailDirectoryPath, this.DetailsRoot);

        var result = this with {
            DetailsRoot = detailsRoot,
            SolutionFile = this.GetRelativePath(this.SolutionFile),
            DetailsFolder = this.GetRelativePath(this.DetailsFolder),
        };
        return result;
    }
}

public record ProjectInfo(
    string Name,
    string FilePath,
    string Language,
    string FolderPath
) {
    public List<DocumentInfo> Documents { get; set; } = new List<DocumentInfo>();
}

public record DocumentInfo(
    string FilePath,
    string Language
);

public record SourceCodeMatch(
    string FilePath,
    int Index,
    int Line,
    MatchInfo Match,
    SourceCodeMatchCSContext? CSContext = null
);

public record MatchInfo(
    string MatchingText,
    bool IsCommand,
    string[] Parts
) {
    protected virtual bool PrintMembers(StringBuilder stringBuilder) {
        stringBuilder.Append($"MatchingText = \"{MatchingText}\", IsCommand = {IsCommand}, ");
        stringBuilder.Append("Parts = [");
        for (int idx = 0; idx < Parts.Length; idx++) {
            if (idx > 0) {
                stringBuilder.Append(", ");
            }
            stringBuilder.Append("\"").Append(Parts[idx]).Append("\"");
        }
        stringBuilder.Append("]");
        return true;
    }
}

public record SourceCodeMatchCSContext(
    string FilePath,
    int Line,
    string FullName,
    string? Namespace,
    string? Type,
    string? Method
);