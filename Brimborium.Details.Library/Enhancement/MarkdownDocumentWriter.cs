namespace Brimborium.Details.Enhancement;

public class MarkdownDocumentWriter {
    private readonly WriterContext _WriterContext;
    private readonly MarkdownDocumentInfo _MarkdownDocumentInfo;
    private readonly string _MarkdownContent;
    private readonly MarkdownDocument _Document;
    private readonly IFileSystem _FileSystem;

    public MarkdownDocumentWriter(
        WriterContext writerContext,
        MarkdownDocumentInfo markdownDocumentInfo,
        string markdownContent,
        MarkdownDocument document,
        IFileSystem fileSystem
        ) {
        this._WriterContext = writerContext;
        this._MarkdownDocumentInfo = markdownDocumentInfo;
        this._MarkdownContent = markdownContent;
        this._Document = document;
        this._FileSystem = fileSystem;
    }

    public WriterContext WriterContext => this._WriterContext;

    public MarkdownDocumentInfo MarkdownDocumentInfo => this._MarkdownDocumentInfo;

    public string MarkdownContent => this._MarkdownContent;

    public MarkdownDocument Document => this._Document;

    public StringSplice? ContentSplice { get; private set; }

    //public async Task ReadAsync(CancellationToken cancellationToken) {
    //    var markdownContent = await this._FileSystem.ReadAllTextAsync(this._MarkdownDocumentInfo.FileName, Encoding.UTF8, cancellationToken);
    //    //MarkdownDocument document = Markdown.Parse(markdownContent, this._Pipeline);

    //    await Task.CompletedTask;
    //}

    public async Task WriteAsync(CancellationToken cancellationToken) {
        if (this.ContentSplice is null) { return; }
        var newContent = this.ContentSplice.BuildReplacement();
        if (this.MarkdownContent.Equals(newContent)) {
            return;
        } else {
            await this._FileSystem.WriteAllTextAsync(this.MarkdownDocumentInfo.FileName, Encoding.UTF8, newContent, cancellationToken);
        }
    }

    public Block? GetBlockByLine(int line) {
        if (this.Document.LineStartIndexes is not List<int> lineStartIndexes) { return null; }
        if (lineStartIndexes.Count <= line) { return null; }
        var startIndex = lineStartIndexes[line];
        var block = this.Document.FindBlockAtPosition(startIndex);
        return block;
    }

    public Range? GetRange(Block source) {
        //if (this.Document.LineStartIndexes is not List<int> lineStartIndexes) { return null; }
        //if (lineStartIndexes.Count <= source.Line) { return null; }
        //var startIndex = lineStartIndexes[source.Line];
        //return new Range(startIndex+ source.Span.Start, startIndex+source.Span.End);
        return new Range(source.Span.Start, source.Span.End + ((int)source.NewLine & 3) + 1);
    }

    public Range? GetRangeAfter(Block source) {
        //if (this.Document.LineStartIndexes is not List<int> lineStartIndexes) { return null; }
        //if (lineStartIndexes.Count <= source.Line) { return null; }
        //var startIndex = lineStartIndexes[source.Line];
        //var end = startIndex + source.Span.End + ((int)source.NewLine & 3);
        var end = source.Span.End + ((int)source.NewLine & 3) + 1;
        return new Range(end, end);
    }

    public StringSplice? CreatePart(Range range) {
        if (this.ContentSplice is null) {
            this.ContentSplice = new StringSplice(this.MarkdownContent);
        }
        return this.ContentSplice.CreatePart(range);
    }
}
