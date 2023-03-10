namespace Brimborium.Details.Repository;

public interface IRootRepositoryFactory {
    RootRepository Get(SolutionData solutionData);
}

[Brimborium.Registrator.Singleton]
public class RootRepositoryFactory : IRootRepositoryFactory {
    private readonly IProjectRepositoryFactory _ProjectRepositoryFactory;
    private readonly IProjectDocumentRepositoryFactory _ProjectDocumentRepositoryFactory;
    private readonly IDocumentRepositoryFactory _DocumentRepositoryFactory;
    private RootRepository? _RootRepository;

    public RootRepositoryFactory(
        IProjectRepositoryFactory projectRepositoryFactory,
        IProjectDocumentRepositoryFactory projectDocumentRepositoryFactory,
        IDocumentRepositoryFactory documentRepositoryFactory
        ) {
        this._ProjectRepositoryFactory = projectRepositoryFactory;
        this._ProjectDocumentRepositoryFactory = projectDocumentRepositoryFactory;
        this._DocumentRepositoryFactory = documentRepositoryFactory;
    }

    public RootRepository Get(SolutionData solutionData) {
        return _RootRepository ??= new RootRepository(
            solutionData,
            this._ProjectRepositoryFactory.Get(solutionData),
            this._ProjectDocumentRepositoryFactory.Get(solutionData),
            this._DocumentRepositoryFactory.Get(solutionData)
            );
    }
}

public interface IRootRepository {
    SolutionData GetSolutionData();
    ParserSinkContext GetParserSinkContext();
    ProjectContext GetProjectContext(ProjectData projectData);
    ProjectData? GetProjectWithFolderPath(FileName detailsFolder);
    ProjectData? GetProjectByFilePath(FileName project);
    ProjectData GetOrAddProject(ProjectData project);
    List<ProjectDocumentData> SetProjectDocuments(
        ProjectData project,
        List<FileName> lstDocumentFileName);
}

public class RootRepository : IRootRepository {
    private readonly SolutionData _SolutionData;
    private readonly ProjectRepository _ProjectRepository;
    private readonly ProjectDocumentRepository _ProjectDocumentRepository;
    private readonly DocumentRepository _DocumentRepository;

    public RootRepository(
        SolutionData solutionData,
        ProjectRepository projectRepository,
        ProjectDocumentRepository projectDocumentRepository,
        DocumentRepository documentRepository
        ) {
        this._SolutionData = solutionData;
        this._ProjectRepository = projectRepository;
        this._ProjectDocumentRepository = projectDocumentRepository;
        this._DocumentRepository = documentRepository;
    }

    public SolutionData GetSolutionData() => this._SolutionData;

    public ParserSinkContext GetParserSinkContext() {
        return new ParserSinkContext(this, this._SolutionData);
    }

    public ProjectContext GetProjectContext(ProjectData projectData) {
        return new ProjectContext(this._ProjectRepository, this._ProjectDocumentRepository, this._DocumentRepository, projectData);
    }

    //public ProjectRepository GetProjectRepository() {
    //    var repository = this._ProjectRepository;
    //    if (repository is null) {
    //        var solutionData = this.GetSolutionData() ?? throw new InvalidOperationException("solutionData is null");
    //        this._ProjectRepository = repository = this._ProjectRepositoryFactory.Get(solutionData);
    //    }
    //    return repository;
    //}

    //public ProjectDocumentRepository GetProjectDocumentRepository() {
    //    var repository = this._ProjectDocumentRepository;
    //    if (repository is null) {
    //        var solutionData = this.GetSolutionData() ?? throw new InvalidOperationException("solutionData is null");
    //        this._ProjectDocumentRepository = repository = this._ProjectDocumentRepositoryFactory.Get(solutionData);
    //    }
    //    return repository;
    //}

    //public DocumentRepository GetDocumentRepository() {
    //    var repository = this._DocumentRepository;
    //    if (repository is null) {
    //        var solutionData = this.GetSolutionData() ?? throw new InvalidOperationException("solutionData is null");
    //        this._DocumentRepository = repository = this._DocumentRepositoryFactory.Get(solutionData);
    //    }
    //    return repository;
    //}

    //this.GetProjectDocumentRepository();
    //this.GetDocumentRepository();

    public ProjectData? GetProjectWithFolderPath(FileName detailsFolder) {
        return this._ProjectRepository.GetProjectWithFolderPath(detailsFolder);
    }

    public ProjectData? GetProjectByFilePath(FileName filePath) {
        return this._ProjectRepository.GetProjectByFilePath(filePath);
    }
    public ProjectData GetOrAddProject(ProjectData project) {
        return this._ProjectRepository.GetOrAdd(project);
    }

    public List<ProjectDocumentData> SetProjectDocuments(
        ProjectData project,
        List<FileName> listDocumentFileName) {
        project = this._ProjectRepository.GetOrAdd(project);
        var result = new List<ProjectDocumentData>();
        foreach (var documentFileName in listDocumentFileName) {
            var documentData = this._DocumentRepository.GetOrAdd(
                new DocumentData(documentFileName)
                );
            var projectDocumentData = this._ProjectDocumentRepository.GetOrAdd(
                new ProjectDocumentData(project, documentData)
                );
            result.Add(projectDocumentData);
        }
        return result;
        //return projectContext.SetProjectDocuments(lstDocumentFileName);
    }
}
