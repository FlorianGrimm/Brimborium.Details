namespace Brimborium.Details.Parse;

// § todo.md § test §
public class ParserSinkContext {
    private readonly IRootRepository _DetailsRepository;
    private readonly SolutionData _SolutionData;

    private readonly Dictionary<string, ProjectData> _ProjectInfoByFilePath;
    private readonly Dictionary<string, ProjectId> _ProjectIdByFilePath;
    private readonly Dictionary<ProjectId, ProjectData> _ProjectInfoByProjectId;

    private ProjectData? _DetailsProjectData;


    public SolutionData SolutionData => this._SolutionData;

    public Dictionary<string, ProjectData> ProjectInfoByFilePath => this._ProjectInfoByFilePath;

    [JsonIgnore]
    public Dictionary<string, ProjectId> ProjectIdByFilePath => this._ProjectIdByFilePath;

    [JsonIgnore]
    public Dictionary<ProjectId, ProjectData> ProjectInfoByProjectId => this._ProjectInfoByProjectId;

    public ProjectData? DetailsProject { get; set; }
    public FileName DetailsRoot => this._SolutionData.DetailsRoot;
    public FileName SolutionFile => this._SolutionData.SolutionFile;
    public FileName DetailsFolder => this._SolutionData.DetailsFolder;

    public ParserSinkContext(
        IRootRepository detailsRepository,
        SolutionData solutionInfo
    ) {
        this._DetailsRepository = detailsRepository;
        this._SolutionData = solutionInfo;
        this._ProjectInfoByFilePath = new(StringComparer.InvariantCultureIgnoreCase);
        this._ProjectIdByFilePath = new(StringComparer.InvariantCultureIgnoreCase);
        this._ProjectInfoByProjectId = new();
    }

    public List<ProjectData> GetLstProjectInfo() => new List<ProjectData>(this._ProjectInfoByFilePath.Values);

    public ProjectData EnsureDetailsProjectInfo() {
        //if (this._DetailsProjectData != null) { return this._DetailsProjectData; }
        //var detailsFolder = this.DetailsFolder;
        //var projectContext= this._DetailsRepository.GetProjectWithFolderPath(this.DetailsFolder);

        //if (projectContext is null) {
        //    var projectData = new ProjectData(
        //        "Details",
        //        detailsFolder.CreateWithRelativePath("details.csproj"),
        //        "Markdown",
        //        detailsFolder);
        //    projectContext = this._DetailsRepository.GetOrAddProject(projectData);
        //}
        //return this._DetailsProjectData = new ProjectData(
        //    projectData.Name,
        //    projectData.FilePath,
        //    projectData.Language,
        //    projectData.FolderPath);
        throw new NotImplementedException();
    }

    public ProjectData EnsureProjectData(ProjectData project) {
        //var projectData = this._DetailsRepository.GetProjectByFilePath(project.FilePath);

        //if (projectData is null) {
        //    projectData = new ProjectData(
        //        project.Name,
        //        project.FilePath,
        //        project.Language,
        //        project.FolderPath);
        //    projectData = this._DetailsRepository.AddProject(projectData);
        //}

        //return projectData;
        throw new NotImplementedException();
    }

    public ProjectData EnsureProjectInfo(ProjectData project) {
        //var projectData = this.EnsureProjectData(project);

        //return new ProjectData(
        //    projectData.Name,
        //    projectData.FilePath,
        //    projectData.Language,
        //    projectData.FolderPath
        //    );
        throw new NotImplementedException();
    }
    public ProjectData EnsureProjectDocumentFileNames(ProjectData project, List<FileName> lstDocument) {
        //var projectData = this.EnsureProjectData(project);
        //List<FileName> lstDocumentFileName = new();
        //var projectFolderFileName = projectData.FolderPath;
        //foreach (var document in lstDocument.OrderBy(document => document.AbsolutePath, StringComparer.OrdinalIgnoreCase)) {
        //    var documentFileName = document.Rebase(projectFolderFileName);
        //    if (documentFileName is null) { continue; }
        //    lstDocumentFileName.Add(documentFileName);
        //}

        //return this._DetailsRepository.SetProjectData(project, lstDocumentFileName);
        throw new NotImplementedException();
    }

    public ProjectData AddCSharpProject(string projectFilePath, string name, ProjectId id, List<FileName> lstDocument) {
        //var projectFileName = this._SolutionData.DetailsRoot.CreateWithAbsolutePath(projectFilePath);
        //var projectFolderFileName = projectFileName.GetParentDirectory() ?? FileName.Empty;

        //var projectInfo = new ProjectData(
        //    Name: name,
        //    FilePath: projectFileName,
        //    Language: "CSharp",
        //    FolderPath: projectFolderFileName
        //);

        //foreach (var document in lstDocument.OrderBy(document => document.AbsolutePath, StringComparer.OrdinalIgnoreCase)) {
        //    var documentFileName = document.Rebase(projectFolderFileName);
        //    if (documentFileName is null) { continue; }
        //    projectInfo.LstDocumentFileName.Add(documentFileName);
        //}

        //lock (this) {
        //    this._ProjectInfoByFilePath[projectInfo.FilePath.AbsolutePath!] = projectInfo;
        //    this._ProjectIdByFilePath[projectInfo.FilePath.AbsolutePath!] = id;
        //    this._ProjectInfoByProjectId[id] = projectInfo;
        //}

        //return projectInfo;
        throw new NotImplementedException();
    }

    public void AddCSharpDocumentInfo(ProjectId projectId, List<CSharpDocumentInfo> lstCSharpDocumentInfo) {
        //var lstSortedCSharpDocumentInfo = lstCSharpDocumentInfo.OrderBy(di => di.FileName.AbsolutePath, StringComparer.OrdinalIgnoreCase);
        //lock (this) {
        //    if (this._ProjectInfoByProjectId.TryGetValue(projectId, out var projectInfo)) {
        //        projectInfo.LstCSharpDocumentInfo.Clear();
        //        projectInfo.LstCSharpDocumentInfo.AddRange(lstSortedCSharpDocumentInfo);
        //    }
        //}
        throw new NotImplementedException();
    }

    public void AddMarkdownDocumentInfo(
        ProjectData projectInfo,
        MarkdownDocumentInfo documentInfo) {
        //lock (this) {
        //    projectInfo.LstMarkdownDocumentInfo.Add(documentInfo);
        //}
        throw new NotImplementedException();
    }

    public void AddTypescriptProject(ProjectData typescriptProject, List<TypescriptDocumentInfo> lstDocumentInfo) {
        //lock (this) {
        //    typescriptProject.LstTypescriptDocumentInfo = lstDocumentInfo;
        //    this._ProjectInfoByFilePath[typescriptProject.FilePath.AbsolutePath!] = typescriptProject;
        //}
        throw new NotImplementedException();
    }

    public List<ProjectDocumentInfo> GetLstProjectDocumentInfo(DetailContextCache? cache) {
        //if (cache?.CacheLstProjectDocumentInfo is List<ProjectDocumentInfo> resultCached) { return resultCached; }

        //var result = new List<ProjectDocumentInfo>();
        //foreach (var projectInfo in this._ProjectInfoByFilePath.Values) {
        //    foreach (var documentInfo in projectInfo.LstMarkdownDocumentInfo) {
        //        result.Add(new ProjectDocumentInfo(projectInfo, documentInfo));
        //    }
        //    foreach (var documentInfo in projectInfo.LstCSharpDocumentInfo) {
        //        result.Add(new ProjectDocumentInfo(projectInfo, documentInfo));
        //    }

        //    foreach (var documentInfo in projectInfo.LstTypescriptDocumentInfo) {
        //        result.Add(new ProjectDocumentInfo(projectInfo, documentInfo));
        //    }
        //}
        //if (cache is not null) {
        //    cache.CacheLstProjectDocumentInfo = result;
        //}
        //return result;
        throw new NotImplementedException();
    }

    public List<ProjectDocumentInfo> GetLstMarkdownDocumentInfo() {
        //var result = new List<ProjectDocumentInfo>();
        //foreach (var projectInfo in this._ProjectInfoByFilePath.Values) {
        //    foreach (var documentInfo in projectInfo.LstMarkdownDocumentInfo) {
        //        result.Add(new ProjectDocumentInfo(projectInfo, documentInfo));
        //    }
        //}
        //return result;
        throw new NotImplementedException();
    }

    public IEnumerable<ProjectDocumentInfoSourceCodeMatch> GetLstConsumes(DetailContextCache? cache) {
        //var result = new List<ProjectDocumentInfoSourceCodeMatch>();
        //foreach (var projectDocumentInfo in this.GetLstProjectDocumentInfo(cache)) {
        //    if (projectDocumentInfo.DocumentInfo.LstConsumes is null) { continue; }
        //    foreach (var sourceCodeMatch in projectDocumentInfo.DocumentInfo.LstConsumes) {
        //        result.Add(new ProjectDocumentInfoSourceCodeMatch(
        //            projectDocumentInfo.ProjectInfo,
        //            projectDocumentInfo.DocumentInfo,
        //            sourceCodeMatch));
        //    }
        //}
        //return result;
        throw new NotImplementedException();
    }

    public IEnumerable<ProjectDocumentInfoSourceCodeMatch> GetLstProvides(DetailContextCache? cache) {
        //var result = new List<ProjectDocumentInfoSourceCodeMatch>();
        //foreach (var projectDocumentInfo in this.GetLstProjectDocumentInfo(cache)) {
        //    if (projectDocumentInfo.DocumentInfo.LstProvides is null) { continue; }
        //    foreach (var sourceCodeMatch in projectDocumentInfo.DocumentInfo.LstProvides) {
        //        result.Add(new ProjectDocumentInfoSourceCodeMatch(
        //            projectDocumentInfo.ProjectInfo,
        //            projectDocumentInfo.DocumentInfo,
        //            sourceCodeMatch));
        //    }
        //}
        //return result;
        throw new NotImplementedException();

    }

    public List<ProjectDocumentInfoSourceCodeMatch> QueryPath(
        PathData searchPath,
        DetailContextCache? cache) {
        //var result = new List<ProjectDocumentInfoSourceCodeMatch>();
        //var (searchPathFileName, searchPathDocumentInfo) = this.FindDocumentInfo(searchPath, cache);
        //if (searchPathDocumentInfo is null) { return result; }
        //foreach (var item in this.GetLstProvides(cache)) {
        //    var (itemFileName, itemDocumentInfo) = this.FindDocumentInfo(item.SourceCodeMatch.DetailData.Path, cache);
        //    if (itemFileName is null || itemDocumentInfo is null) { continue; }
        //    if (itemFileName.Equals(searchPathFileName)) {
        //        if (searchPath.IsContentPathEqual(item.SourceCodeMatch.DetailData.Path)) {
        //            result.Add(item);
        //        }
        //    }
        //}
        //return result;
        throw new NotImplementedException();
    }


    public List<ProjectDocumentInfoSourceCodeMatch> QueryPathChildren(
        PathData searchPath,
        DetailContextCache? cache) {
        //var result = new List<ProjectDocumentInfoSourceCodeMatch>();
        //var (searchPathFileName, searchPathDocumentInfo) = this.FindDocumentInfo(searchPath, cache);
        //if (searchPathDocumentInfo is null) { return result; }
        //foreach (var item in this.GetLstProvides(cache)) {
        //    var (itemFileName, itemDocumentInfo) = this.FindDocumentInfo(item.SourceCodeMatch.DetailData.Path, cache);
        //    if (itemFileName is null || itemDocumentInfo is null) { continue; }
        //    if (itemFileName.Equals(searchPathFileName)) {
        //        if (item.SourceCodeMatch.DetailData.Path.IsContentPathParent(searchPath)) {
        //            result.Add(item);
        //        }
        //    }
        //}
        //return result;
        throw new NotImplementedException();
    }

    public (FileName fileName, ProjectDocumentInfo? documentInfo) FindDocumentInfo(PathData path, DetailContextCache? cache) {
        //var resultDetailsRoot = this.SolutionData.DetailsRoot.CreateWithRelativePath(path.FilePath);
        //var resultDetailsFolder = this.SolutionData.DetailsFolder.CreateWithRelativePath(path.FilePath);
        //var lstProjectDocumentInfo = this.GetLstProjectDocumentInfo(cache);
        //var lstWithRelativePath = new List<FileName>();
        //foreach (var projectDocumentInfo in lstProjectDocumentInfo) {
        //    var rootRelativePath = projectDocumentInfo.DocumentInfo.FileName;
        //    if (path.FilePath.Equals(rootRelativePath.RelativePath, StringComparison.OrdinalIgnoreCase)) {
        //        return (projectDocumentInfo.DocumentInfo.FileName, projectDocumentInfo);
        //    }

        //    var projectRelativePath = projectDocumentInfo.DocumentInfo.GetFileNameProjectRebased(projectDocumentInfo.ProjectInfo);
        //    if (path.FilePath.Equals(projectRelativePath.RelativePath, StringComparison.OrdinalIgnoreCase)) {
        //        return (projectDocumentInfo.DocumentInfo.FileName, projectDocumentInfo);
        //    }
        //}
        //return (resultDetailsRoot, null);
        throw new NotImplementedException();
    }

    public FileName ConvertToFileName(string absolutePath) {
        if (string.IsNullOrEmpty(absolutePath)) {
            return FileName.Empty;
        } else {
            return this.DetailsRoot.CreateWithAbsolutePath(absolutePath);
        }
    }

}
public class DetailContextCache {
    internal List<ProjectDocumentInfo>? CacheLstProjectDocumentInfo;
}

public readonly record struct ProjectDocumentInfo(ProjectData ProjectInfo, IDocumentInfo DocumentInfo);
public readonly record struct ProjectDocumentInfoSourceCodeMatch(ProjectData ProjectInfo, IDocumentInfo DocumentInfo, SourceCodeData SourceCodeMatch);

