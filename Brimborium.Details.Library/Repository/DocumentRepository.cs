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
    private readonly Dictionary<FileName, DocumentData> _DictDocumentData;

    public DocumentRepository(SolutionData solutionData) {
        this._SolutionData = solutionData;
        this._DictDocumentData = new Dictionary<FileName, DocumentData>();
    }

    public DocumentData GetOrAdd(DocumentData documentData) {
        if (this._DictDocumentData.TryGetValue(documentData.FilePath, out var result)) {
            return result;
        } else {
            this._DictDocumentData.Add(documentData.FilePath, documentData);
            return documentData;
        }
    }

    public bool TryGet(FileName filePath, [MaybeNullWhen(false)] DocumentData value)
        => this._DictDocumentData.TryGetValue(filePath, out value);

    public void Set(DocumentData documentData) {
        this._DictDocumentData[documentData.FilePath] = documentData;
    }

    public DocumentRepositorySnapshot GetSnapshot() {
        lock (this) {
            var result = new DocumentRepositorySnapshot(
                this._SolutionData,
                new Dictionary<FileName, DocumentData>(_DictDocumentData));
            return result;
        }
    }
}


public class DocumentRepositorySnapshot {
    private SolutionData _SolutionData;
    private readonly Dictionary<FileName, DocumentData> _DictDocumentData;

    public DocumentRepositorySnapshot(
        SolutionData solutionData, 
        Dictionary<FileName, DocumentData> dictDocumentData) {
        this._SolutionData = solutionData;
        this._DictDocumentData = dictDocumentData;
    }

    private List<IDocumentInfo>? _GetAllDocumentInfo;
    public List<IDocumentInfo> GetAllDocumentInfo() {
        if (this._GetAllDocumentInfo is not null) {
            return this._GetAllDocumentInfo;
        }
        var result = new List<IDocumentInfo>();
        foreach (var item in this._DictDocumentData) {
            if (item.Value.DocumentInfo is not null) {
                result.Add(item.Value.DocumentInfo);
            }
        }
        return this._GetAllDocumentInfo = result;
    }

    private List<MarkdownDocumentInfo>? _GetAllMarkdownDocumentInfo;
    public List<MarkdownDocumentInfo> GetAllMarkdownDocumentInfo() {
        if (this._GetAllMarkdownDocumentInfo is not null) {
            return this._GetAllMarkdownDocumentInfo;
        }
        var result = new List<MarkdownDocumentInfo>();
        foreach (var item in this._DictDocumentData) {
            if (item.Value.DocumentInfo is MarkdownDocumentInfo mdi) {
                result.Add(mdi);
            }
        }
        return this._GetAllMarkdownDocumentInfo = result;
    }

    private List<DocumentInfoSourceCodeMatch>? _GetAllConsumes;
    public List<DocumentInfoSourceCodeMatch> GetAllConsumes() {
        if (this._GetAllConsumes is not null) {
            return this._GetAllConsumes;
        }
        var result = new List<DocumentInfoSourceCodeMatch>();
        foreach (var item in this._DictDocumentData) {
            if (item.Value.DocumentInfo?.ListConsumes is List<SourceCodeData> list) {
                foreach (var scd in list) {
                    result.Add(new DocumentInfoSourceCodeMatch(
                        item.Value.DocumentInfo, 
                        scd
                        ));
                }
            }
        }
        return this._GetAllConsumes = result;
    }

    private List<DocumentInfoSourceCodeMatch>? _GetAllProvides;
    public List<DocumentInfoSourceCodeMatch> GetAllProvides() {
        if (this._GetAllProvides is not null) {
            return this._GetAllProvides;
        }
        var result = new List<DocumentInfoSourceCodeMatch>();
        foreach (var item in this._DictDocumentData) {
            if (item.Value.DocumentInfo?.ListProvides is List<SourceCodeData> list) {
                foreach (var scd in list) {
                    result.Add(new DocumentInfoSourceCodeMatch(
                        item.Value.DocumentInfo, 
                        scd
                        ));
                }
            }
        }
        return this._GetAllProvides = result;
    }

    public bool TryGetByAbsoluteFilePath(
        FileName filePath,
        [MaybeNullWhen(false)] out DocumentData documentData) 
        => this._DictDocumentData.TryGetValue(filePath, out documentData);
}

public record DocumentData(
    FileName FilePath,
    IDocumentInfo? DocumentInfo) ;
