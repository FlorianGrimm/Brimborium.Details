namespace Brimborium.Details.Repository;
public class RootRepositorySnapshot {
    private SolutionData _SolutionData;
    private ProjectRepositorySnapshot _ProjectRepository;
    private ProjectDocumentRepositorySnapshot _ProjectDocumentRepository;
    private DocumentRepositorySnapshot _DocumentRepository;

    public RootRepositorySnapshot(
        SolutionData solutionData,
        ProjectRepositorySnapshot projectRepository,
        ProjectDocumentRepositorySnapshot projectDocumentRepository,
        DocumentRepositorySnapshot documentRepository) {
        this._SolutionData = solutionData;
        this._ProjectRepository = projectRepository;
        this._ProjectDocumentRepository = projectDocumentRepository;
        this._DocumentRepository = documentRepository;
    }

    internal void Initialize() {
        this._ProjectRepository.Initialize();
        this._ProjectDocumentRepository.Initialize();
        this._DocumentRepository.Initialize();
    }

    public ProjectRepositorySnapshot ProjectRepository => this._ProjectRepository;
    public ProjectDocumentRepositorySnapshot ProjectDocumentRepository  => this._ProjectDocumentRepository; 
    public DocumentRepositorySnapshot DocumentRepository => this._DocumentRepository;
}
