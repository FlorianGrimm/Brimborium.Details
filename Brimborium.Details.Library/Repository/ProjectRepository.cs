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
                new DocumentData(documentFileName)
                );
            var projectDocumentData = this._ProjectDocumentRepository.GetOrAdd(
                new ProjectDocumentData(this._Project, documentData)
                );
            result.Add(projectDocumentData);
        }
        return result;
    }
}
