namespace Brimborium.Details;

[Brimborium.Registrator.Singleton]
    public class SolutionAnalyzer 
    : ISolutionAnalyzer {
    private readonly MarkdownService _MarkdownService;
    private readonly CSharpService _CSharpService;
    private readonly TypeScriptService _TypeScriptService;
    
    private SolutionInfo? _SolutionInfo;
    
    public SolutionAnalyzer(
        MarkdownService markdownService,
        CSharpService csharpService,
        TypeScriptService typeScriptService
        ) {
        this._MarkdownService = markdownService;
        this._CSharpService = csharpService;
        this._TypeScriptService = typeScriptService;
    }

    public async Task AnalyzeAsync(SolutionInfo solutionInfo, CancellationToken cancellationToken) {
        this._SolutionInfo = solutionInfo;

        var detailContext = new DetailContext();
        {
            var t1 = this._MarkdownService.ParseDetail(detailContext, cancellationToken);
            // § todo.md
            var t2 = this._CSharpService.ParseCSharp(detailContext, cancellationToken);
            var t3 = this._TypeScriptService.ParseTypeScript(detailContext, cancellationToken);

            await Task.WhenAll(t1, t2, t3)
                .WaitAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        {
            var t1 = this._MarkdownService.WriteDetail(detailContext, cancellationToken);
            var t2 = this._CSharpService.WriteDetail(detailContext, cancellationToken);

            await Task.WhenAll(t1, t2)
                .WaitAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
