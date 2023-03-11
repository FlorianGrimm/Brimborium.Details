namespace Brimborium.Details.Repository;

public interface IProjectRepositoryFactory {
    ProjectRepository Get(SolutionData solutionData);
}

[Brimborium.Registrator.Singleton]
public class ProjectRepositoryFactory : IProjectRepositoryFactory {
    private ProjectRepository? _ProjectRepository;

    public ProjectRepositoryFactory() {
    }

    public ProjectRepository Get(SolutionData solutionData) {
        return _ProjectRepository ??= new ProjectRepository(solutionData);
    }
}

public class ProjectRepository {
    private readonly Dictionary<string, ProjectData> _ProjectByAbsoluteFilePath;
    private readonly Dictionary<string, ProjectData> _ProjectByRootRelativeFilePath;
    private readonly List<ProjectData> _ListProjects;
    private SolutionData _SolutionData;
    private ProjectData? _DetailsProject;

    public ProjectRepository(SolutionData solutionData) {
        this._ProjectByAbsoluteFilePath = new(StringComparer.InvariantCultureIgnoreCase);
        this._ProjectByRootRelativeFilePath = new(StringComparer.InvariantCultureIgnoreCase);
        this._ListProjects = new List<ProjectData>();
        this._SolutionData = solutionData;
    }

    public void SetSolutionData(SolutionData solutionData) {
        this._SolutionData = solutionData;
    }

    public ProjectData GetOrAdd(ProjectData project) {
        var rootRelativeProjectFileName
            = project.FilePath.Rebase(this._SolutionData.DetailsRoot)
            ?? throw new ArgumentException("empty filepath", nameof(project));
        if (!ReferenceEquals(rootRelativeProjectFileName, project.FilePath)) {
            project = project with { FilePath = rootRelativeProjectFileName };
        }
        var relativePath = rootRelativeProjectFileName.RelativePath
            ?? throw new ArgumentException("FilePath.RelativePath is null", nameof(project));
        lock (this) {
            if (this._ProjectByRootRelativeFilePath.TryGetValue(relativePath, out var result)) {
                return result;
            } else {
                result = project with { };
                this._ProjectByRootRelativeFilePath.Add(relativePath, result);
                this._ProjectByAbsoluteFilePath.Add(
                    rootRelativeProjectFileName.AbsolutePath
                    ?? throw new ArgumentException("FilePath.AbsolutePath is null", nameof(project)),
                    result);
                this._ListProjects.Add(result);
                if (project.FolderPath == this._SolutionData.DetailsFolder) {
                    this._DetailsProject = result;
                }
                return result;
            }
        }
    }

    public ProjectData? GetProjectWithFolderPath(FileName detailsFolder) {
        var result = this._ListProjects.FirstOrDefault(
            project => project.FolderPath == detailsFolder
            );
        return result;
    }

    public ProjectData? GetProjectByFilePath(FileName filePath) {
        var result = this._ListProjects.FirstOrDefault(
            project => project.FilePath == filePath
            );
        return result;
    }

    public ProjectRepositorySnapshot GetSnapshot() {
        lock (this) {
            var result = new ProjectRepositorySnapshot(
                this._SolutionData, 
                new List<ProjectData>(this._ListProjects),
                this._DetailsProject);
            return result;
        }
    }
}
public class ProjectRepositorySnapshot {
    private readonly SolutionData _SolutionData;
    private readonly List<ProjectData> _ProjectDatas;
    private readonly ProjectData? _DetailsProject;
    private readonly Dictionary<string, ProjectData> _ProjectByAbsoluteFilePath;
    private readonly Dictionary<string, ProjectData> _ProjectByRootRelativeFilePath;
    public ProjectRepositorySnapshot(
        SolutionData solutionData,
        List<ProjectData> projectDatas,
        ProjectData? detailsProject) {
        this._SolutionData = solutionData;
        this._ProjectDatas = projectDatas;
        this._DetailsProject = detailsProject;
        this._ProjectByAbsoluteFilePath = new(StringComparer.InvariantCultureIgnoreCase);
        this._ProjectByRootRelativeFilePath = new(StringComparer.InvariantCultureIgnoreCase);
        foreach(var projectData in projectDatas) {
            if (projectData.FilePath.AbsolutePath is not null){
                this._ProjectByAbsoluteFilePath.Add(projectData.FilePath.AbsolutePath, projectData);
            }
            if (projectData.FilePath.RelativePath is not null){
                this._ProjectByRootRelativeFilePath.Add(projectData.FilePath.RelativePath, projectData);
            }
        }
    }

    public bool TryGetByAbsoluteFilePath(
        FileName project,
        [MaybeNullWhen(false)] out ProjectData projectData) 
        => _ProjectByAbsoluteFilePath.TryGetValue(
                project.AbsolutePath
                ?? throw new ArgumentException("FilePath.AbsolutePath is null", nameof(project)),
                out projectData);
}

public class ProjectContext {
    private readonly ProjectRepository _ProjectRepository;
    private readonly ProjectDocumentRepository _ProjectDocumentRepository;
    private readonly DocumentRepository _DocumentRepository;
    private readonly ProjectData _Project;

    public ProjectContext(
        ProjectRepository projectRepository,
        ProjectDocumentRepository projectDocumentRepository,
        DocumentRepository documentRepository,
        ProjectData project
        ) {
        this._ProjectRepository = projectRepository;
        this._ProjectDocumentRepository = projectDocumentRepository;
        this._DocumentRepository = documentRepository;
        this._Project = project;
    }

    public ProjectData Project => this._Project;

    public List<ProjectDocumentData> SetProjectDocuments(List<FileName> listDocumentFileName) {
        var result = new List<ProjectDocumentData>();
        foreach (var documentFileName in listDocumentFileName) {
            var documentData = this._DocumentRepository.GetOrAdd(
                new DocumentData(documentFileName, null));
            var projectDocumentData = this._ProjectDocumentRepository.GetOrAdd(
                new ProjectDocumentData(this._Project.FilePath, documentData.FilePath));
            result.Add(projectDocumentData);
        }
        return result;
    }

    public void SetListProjectDocumentInfo<TDocumentInfo>(List<TDocumentInfo> listDocumentInfo)
        where TDocumentInfo : IDocumentInfo {
        foreach (var documentInfo in listDocumentInfo) {
            var documentData = new DocumentData(documentInfo.FileName, documentInfo);
            this._DocumentRepository.Set(documentData);
            var projectDocumentData = this._ProjectDocumentRepository.GetOrAdd(
                new ProjectDocumentData(this._Project.FilePath, documentData.FilePath)
                );

        }
    }
}
