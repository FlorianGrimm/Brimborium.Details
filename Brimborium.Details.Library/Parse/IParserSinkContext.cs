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
    private readonly IWatchServiceConfigurator _WatchServiceConfigurator;

    public SolutionData SolutionData => this._SolutionData;
    public ProjectData? DetailsProject { get; set; }
    public FileName DetailsRoot => this._SolutionData.DetailsRoot;
    public FileName SolutionFile => this._SolutionData.SolutionFile;
    public FileName DetailsFolder => this._SolutionData.DetailsFolder;

    public ParserSinkContext(
        IRootRepository detailsRepository,
        SolutionData solutionData,
         IWatchServiceConfigurator watchServiceConfigurator
    ) {
        this._DetailsRepository = detailsRepository;
        this._SolutionData = solutionData;
        this._WatchServiceConfigurator = watchServiceConfigurator;
    }


    public ProjectData GetOrAddDetailsProject(ProjectData? project) {
        var result = this._DetailsRepository.GetOrAddDetailsProject(project);
        if (project is not null){
            this._WatchServiceConfigurator.AddDirectory(project, result.FolderPath);
        }
        return result;
    }

    public ProjectData GetOrAddProject(ProjectData project) {
        var result = this._DetailsRepository.GetOrAddProject(project);
        this._WatchServiceConfigurator.AddDirectory(project, result.FolderPath);
        return result;
    }

    public void SetProjectDocuments(ProjectData project, List<FileName> listDocument) {
        var projectContext = this._DetailsRepository.GetProjectContext(project);
        var result=projectContext.SetProjectDocuments(listDocument);
        foreach (var item in result) {
            this._WatchServiceConfigurator.AddFile(project, item.Document);
        }
    }

    public void SetListProjectDocumentInfo<TDocumentInfo>(ProjectData project, List<TDocumentInfo> listDocumentInfo)
        where TDocumentInfo : IDocumentInfo {
        var projectContext = this._DetailsRepository.GetProjectContext(project);
        var result = projectContext.SetListProjectDocumentInfo(listDocumentInfo);
        foreach (var item in result) {
            this._WatchServiceConfigurator.AddFile(project, item.Document);
        }
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

