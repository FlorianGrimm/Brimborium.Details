namespace Brimborium.Details;

[Brimborium.Registrator.Singleton]
public class SolutionAnalyzer
    : ISolutionAnalyzer {
    private readonly MarkdownService _MarkdownService;
    private readonly CSharpService _CSharpService;
    private readonly TypeScriptService _TypeScriptService;

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

        var detailContext = new DetailContext(solutionInfo);
        {
            var csharpContext = await this._CSharpService.PrepareSolutionCSharp(detailContext, cancellationToken);
            var markdownContext = await this._MarkdownService.PrepareSolutionDetail(detailContext, cancellationToken);

            var t1 = (csharpContext is null)
                ? Task.CompletedTask
                : this._CSharpService.ParseCSharp(detailContext, csharpContext, cancellationToken);
            await t1;

            var t2 = (markdownContext is null)
                ? Task.CompletedTask
                : this._MarkdownService.ParseDetail(detailContext, markdownContext, cancellationToken);
            await t2;
            // § todo.md

            var t3 = this._TypeScriptService.ParseTypeScript(detailContext, cancellationToken);
            await t3;

            await Task.WhenAll(t1, t2, t3)
                .WaitAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        {
            var targetPath = solutionInfo.DetailsRoot.CreateWithRelativePath("detailContext.json").AbsolutePath;
            System.Console.Out.WriteLine($"targetPath: {targetPath}");
            if (targetPath is not null) {
                await System.IO.File.WriteAllTextAsync(
                    targetPath,
                    System.Text.Json.JsonSerializer.Serialize(detailContext, new System.Text.Json.JsonSerializerOptions() { WriteIndented = true }),
                    cancellationToken)
                    .ConfigureAwait(false);
            }
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
