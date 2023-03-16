namespace Brimborium.Details.Parse;

public interface IParserSinkContext {
    FileName DetailsFolder { get; }
    ProjectData? DetailsProject { get; set; }
    FileName DetailsRoot { get; }
    SolutionData SolutionData { get; }
    FileName SolutionFile { get; }

    void AddTypescriptProject(ProjectData typescriptProject, List<TypescriptDocumentInfo> lstDocumentInfo);
    FileName ConvertToFileName(string absolutePath);
    ProjectData GetOrAddDetailsProject(ProjectData? project);
    ProjectData GetOrAddProject(ProjectData project);
    void SetListProjectDocumentInfo<TDocumentInfo>(ProjectData project, List<TDocumentInfo> listDocumentInfo) where TDocumentInfo : IDocumentInfo;
    void SetProjectDocuments(ProjectData project, List<FileName> listDocument);
}

// § todo.md § test §
public class ParserSinkContext : IParserSinkContext {
    private readonly IRootRepository _DetailsRepository;
    private readonly SolutionData _SolutionData;
    
    public SolutionData SolutionData => this._SolutionData;

    public ProjectData? DetailsProject { get; set; }
    public FileName DetailsRoot => this._SolutionData.DetailsRoot;
    public FileName SolutionFile => this._SolutionData.SolutionFile;
    public FileName DetailsFolder => this._SolutionData.DetailsFolder;

    public ParserSinkContext(
        IRootRepository detailsRepository,
        SolutionData solutionData
    ) {
        this._DetailsRepository = detailsRepository;
        this._SolutionData = solutionData;
    }


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

    public void SetListProjectDocumentInfo<TDocumentInfo>(ProjectData project, List<TDocumentInfo> listDocumentInfo)
        where TDocumentInfo : IDocumentInfo {
        var projectContext = this._DetailsRepository.GetProjectContext(project);
        projectContext.SetListProjectDocumentInfo(listDocumentInfo);
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

