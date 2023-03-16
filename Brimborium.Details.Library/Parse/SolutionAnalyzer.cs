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
        var parserSinkContext = this._RootRepository.GetParserSinkContext();
        CSharpContext? csharpContext = default;
        try {
            {
                csharpContext = await this._CSharpService.PrepareSolutionCSharp(parserSinkContext, cancellationToken);
                var markdownContext = await this._MarkdownService.PrepareSolutionDetail(parserSinkContext, cancellationToken);

                var t1 = csharpContext is null
                    ? Task.CompletedTask
                    : this._CSharpService.ParseCSharp(parserSinkContext, csharpContext, cancellationToken);
                await t1;

                var t2 = markdownContext is null
                    ? Task.CompletedTask
                    : this._MarkdownService.ParseDetail(parserSinkContext, markdownContext, cancellationToken);
                await t2;
                // § todo.md

                var t3 = this._TypeScriptService.ParseTypeScript(parserSinkContext, cancellationToken);
                await t3;

                await Task.WhenAll(t1, t2, t3)
                    .WaitAsync(cancellationToken)
                    .ConfigureAwait(false);

                // HACK ONLY
                {
                    var detailsRoot = this._RootRepository.GetSolutionData().DetailsRoot;
                    var targetPath = parserSinkContext.DetailsRoot.CreateWithRelativePath("detailsRootRepository.json").AbsolutePath;
                    Console.Out.WriteLine($"targetPath: {targetPath}");
                    if (targetPath is not null) {
                        await File.WriteAllTextAsync(
                            targetPath,
                            JsonSerializer.Serialize(this._RootRepository, new JsonSerializerOptions() { WriteIndented = true }),
                            cancellationToken)
                            .ConfigureAwait(false);
                    }
                }
            }
            {
                var writerContext = this._RootRepository.GetWriterContext(parserSinkContext);
                if (writerContext is null) {
                    return;
                }

                // HACK ONLY
                {
                    var detailsRoot = this._RootRepository.GetSolutionData().DetailsRoot;
                    var targetPath = detailsRoot.CreateWithRelativePath("detailsWriterContext.json").AbsolutePath;
                    Console.Out.WriteLine($"targetPath: {targetPath}");
                    if (targetPath is not null) {
                        /*
                        await File.WriteAllTextAsync(
                            targetPath,
                            JsonSerializer.Serialize(writerContext!, typeof(WriterContext), WriterContextJsonContext.Default),
                            cancellationToken)
                            .ConfigureAwait(false);

                         */
                        await File.WriteAllTextAsync(
                            targetPath,
                            JsonSerializer.Serialize<WriterContext>(writerContext,
                            new JsonSerializerOptions(JsonSerializerOptions.Default) {
                                WriteIndented = true,
                                Converters ={
                                new JsonStringEnumConverter()
                                }
                            }),
                            cancellationToken)
                            .ConfigureAwait(false);
                    }
                }

                var t1 = this._MarkdownService.WriteDetail(writerContext, cancellationToken);
                var t2 = this._CSharpService.WriteDetail(writerContext, cancellationToken);

                await Task.WhenAll(t1, t2)
                    .WaitAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
        } finally {
            if (csharpContext is not null) {
                csharpContext.Dispose();
            }
        }
    }
}
