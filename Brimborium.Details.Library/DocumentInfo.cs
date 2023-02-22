namespace Brimborium.Details;

public record DocumentInfo(
    FileName FilePath,
    string Language
) {
    public DocumentInfoPersitence PreSave(FileName detailsRoot) {
        return new DocumentInfoPersitence(
            this.FilePath.Rebase(detailsRoot)?.RelativePath ?? this.FilePath.ToString(),
            this.Language
        );
    }
}

public record DocumentInfoPersitence(
    string FilePath,
    string Language
) {
    public DocumentInfo PostLoad(FileName detailsRoot) {
        return new DocumentInfo(
            detailsRoot.Create(this.FilePath),
            this.Language
        );
    }
}
