namespace Brimborium.Details;

//
[Brimborium.Registrator.Transient]
public class CommandShowList : IMatchCommand {
    public CommandShowList() {
    }

    public bool IsMatching(MatchInfo matchInfo)
        => matchInfo.Command == "Show-List";

    public Task ExecuteAsync(
        SourceCodeMatch sourceCodeMatch,
        MarkdownDocumentWriter markdownDocumentWriter,
        IReplacementFinder? replacementFinder,
        DetailContextCache? cache,
        CancellationToken cancellationToken) {
        var detailContext = markdownDocumentWriter.DetailContext;
        var matchInfo = sourceCodeMatch.Match;

        var path = matchInfo.Path;
        if (path is null || path.IsEmpty()) {
            // § todo.md
            string relativePath =
                markdownDocumentWriter.MarkdownDocumentInfo.FileName.Rebase(
                    markdownDocumentWriter.DetailContext.SolutionInfo.DetailsFolder
                )?.RelativePath
                ?? string.Empty;
            path = PathInfo.Parse(relativePath);
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
            var source = markdownDocumentWriter.GetBlockByLine(sourceCodeMatch.Match.Line);
            if (source is null) {
                return Task.CompletedTask;
            }
            targetRange = markdownDocumentWriter.GetRangeAfter(source);
        }
        
        if (targetRange is  null) {
            return Task.CompletedTask;
        }
        var replacementContent = markdownDocumentWriter.CreatePart(targetRange.Value);
        if (replacementContent is null) {
            return Task.CompletedTask;
        }
        var sb=replacementContent.GetReplacementBuilder();
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
                if (match.SourceCodeMatch.FilePath.RootFolder == markdownDocumentWriter.DetailContext.SolutionInfo.DetailsFolder) {
                    sb.Append("details://").Append(match.SourceCodeMatch.FilePath.RelativePath);
                    if (match.SourceCodeMatch.Match.Line > 0) {
                        sb.Append("#").Append(match.SourceCodeMatch.Match.Line);
                    }
                } else {
                    sb.Append("detailscode://").Append(match.SourceCodeMatch.FilePath.RelativePath);
                    if (match.SourceCodeMatch.Match.Line > 0) {
                        sb.Append("#").Append(match.SourceCodeMatch.Match.Line);
                    }
                }
                sb.AppendLine();
            }
        }
        return Task.CompletedTask;
    }

    
    public IReplacementFinder? GetReplacementFinder(
        MarkdownDocumentInfo markdownDocumentInfo,
        SourceCodeMatch sourceCodeMatch) {
        var result = new ListBlockReplacementFinder(this, sourceCodeMatch);
        markdownDocumentInfo.GetLstReplacementFinder().Add(result);
        return result;
    }
}
