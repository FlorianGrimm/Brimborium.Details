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
    //int Index,
    //int Line,
    MatchInfo Match,
    SourceCodeMatchCSContext? CSContext = null
) {
}

public enum MatchInfoKind {
    Invalid,
    Anchor,
    Paragraph,
    ParagraphCommand,
    DetailsLink,
    DetailscodeLink,
    DetailsLinkMarkdown,
    DetailscodeLinkMarkdown
}

public record MatchInfo(
    MatchInfoKind Kind,
    PathInfo MatchPath,
    Range MatchRange,
    PathInfo Path,
    string Command,
    PathInfo Anchor,
    string Comment,
    int Line = 0
) {
    public bool IsCommand => !string.IsNullOrEmpty(this.Command);

    public int MatchLength {
        get {
            return this.MatchRange.End.Value - this.MatchRange.Start.Value;
        }
    }
}

public record SourceCodeMatchCSContext(
    //FileName FilePath,
    //int Line,
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
    //Task AnalyzeAsync(SolutionInfo solutionInfo, ProjectInfo projectInfo, CancellationToken cancellationToken);
}

public interface IDocumentInfo {
    FileName FileName { get; }

    FileName GetFileNameProjectRebased(ProjectInfo projectInfo);

    List<SourceCodeMatch>? LstConsumes { get; }

    List<SourceCodeMatch>? LstProvides { get; }
}

public record MarkdownDocumentInfo(
    FileName FileName,
    string DetailsRelativePath
) : IDocumentInfo {
    public List<string> LstHeading { get; set; } = new();
    public List<SourceCodeMatch>? LstConsumes { get; set; }
    public List<SourceCodeMatch> LstProvides { get; set; } = new();

    public List<SourceCodeMatch> GetLstConsumes() => this.LstConsumes ??= new();

    public List<SourceCodeMatch> GetLstProvides() => this.LstProvides ??= new();

    public FileName GetFileNameProjectRebased(ProjectInfo projectInfo) {
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
            fileName.Rebase(detailFolder)?.RelativePath ?? throw new InvalidOperationException()
            );
    }

    public List<IReplacementFinder>? LstReplacementFinder { get; set; }
    public List<IReplacementFinder> GetLstReplacementFinder() => this.LstReplacementFinder ??= new();
}

public record CSharpDocumentInfo(
    FileName FileName
) : IDocumentInfo {
    public List<SourceCodeMatch>? LstConsumes { get; set; }

    public List<SourceCodeMatch>? LstProvides { get; set; }

    public List<SourceCodeMatch> GetLstConsumes() => this.LstConsumes ??= new();

    public List<SourceCodeMatch> GetLstProvides() => this.LstProvides ??= new();

    public FileName GetFileNameProjectRebased(ProjectInfo projectInfo) {
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
    public List<SourceCodeMatch>? LstConsumes { get; set; }

    public List<SourceCodeMatch>? LstProvides { get; set; }

    public List<SourceCodeMatch> GetLstConsumes() => this.LstConsumes ??= new();

    public List<SourceCodeMatch> GetLstProvides() => this.LstProvides ??= new();

    public FileName GetFileNameProjectRebased(ProjectInfo projectInfo) {
        if (this._FileNameProjectRebased is null) {
            this._FileNameProjectRebased = this.FileName.Rebase(projectInfo.FolderPath) ?? throw new InvalidOperationException();
        }
        return this._FileNameProjectRebased;
    }
    private FileName? _FileNameProjectRebased;
}

public interface IMatchCommand {
    bool IsMatching(MatchInfo matchInfo);
    Task ExecuteAsync(
        SourceCodeMatch sourceCodeMatch,
        MarkdownDocumentWriter markdownDocumentWriter,
        IReplacementFinder? replacementFinder,
        DetailContextCache? cache,
        CancellationToken cancellationToken);
    
    IReplacementFinder? GetReplacementFinder(
        MarkdownDocumentInfo markdownDocumentInfo,
        SourceCodeMatch sourceCodeMatch);
}

public interface IReplacementFinder {
    IMatchCommand Command { get; }
    SourceCodeMatch SourceCodeMatch { get; }

    /// <summary>
    /// The parser will call this method for the next blocks it finds in the document.
    /// </summary>
    /// <param name="block">the block to find.</param>
    /// <returns>true if found</returns>
    bool VisitBlock(Block block);

    /// <summary>
    /// The parser notify you to give up.
    /// </summary>
    void VisitNotFound();
}

public record ProjectProjectInfo(Project Project, ProjectInfo ProjectInfo);