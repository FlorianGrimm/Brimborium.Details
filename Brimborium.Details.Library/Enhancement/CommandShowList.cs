namespace Brimborium.Details.Enhancement;

//
[Transient]
public class CommandShowList : IMatchCommand {
    public CommandShowList() {
    }

    public bool IsMatching(DetailData matchInfo)
        => matchInfo.Command == "Show-List";

    public Task ExecuteAsync(
        SourceCodeData sourceCodeMatch,
        MarkdownDocumentWriter markdownDocumentWriter,
        IReplacementFinder? replacementFinder,
        DetailContextCache? cache,
        CancellationToken cancellationToken) {
        var detailContext = markdownDocumentWriter.DetailContext;
        var matchInfo = sourceCodeMatch.DetailData;

        var path = matchInfo.Path;
        if (path is null || path.IsEmpty()) {
            // § todo.md
            var relativePath =
                markdownDocumentWriter.MarkdownDocumentInfo.FileName.Rebase(
                    markdownDocumentWriter.DetailContext.SolutionData.DetailsFolder
                )?.RelativePath
                ?? string.Empty;
            path = PathData.Parse(relativePath);
        }

        var lstMatch = detailContext.QueryPath(path, cache);

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
            foreach (var match in lstMatch) {
                //string? link;
                //if (string.IsNullOrEmpty(match.SourceCodeMatch.Match.MatchPath.ContentPath)) {
                //    link = match.SourceCodeMatch.Match.MatchPath.FilePath;
                //} else {
                //    link = match.SourceCodeMatch.Match.MatchPath.FilePath;
                //}
                sb.Append("- ");
                if (match.SourceCodeMatch.FilePath.RootFolder == markdownDocumentWriter.DetailContext.SolutionData.DetailsFolder) {
                    sb.Append("details://").Append(match.SourceCodeMatch.FilePath.RelativePath);
                    if (match.SourceCodeMatch.DetailData.Line > 0) {
                        sb.Append("#").Append(match.SourceCodeMatch.DetailData.Line);
                    }
                } else {
                    sb.Append("detailscode://").Append(match.SourceCodeMatch.FilePath.RelativePath);
                    if (match.SourceCodeMatch.DetailData.Line > 0) {
                        sb.Append("#").Append(match.SourceCodeMatch.DetailData.Line);
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
