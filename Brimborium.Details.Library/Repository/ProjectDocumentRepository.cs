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
    private SolutionData _SolutionData;

    public ProjectDocumentRepository(SolutionData solutionData) {
        this._SolutionData = solutionData;
    }

    public ProjectDocumentData GetOrAdd(ProjectDocumentData projectDocumentData) {
        return projectDocumentData;
    }
}

public record ProjectDocumentData(
    ProjectData Project,
    DocumentData Document
    );