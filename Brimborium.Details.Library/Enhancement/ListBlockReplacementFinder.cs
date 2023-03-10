namespace Brimborium.Details.Enhancement;

public class ListBlockReplacementFinder : IReplacementFinder {
    public Range? Range;

    public ListBlockReplacementFinder(
        IMatchCommand command,
        SourceCodeData sourceCodeMatch) {
        this.Command = command;
        this.SourceCodeMatch = sourceCodeMatch;
    }

    public IMatchCommand Command { get; }
    public SourceCodeData SourceCodeMatch { get; }

    public bool VisitBlock(Block block) {
        if (block is ListBlock listBlock) {
            var lastBlock = listBlock;
            var parent = block.Parent!;
            var idxFound = -1;
            for (var idx = 0; idx < parent.Count; idx++) {
                if (ReferenceEquals(parent[idx], listBlock)) {
                    idxFound = idx;
                    break;
                }
            }
            if (idxFound >= 0) {
                for (var idx = idxFound + 1; idx < parent.Count; idx++) {
                    if (parent[idx] is ListBlock nextListBlock) {
                        lastBlock = nextListBlock;
                    } else {
                        break;
                    }
                }
            }

            this.Range = new Range(
                block.Span.Start,
                lastBlock.Span.End + ((int)lastBlock.NewLine & 3) + 1);
            return true;
        }
        return false;
    }

    public void VisitNotFound() {
        this.Range = null;
    }
}
