namespace Brimborium.Details;

// § todo.md § test §
public class DetailContext {
    private readonly SolutionInfo _SolutionInfo;

    private readonly Dictionary<string, ProjectInfo> _ProjectInfoByFilePath;
    private readonly Dictionary<string, ProjectId> _ProjectIdByFilePath;
    private readonly Dictionary<ProjectId, ProjectInfo> _ProjectInfoByProjectId;

    public SolutionInfo SolutionInfo => this._SolutionInfo;

    public Dictionary<string, ProjectInfo> ProjectInfoByFilePath => this._ProjectInfoByFilePath;

    [JsonIgnore]
    public Dictionary<string, ProjectId> ProjectIdByFilePath => this._ProjectIdByFilePath;

    [JsonIgnore]
    public Dictionary<ProjectId, ProjectInfo> ProjectInfoByProjectId => this._ProjectInfoByProjectId;

    public ProjectInfo? DetailsProject { get; set; }

    public DetailContext(
        SolutionInfo solutionInfo
    ) {
        this._SolutionInfo = solutionInfo;
        this._ProjectInfoByFilePath = new(StringComparer.InvariantCultureIgnoreCase);
        this._ProjectIdByFilePath = new(StringComparer.InvariantCultureIgnoreCase);
        this._ProjectInfoByProjectId = new();
    }

    public List<ProjectInfo> GetLstProjectInfo() => new List<ProjectInfo>(this._ProjectInfoByFilePath.Values);

    public void SetDetailsProject(ProjectInfo markdownProjectInfo) {
        lock (this) {
            this.DetailsProject = markdownProjectInfo;
            this._ProjectInfoByFilePath[markdownProjectInfo.FilePath.AbsolutePath!] = markdownProjectInfo;
        }
    }

    public ProjectInfo AddCSharpProject(string projectFilePath, string name, ProjectId id, List<FileName> lstDocument) {
        var projectFileName = this._SolutionInfo.DetailsRoot.CreateWithAbsolutePath(projectFilePath);
        var projectFolderFileName = projectFileName.GetParentDirectory() ?? FileName.Empty;

        ProjectInfo projectInfo = new ProjectInfo(
            Name: name,
            FilePath: projectFileName,
            Language: "CSharp",
            FolderPath: projectFolderFileName
        );

        foreach (var document in lstDocument.OrderBy(document => document.AbsolutePath, StringComparer.OrdinalIgnoreCase)) {
            var documentFileName = document.Rebase(projectFolderFileName);
            if (documentFileName is null) { continue; }
            projectInfo.LstDocumentFileName.Add(documentFileName);
        }

        lock (this) {
            this._ProjectInfoByFilePath[projectInfo.FilePath.AbsolutePath!] = projectInfo;
            this._ProjectIdByFilePath[projectInfo.FilePath.AbsolutePath!] = id;
            this._ProjectInfoByProjectId[id] = projectInfo;
        }

        return projectInfo;
    }

    public void AddCSharpDocumentInfo(ProjectId projectId, List<CSharpDocumentInfo> lstCSharpDocumentInfo) {
        var lstSortedCSharpDocumentInfo = lstCSharpDocumentInfo.OrderBy(di => di.FileName.AbsolutePath, StringComparer.OrdinalIgnoreCase);
        lock (this) {
            if (this._ProjectInfoByProjectId.TryGetValue(projectId, out var projectInfo)) {
                projectInfo.LstCSharpDocumentInfo.Clear();
                projectInfo.LstCSharpDocumentInfo.AddRange(lstSortedCSharpDocumentInfo);
            }
        }
    }

    public void AddMarkdownDocumentInfo(
        ProjectInfo projectInfo,
        MarkdownDocumentInfo documentInfo) {
        lock (this) {
            projectInfo.LstMarkdownDocumentInfo.Add(documentInfo);
        }
    }

    public void AddTypescriptProject(ProjectInfo typescriptProject, List<TypescriptDocumentInfo> lstDocumentInfo) {
        lock (this) {
            typescriptProject.LstTypescriptDocumentInfo = lstDocumentInfo;
            this._ProjectInfoByFilePath[typescriptProject.FilePath.AbsolutePath!] = typescriptProject;
        }
    }

    public List<ProjectDocumentInfo> GetLstProjectDocumentInfo(DetailContextCache? cache) {
        if (cache?.CacheLstProjectDocumentInfo is List<ProjectDocumentInfo> resultCached) { return resultCached; }

        var result = new List<ProjectDocumentInfo>();
        foreach (var projectInfo in this._ProjectInfoByFilePath.Values) {
            foreach (var documentInfo in projectInfo.LstMarkdownDocumentInfo) {
                result.Add(new ProjectDocumentInfo(projectInfo, documentInfo));
            }
            foreach (var documentInfo in projectInfo.LstCSharpDocumentInfo) {
                result.Add(new ProjectDocumentInfo(projectInfo, documentInfo));
            }

            foreach (var documentInfo in projectInfo.LstTypescriptDocumentInfo) {
                result.Add(new ProjectDocumentInfo(projectInfo, documentInfo));
            }
        }
        if (cache is not null) {
            cache.CacheLstProjectDocumentInfo = result;
        }
        return result;
    }

    public List<ProjectDocumentInfo> GetLstMarkdownDocumentInfo() {
        var result = new List<ProjectDocumentInfo>();
        foreach (var projectInfo in this._ProjectInfoByFilePath.Values) {
            foreach (var documentInfo in projectInfo.LstMarkdownDocumentInfo) {
                result.Add(new ProjectDocumentInfo(projectInfo, documentInfo));
            }
        }
        return result;
    }

    public IEnumerable<ProjectDocumentInfoSourceCodeMatch> GetLstConsumes(DetailContextCache? cache) {
        var result = new List<ProjectDocumentInfoSourceCodeMatch>();
        foreach (var projectDocumentInfo in this.GetLstProjectDocumentInfo(cache)) {
            if (projectDocumentInfo.DocumentInfo.LstConsumes is null) { continue; }
            foreach (var sourceCodeMatch in projectDocumentInfo.DocumentInfo.LstConsumes) {
                result.Add(new ProjectDocumentInfoSourceCodeMatch(
                    projectDocumentInfo.ProjectInfo,
                    projectDocumentInfo.DocumentInfo,
                    sourceCodeMatch));
            }
        }
        return result;
    }

    public IEnumerable<ProjectDocumentInfoSourceCodeMatch> GetLstProvides(DetailContextCache? cache) {
        var result = new List<ProjectDocumentInfoSourceCodeMatch>();
        foreach (var projectDocumentInfo in this.GetLstProjectDocumentInfo(cache)) {
            if (projectDocumentInfo.DocumentInfo.LstProvides is null) { continue; }
            foreach (var sourceCodeMatch in projectDocumentInfo.DocumentInfo.LstProvides) {
                result.Add(new ProjectDocumentInfoSourceCodeMatch(
                    projectDocumentInfo.ProjectInfo,
                    projectDocumentInfo.DocumentInfo,
                    sourceCodeMatch));
            }
        }

        return result;
    }

    public List<ProjectDocumentInfoSourceCodeMatch> QueryPath(
        PathInfo searchPath, 
        DetailContextCache? cache) {
        var result = new List<ProjectDocumentInfoSourceCodeMatch>();
        var (searchPathFileName, searchPathDocumentInfo) = this.FindDocumentInfo(searchPath, cache);
        if (searchPathDocumentInfo is null) { return result; }
        foreach (var item in this.GetLstProvides(cache)) {
            var (itemFileName, itemDocumentInfo) = this.FindDocumentInfo(item.SourceCodeMatch.Match.Path, cache);
            if (itemFileName is null || itemDocumentInfo is null) { continue; }
            if (itemFileName.Equals(searchPathFileName)) {
                if (searchPath.IsContentPathEqual(item.SourceCodeMatch.Match.Path)) {
                    result.Add(item);
                }
            }
        }
        return result;
    }


    public List<ProjectDocumentInfoSourceCodeMatch> QueryPathChildren(
        PathInfo searchPath,
        DetailContextCache? cache) {
        var result = new List<ProjectDocumentInfoSourceCodeMatch>();
        var (searchPathFileName, searchPathDocumentInfo) = this.FindDocumentInfo(searchPath, cache);
        if (searchPathDocumentInfo is null) { return result; }
        foreach (var item in this.GetLstProvides(cache)) {
            var (itemFileName, itemDocumentInfo) = this.FindDocumentInfo(item.SourceCodeMatch.Match.Path, cache);
            if (itemFileName is null || itemDocumentInfo is null) { continue; }
            if (itemFileName.Equals(searchPathFileName)) {
                if (item.SourceCodeMatch.Match.Path.IsContentPathParent(searchPath)) {
                    result.Add(item);
                }
            }
        }
        return result;
    }

    public (FileName fileName, ProjectDocumentInfo? documentInfo) FindDocumentInfo(PathInfo path, DetailContextCache? cache) {
        var resultDetailsRoot = this.SolutionInfo.DetailsRoot.CreateWithRelativePath(path.FilePath);
        var resultDetailsFolder = this.SolutionInfo.DetailsFolder.CreateWithRelativePath(path.FilePath);
        var lstProjectDocumentInfo = this.GetLstProjectDocumentInfo(cache);
        var lstWithRelativePath = new List<FileName>();
        foreach (var projectDocumentInfo in lstProjectDocumentInfo) {
            var rootRelativePath = projectDocumentInfo.DocumentInfo.FileName;
            if (path.FilePath.Equals(rootRelativePath.RelativePath, StringComparison.OrdinalIgnoreCase)) {
                return (projectDocumentInfo.DocumentInfo.FileName, projectDocumentInfo);
            }

            var projectRelativePath = projectDocumentInfo.DocumentInfo.GetFileNameProjectRebased(projectDocumentInfo.ProjectInfo);
            if (path.FilePath.Equals(projectRelativePath.RelativePath, StringComparison.OrdinalIgnoreCase)) {
                return (projectDocumentInfo.DocumentInfo.FileName, projectDocumentInfo);
            }
        }
        return (resultDetailsRoot, null);
    }


}
public class DetailContextCache {
    internal List<ProjectDocumentInfo>? CacheLstProjectDocumentInfo;
}

public readonly record struct ProjectDocumentInfo(ProjectInfo ProjectInfo, IDocumentInfo DocumentInfo);
public readonly record struct ProjectDocumentInfoSourceCodeMatch(ProjectInfo ProjectInfo, IDocumentInfo DocumentInfo, SourceCodeMatch SourceCodeMatch);

