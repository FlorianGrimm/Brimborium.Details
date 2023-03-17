namespace Brimborium.Details.Enhancement;

//
[Transient]
public class CommandShowListOrderByComment : IMatchCommand {
    private readonly ILogger<CommandShowListOrderByComment> _Logger;

    public CommandShowListOrderByComment(
        ILogger<CommandShowListOrderByComment> logger
        ) {
        this._Logger = logger;
    }

    public bool IsMatching(DetailData matchInfo)
        => matchInfo.Command == "Show-List-OrderByComment";

    public Task ExecuteAsync(
        SourceCodeData sourceCodeMatch,
        MarkdownDocumentWriter markdownDocumentWriter,
        IReplacementFinder? replacementFinder,
        CancellationToken cancellationToken) {
        var detailContext = markdownDocumentWriter.WriterContext;
        var matchInfo = sourceCodeMatch.DetailData;

        var path = matchInfo.Path;
        if (path is null || path.IsEmpty()) {
            // § todo.md why does matchInfo.MatchPath not work? why is contentpath empty?
            path = matchInfo.MatchPath.WithLine(0);
            //var relativePath =
            //    markdownDocumentWriter.MarkdownDocumentInfo.FileName.Rebase(
            //        markdownDocumentWriter.WriterContext.DetailsFolder
            //    )?.RelativePath
            //    ?? string.Empty;
            //path = PathData.Parse(relativePath);
        }

        this._Logger.LogDebug("Show-List: {0}", path);

        var lstMatch = detailContext.QueryPath(path);

        //ListBlock? target = default;
        Range? targetRange = default;
        if (replacementFinder is null) { return Task.CompletedTask; }
        if (replacementFinder is not ListBlockReplacementFinder listBlockReplacementFinder) { return Task.CompletedTask; }

        if (listBlockReplacementFinder.Range is not null) {
            targetRange = listBlockReplacementFinder.Range;
        }

        if (targetRange is null) {
            var source = markdownDocumentWriter.GetBlockByLine(sourceCodeMatch.DetailData.Line);
            if (source is null) {
                return Task.CompletedTask;
            }
            targetRange = markdownDocumentWriter.GetRangeAfter(source);
        }

        if (targetRange is null) {
            return Task.CompletedTask;
        }
        var replacementContent = markdownDocumentWriter.CreatePart(targetRange.Value);
        if (replacementContent is null) {
            return Task.CompletedTask;
        }
        var sb = replacementContent.GetReplacementBuilder();
        if (lstMatch.Count == 0) {
            sb.Append("- No Matches").AppendLine();
        } else {
            lstMatch = lstMatch.OrderBy(match => match.SourceCodeMatch.DetailData.Comment).ToList();
            foreach (var match in lstMatch) {
                //string? link;
                //if (string.IsNullOrEmpty(match.SourceCodeMatch.Match.MatchPath.ContentPath)) {
                //    link = match.SourceCodeMatch.Match.MatchPath.FilePath;
                //} else {
                //    link = match.SourceCodeMatch.Match.MatchPath.FilePath;
                //}
                sb.Append("- ");
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


    public IReplacementFinder? GetReplacementFinder(
        MarkdownDocumentInfo markdownDocumentInfo,
        SourceCodeData sourceCodeMatch) {
        var result = new ListBlockReplacementFinder(this, sourceCodeMatch);
        markdownDocumentInfo.GetLstReplacementFinder().Add(result);
        return result;
    }
}
