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

    public ProjectData GetOrAddDetailsProject(ProjectData? project) {
        return this._DetailsRepository.GetOrAddDetailsProject(project);
    }

    public ProjectData GetOrAddProject(ProjectData project) {
        var result = this._DetailsRepository.GetOrAddProject(project);
        return result;
    }

    public void SetProjectDocuments(ProjectData project, List<FileName> listDocument) {
        var projectContext = this._DetailsRepository.GetProjectContext(project);
        projectContext.SetProjectDocuments(listDocument);
    }

    public ProjectData AddCSharpProject(string projectFilePath, string name, ProjectId id, List<FileName> listDocument) {
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

    public void SetListProjectDocumentInfo<TDocumentInfo>(ProjectData project, List<TDocumentInfo> listDocumentInfo)
        where TDocumentInfo : IDocumentInfo {
        var projectContext = this._DetailsRepository.GetProjectContext(project);
        projectContext.SetListProjectDocumentInfo(listDocumentInfo);
    }
    
    public void AddDocumentInfo(
        ProjectData project,
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

    public FileName ConvertToFileName(string absolutePath) {
        if (string.IsNullOrEmpty(absolutePath)) {
            return FileName.Empty;
        } else {
            return this.DetailsRoot.CreateWithAbsolutePath(absolutePath);
        }
    }

}

public readonly record struct ProjectDocumentInfoSourceCodeMatch(
    ProjectData Project, 
    IDocumentInfo Document,
    SourceCodeData SourceCodeMatch);

