namespace Brimborium.Details.Enhancement;

public interface IReplacementFinder {
    IMatchCommand Command { get; }
    SourceCodeData SourceCodeMatch { get; }

    /// <summary>
    /// The parser will call this method for the next blocks it finds in the document.
    /// </summary>
    /// <param name="block">the block to find.</param>
    /// <returns>true if found</returns>
    bool VisitBlock(Block block);

    /// <summary>
    /// The parser notify you to give up.
    /// </summary>
    void VisitNotFound();
}
