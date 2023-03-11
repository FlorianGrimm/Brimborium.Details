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
            return result;
        }
    }
}

public class ProjectDocumentRepositorySnapshot {
    private readonly SolutionData _SolutionData;
    private readonly Dictionary<ProjectDocumentID, ProjectDocumentData> _Dictionary;
    private readonly ProjectRepositorySnapshot _ProjectRepository;
    private readonly DocumentRepositorySnapshot _DocumentRepository;

    public ProjectDocumentRepositorySnapshot(
        SolutionData solutionData,
        Dictionary<ProjectDocumentID,
        ProjectDocumentData> dictionary,
        ProjectRepositorySnapshot projectRepository,
        DocumentRepositorySnapshot documentRepository) {
        this._SolutionData = solutionData;
        this._Dictionary = dictionary;
        this._ProjectRepository = projectRepository;
        this._DocumentRepository = documentRepository;
    }

    private List<ProjectDocumentInfo>? _GetAllProjectDocumentInfo;
    public List<ProjectDocumentInfo> GetAllProjectDocumentInfo() {
        if (this._GetAllProjectDocumentInfo is not null) {
            return this._GetAllProjectDocumentInfo;
        }
        var result = new List<ProjectDocumentInfo>();
        foreach (var item in this._Dictionary.Values) {
            if (this._ProjectRepository.TryGetByAbsoluteFilePath(item.Project, out var projectData)
                && this._DocumentRepository.TryGetByAbsoluteFilePath(item.Document, out var documentData)) {
                if (documentData.DocumentInfo is not null) {
                    result.Add(new ProjectDocumentInfo(projectData, documentData.DocumentInfo));
                }
            }
        }
        return this._GetAllProjectDocumentInfo = result;
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