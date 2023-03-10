namespace Brimborium.Details.Parse;

public interface ISolutionAnalyzerFactory {
    ISolutionAnalyzer GetSolutionAnalyzer(
        RootRepository rootRepository);
}

[Brimborium.Registrator.Singleton]
public class SolutionAnalyzerFactory : ISolutionAnalyzerFactory {
    private readonly IServiceProvider _ServiceProvider;

    public SolutionAnalyzerFactory(
        IServiceProvider serviceProvider
        ) {
        this._ServiceProvider = serviceProvider;
    }
    public ISolutionAnalyzer GetSolutionAnalyzer(
        RootRepository rootRepository
        ) {
        return new SolutionAnalyzer(
            rootRepository,
            this._ServiceProvider.GetRequiredService<MarkdownService>(),
            this._ServiceProvider.GetRequiredService<CSharpService>(),
            this._ServiceProvider.GetRequiredService<TypeScriptService>()
            );
    }
}

public interface ISolutionAnalyzer {
    Task AnalyzeAsync(
        CancellationToken cancellationToken);
}

public class SolutionAnalyzer
    : ISolutionAnalyzer {
    private readonly RootRepository _RootRepository;
    private readonly MarkdownService _MarkdownService;
    private readonly CSharpService _CSharpService;
    private readonly TypeScriptService _TypeScriptService;

    public SolutionAnalyzer(
        RootRepository rootRepository,
        MarkdownService markdownService,
        CSharpService csharpService,
        TypeScriptService typeScriptService
        ) {
        this._RootRepository = rootRepository;
        this._MarkdownService = markdownService;
        this._CSharpService = csharpService;
        this._TypeScriptService = typeScriptService;
    }

    public async Task AnalyzeAsync(
        CancellationToken cancellationToken) {
        var detailContext = this._RootRepository.GetParserSinkContext();
        {
            var csharpContext = await this._CSharpService.PrepareSolutionCSharp(detailContext, cancellationToken);
            var markdownContext = await this._MarkdownService.PrepareSolutionDetail(detailContext, cancellationToken);

            var t1 = csharpContext is null
                ? Task.CompletedTask
                : this._CSharpService.ParseCSharp(detailContext, csharpContext, cancellationToken);
            await t1;

            var t2 = markdownContext is null
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
        // HACK ONLY
        {
            var targetPath = detailContext.DetailsRoot.CreateWithRelativePath("detailContext.json").AbsolutePath;
            Console.Out.WriteLine($"targetPath: {targetPath}");
            if (targetPath is not null) {
                await File.WriteAllTextAsync(
                    targetPath,
                    JsonSerializer.Serialize(detailContext, new JsonSerializerOptions() { WriteIndented = true }),
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
