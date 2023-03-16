using System.Text.Json.Serialization;

namespace Brimborium.Details.Repository;

public interface IProjectDocumentRepositoryFactory {
    ProjectDocumentRepository Get(SolutionData solutionData);
}

[Brimborium.Registrator.Singleton]
public class ProjectDocumentRepositoryFactory : IProjectDocumentRepositoryFactory {
    private ProjectDocumentRepository? _ProjectDocumentRepository;

    public ProjectDocumentRepositoryFactory() {
    }

    public ProjectDocumentRepository Get(SolutionData solutionData) {
        return _ProjectDocumentRepository ??= new ProjectDocumentRepository(solutionData);
    }
}

public class ProjectDocumentRepository {
    private readonly SolutionData _SolutionData;
    private readonly Dictionary<ProjectDocumentID, ProjectDocumentData> _DictProjectDocumentData;

    public ProjectDocumentRepository(SolutionData solutionData) {
        this._SolutionData = solutionData;
        this._DictProjectDocumentData = new Dictionary<ProjectDocumentID, ProjectDocumentData>();
    }

    public ProjectDocumentData GetOrAdd(ProjectDocumentData projectDocumentData) {
        lock (this) {
            if (this._DictProjectDocumentData.TryGetValue(projectDocumentData.GetProjectDocumentID(), out var result)) {
                return result;
            } else {
                this._DictProjectDocumentData.Add(projectDocumentData.GetProjectDocumentID(), projectDocumentData);
                return projectDocumentData;
            }
        }
    }

    public ProjectDocumentRepositorySnapshot GetSnapshot(
        ProjectRepositorySnapshot projectRepository,
        DocumentRepositorySnapshot documentRepository) {
        lock (this) {
            var result = new ProjectDocumentRepositorySnapshot(
                this._SolutionData,
                new Dictionary<ProjectDocumentID, ProjectDocumentData>(this._DictProjectDocumentData),
                projectRepository,
                documentRepository
                );
            result.Initialize();
            return result;
        }
    }
}

public record ProjectDocumentID(
    FileName Project,
    FileName Document
);
public record ProjectDocumentData(
    FileName Project,
    FileName Document
    ) {
    public ProjectDocumentID GetProjectDocumentID() => new ProjectDocumentID(this.Project, this.Document);
}

public class ProjectDocumentRepositorySnapshot {
    private readonly SolutionData _SolutionData;
    private readonly Dictionary<ProjectDocumentID, ProjectDocumentData> _DictionaryProjectDocumentData;
    private readonly ProjectRepositorySnapshot _ProjectRepository;
    private readonly DocumentRepositorySnapshot _DocumentRepository;

    private readonly List<ProjectDocumentInfo> _ListProjectDocumentInfoAbsolute;
    private readonly List<ProjectDocumentInfo> _ListProjectDocumentInfoProjectRootRelative;
    private readonly List<ProjectDocumentInfo> _ListProjectDocumentInfoProjectProjectRelative;
    private readonly List<ProjectDocumentInfo> _ListProjectDocumentInfoRootRelative;
    private readonly List<ProjectDocumentInfo> _ListProjectDocumentInfoProjectRelative;

    public ProjectDocumentRepositorySnapshot(
        SolutionData solutionData,
        Dictionary<ProjectDocumentID, ProjectDocumentData> dictionaryProjectDocumentData,
        ProjectRepositorySnapshot projectRepository,
        DocumentRepositorySnapshot documentRepository) {
        this._SolutionData = solutionData;
        this._DictionaryProjectDocumentData = dictionaryProjectDocumentData;
        this._ProjectRepository = projectRepository;
        this._DocumentRepository = documentRepository;

        var listProjectDocumentInfo = new List<ProjectDocumentInfo>();
        foreach (var projectDocumentData in dictionaryProjectDocumentData.Values) {
            if (this._ProjectRepository.TryGetByAbsoluteFilePath(projectDocumentData.Project, out var projectData)
                && this._DocumentRepository.TryGetByAbsoluteFilePath(projectDocumentData.Document, out var documentData)) {
                if (documentData.DocumentInfo is not null) {
                    FileName documentFilePathRootRelative = documentData.DocumentInfo.FileName.Rebase(
                            this._SolutionData.DetailsRoot)
                        ?? throw new InvalidOperationException("FilePathRootRelative is null.");
                    FileName documentFilePathProjectRelative = documentData.DocumentInfo.FileName.Rebase(
                            projectData.FolderPath)
                        ?? throw new InvalidOperationException("FilePathProjectRelative is null.");
                    var projectDocumentInfo = new ProjectDocumentInfo(
                        projectData.FilePath,
                        documentFilePathRootRelative,
                        documentFilePathProjectRelative,
                        projectData,
                        documentData.DocumentInfo);
                    listProjectDocumentInfo.Add(projectDocumentInfo);
                }
            }
        }

        this._ListProjectDocumentInfoAbsolute = new List<ProjectDocumentInfo>(
            listProjectDocumentInfo
                .OrderBy(item => item.DocumentFilePathRootRelative.AbsolutePath));

        this._ListProjectDocumentInfoProjectRootRelative = new List<ProjectDocumentInfo>(
            listProjectDocumentInfo
                .OrderBy(item => item.ProjectFilePathRootRelative.AbsolutePath)
                .ThenBy(item => item.DocumentFilePathRootRelative.RelativePath));

        this._ListProjectDocumentInfoProjectProjectRelative = new List<ProjectDocumentInfo>(
            listProjectDocumentInfo
                .OrderBy(item => item.ProjectFilePathRootRelative.AbsolutePath)
                .ThenBy(item => item.DocumentFilePathProjectRelative.RelativePath));

        this._ListProjectDocumentInfoRootRelative = new List<ProjectDocumentInfo>(
            listProjectDocumentInfo
                .OrderBy(item => item.ProjectFilePathRootRelative.RelativePath));

        this._ListProjectDocumentInfoProjectRelative = new List<ProjectDocumentInfo>(
            listProjectDocumentInfo
                .OrderBy(item => item.DocumentFilePathProjectRelative.RelativePath));
    }

    internal void Initialize() {
    }

    public List<ProjectDocumentInfo> GetAllProjectDocumentInfoAbsolute()
        => this._ListProjectDocumentInfoAbsolute;

    public List<ProjectDocumentInfo> GetAllProjectDocumentInfoProjectRootRelative()
        => this._ListProjectDocumentInfoProjectRootRelative;

    public List<ProjectDocumentInfo> GetAllProjectDocumentInfoProjectProjectRelative()
        => this._ListProjectDocumentInfoProjectProjectRelative;

    public List<ProjectDocumentInfo> GetAllProjectDocumentInfoRootRelative()
        => this._ListProjectDocumentInfoRootRelative;

    public List<ProjectDocumentInfo> GetAllProjectDocumentInfoProjectRelative()
        => this._ListProjectDocumentInfoProjectRelative;
}

public readonly record struct ProjectDocumentInfo(
    FileName ProjectFilePathRootRelative,
    FileName DocumentFilePathRootRelative,
    FileName DocumentFilePathProjectRelative,
    [property: JsonIgnore]
    ProjectData Project,
    [property: JsonIgnore]
    IDocumentInfo Document) {
    public string ProjectName => this.Project.Name;
}
