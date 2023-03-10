namespace Brimborium.Details.Enhancement;

public interface IMatchCommand {
    bool IsMatching(DetailData matchInfo);
    Task ExecuteAsync(
        SourceCodeData sourceCodeMatch,
        MarkdownDocumentWriter markdownDocumentWriter,
        IReplacementFinder? replacementFinder,
        DetailContextCache? cache,
        CancellationToken cancellationToken);

    IReplacementFinder? GetReplacementFinder(
        MarkdownDocumentInfo markdownDocumentInfo,
        SourceCodeData sourceCodeMatch);
}
