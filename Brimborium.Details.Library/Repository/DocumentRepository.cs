namespace Brimborium.Details.Repository;

public interface IDocumentRepositoryFactory {
    DocumentRepository Get(SolutionData solutionData);
}

[Brimborium.Registrator.Singleton]
public class DocumentRepositoryFactory : IDocumentRepositoryFactory {
    private DocumentRepository? _DocumentRepository;

    public DocumentRepositoryFactory() {
    }

    public DocumentRepository Get(SolutionData solutionData) {
        return _DocumentRepository ??= new DocumentRepository(solutionData);
    }
}

public class DocumentRepository {
    private SolutionData _SolutionData;

    public DocumentRepository(SolutionData solutionData) {
        this._SolutionData = solutionData;
    }

    public DocumentData GetOrAdd(DocumentData documentData) {
        return documentData;
    }
}

public record DocumentData(
    FileName fileName);