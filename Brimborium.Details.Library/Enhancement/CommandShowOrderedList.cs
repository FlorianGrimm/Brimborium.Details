namespace Brimborium.Details.Enhancement;

//
[Transient]
public class CommandShowOrderedList : IMatchCommand {
    private readonly ILogger<CommandShowOrderedList> _Logger;

    public CommandShowOrderedList(
        ILogger<CommandShowOrderedList> logger
        ) {
        this._Logger = logger;
    }

    public bool IsMatching(DetailData matchInfo)
        => matchInfo.Command == "Show-Ordered-List";

    public Task ExecuteAsync(
        SourceCodeData sourceCodeMatch,
        MarkdownDocumentWriter markdownDocumentWriter,
        IReplacementFinder? replacementFinder,
        CancellationToken cancellationToken) {
        var detailContext = markdownDocumentWriter.WriterContext;
        var matchInfo = sourceCodeMatch.DetailData;

        var path = matchInfo.Path;
        if (path is null || path.IsEmpty()) {
            path = matchInfo.MatchPath.WithLine(0);
        }

        this._Logger.LogDebug("Show-List: {0}", path);

        var lstMatch = detailContext.QueryPath(path);

        //ListBlock? target = default;
        Range? targetRange = default;
        if (replacementFinder is null) { return Task.CompletedTask; }
        if (replacementFinder is not ListBlockReplacementFinder listBlockReplacementFinder) { return Task.CompletedTask; }

        if (listBlockReplacementFinder.Range is not null) {
            targetRange = listBlockReplacementFinder.Range;
            // TODO
        }

        if (targetRange is null) {
            var mdBlock = markdownDocumentWriter.GetBlockByLine(sourceCodeMatch.DetailData.Line);
            if (mdBlock is null) {
                return Task.CompletedTask;
            }
            targetRange = markdownDocumentWriter.GetRangeAfter(mdBlock);
        }

        if (targetRange is null) {
            return Task.CompletedTask;
        }
        var replacementContent = markdownDocumentWriter.CreatePart(targetRange.Value);
        if (replacementContent is null) {
            return Task.CompletedTask;
        }
        var oldContent = replacementContent.AsSubString();

        var sb = replacementContent.GetReplacementBuilder();
        if (lstMatch.Count == 0) {
            sb.Append("- No Matches").AppendLine();
        } else {
            var lstMatchOrdered =
                OrderedItem.CreateList(lstMatch,
                    (match) => OrderedItem.ExtractOrderFromComment(match.SourceCodeMatch.DetailData.Comment));

            foreach (var (order, match) in lstMatchOrdered) {
                sb.Append(order.ToString());
                sb.Append(". ");
                if (match.SourceCodeMatch.FilePath.RootFolder == markdownDocumentWriter.WriterContext.DetailsFolder) {
                    sb.Append("details://").Append(match.SourceCodeMatch.FilePath.RelativePath);
                    if (match.SourceCodeMatch.DetailData.Line > 0) {
                        sb.Append("#").Append(match.SourceCodeMatch.DetailData.Line);
                    }
                    if (!string.IsNullOrEmpty(match.SourceCodeMatch.DetailData.Comment)) {
                        sb.Append(" § ").Append(match.SourceCodeMatch.DetailData.Comment);
                    }
                } else {
                    sb.Append("detailscode://").Append(match.SourceCodeMatch.FilePath.RelativePath);
                    if (match.SourceCodeMatch.DetailData.Line > 0) {
                        sb.Append("#").Append(match.SourceCodeMatch.DetailData.Line);
                    }
                    if (!string.IsNullOrEmpty(match.SourceCodeMatch.DetailData.Comment)) {
                        sb.Append(" § ").Append(match.SourceCodeMatch.DetailData.Comment);
                    }
                }
                sb.AppendLine();
            }
        }
        return Task.CompletedTask;
    }

    private void parseListBlock(ListBlock listBlock) {
        foreach (var subBlock in listBlock) {
            if (subBlock is ListBlock subListBlock) {
                parseListBlock(subListBlock);
            }
        }
    }

    public IReplacementFinder? GetReplacementFinder(
        MarkdownDocumentInfo markdownDocumentInfo,
        SourceCodeData sourceCodeMatch) {
        var result = new ListBlockReplacementFinder(this, sourceCodeMatch);
        markdownDocumentInfo.GetLstReplacementFinder().Add(result);
        return result;
    }
}
