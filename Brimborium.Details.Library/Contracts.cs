namespace Brimborium.Details;

public class SolutionInfoConfiguration {
    public SolutionInfoConfiguration() {
        this.ListMainProjectName = new List<string>();
        this.ListMainProjectInfo = new List<ProjectInfoPersitence>();
        this.ListProject = new List<ProjectInfoPersitence>();
    }
    public string? DetailsRoot { get; set; }
    public string? SolutionFile { get; set; }
    public string? DetailsFolder { get; set; }
    public List<string> ListMainProjectName { get; set; }
    public List<ProjectInfoPersitence> ListMainProjectInfo { get; set; }
    public List<ProjectInfoPersitence> ListProject { get; set; }
}

public record SourceCodeMatch(
    FileName FilePath,
    int Index,
    int Line,
    MatchInfo Match,
    SourceCodeMatchCSContext? CSContext = null
);

public enum MatchInfoKind { 
    Invalid,
    Paragraph,
    ParagraphCommand,
    DetailsLink,
    DetailscodeLink,
    DetailsLinkMarkdown,
    DetailscodeLinkMarkdown
}

public record MatchInfo(
    MatchInfoKind Kind,
    int MatchLength,
    string Path,
    string Command,
    string Anchor,
    string Comment
) {
    /*
    protected virtual bool PrintMembers(StringBuilder stringBuilder) {
        stringBuilder.Append($"MatchingText = \"{MatchingText}\", IsCommand = {IsCommand}, ");
        //stringBuilder.Append("Parts = [");
        //for (int idx = 0; idx < Parts.Length; idx++) {
        //    if (idx > 0) {
        //        stringBuilder.Append(", ");
        //    }
        //    stringBuilder.Append("\"").Append(Parts[idx]).Append("\"");
        //}
        //stringBuilder.Append("]");
        return true;
    }
    */
}

public record SourceCodeMatchCSContext(
    FileName FilePath,
    int Line,
    string FullName,
    string? Namespace,
    string? Type,
    string? Method
);


public interface IFileChangeReceiver {
    List<GlobPattern> GetLstGlobPattern();
    Task OnFileChangedAsync(string filePath, CancellationToken cancellationToken);
}

public record GlobPattern(
    string Extension,
    string RelativePath,
    Regex? Include,
    Regex? Exclude,
    Func<string, bool>? IsMatch
);

public interface ISolutionAnalyzer {
    Task AnalyzeAsync(SolutionInfo solutionInfo, CancellationToken cancellationToken);
}

public interface IProjectAnalyzer {
    Task AnalyzeAsync(SolutionInfo solutionInfo, ProjectInfo projectInfo, CancellationToken cancellationToken);
}