namespace Brimborium.Details.Parse;

public interface ISolutionAnalyzerFactory {
    ISolutionAnalyzer GetSolutionAnalyzer(
        IRootRepository rootRepository);
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
        IRootRepository rootRepository
        ) {
        return new SolutionAnalyzer(
            rootRepository,
            this._ServiceProvider.GetRequiredService<MarkdownService>(),
            this._ServiceProvider.GetRequiredService<CSharpService>(),
            this._ServiceProvider.GetRequiredService<TypeScriptService>(),
            this._ServiceProvider.GetRequiredService<ILogger<SolutionAnalyzer>>()
            );
    }
}

public interface ISolutionAnalyzer {
    Task<IRepeat?> AnalyzeAsync(
        IWatchServiceConfigurator watchServiceConfigurator,
        CancellationToken cancellationToken);
}

public class SolutionAnalyzer
    : ISolutionAnalyzer {
    private readonly IRootRepository _RootRepository;
    private readonly MarkdownService _MarkdownService;
    private readonly CSharpService _CSharpService;
    private readonly TypeScriptService _TypeScriptService;
    private readonly ILogger<SolutionAnalyzer> _Logger;

    public SolutionAnalyzer(
        IRootRepository rootRepository,
        MarkdownService markdownService,
        CSharpService csharpService,
        TypeScriptService typeScriptService,
        ILogger<SolutionAnalyzer> logger
        ) {
        this._RootRepository = rootRepository;
        this._MarkdownService = markdownService;
        this._CSharpService = csharpService;
        this._TypeScriptService = typeScriptService;
        this._Logger = logger;
    }

    public async Task<IRepeat?> AnalyzeAsync(
        IWatchServiceConfigurator watchServiceConfigurator,
        CancellationToken cancellationToken) {
        var parserSinkContext = this._RootRepository
            .GetParserSinkContext(watchServiceConfigurator);

        var repeatComplete = new RepeatComplete(this);
        var repeatDifferential = await this.ParseAsync(repeatComplete, parserSinkContext, cancellationToken);

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
        var writerContext = this._RootRepository.GetWriterContext(parserSinkContext);
        if (writerContext is null) {
            return repeatDifferential;
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

        await this.WriteDetailAsync(writerContext, cancellationToken);

        return repeatDifferential;
    }

    public async Task<IRepeat> ParseAsync(
        RepeatComplete repeatComplete,
        IParserSinkContext parserSinkContext,
        CancellationToken cancellationToken) {
        var csharpContext = await this._CSharpService.PrepareSolutionCSharp(parserSinkContext, cancellationToken);
        var markdownContext = await this._MarkdownService.PrepareSolutionDetail(parserSinkContext, cancellationToken);
        var repeat = new RepeatDifferential(repeatComplete, this, csharpContext, markdownContext);

        var taskParseCSharp = csharpContext is null
            ? Task.CompletedTask
            : this._CSharpService.ParseCSharp(parserSinkContext, csharpContext, cancellationToken);
        await taskParseCSharp;

        var taskParseDetail = markdownContext is null
            ? Task.CompletedTask
            : this._MarkdownService.ParseDetail(parserSinkContext, markdownContext, cancellationToken);
        await taskParseDetail;
        // § todo.md

        var taskParseTypeScript = this._TypeScriptService.ParseTypeScript(parserSinkContext, cancellationToken);
        await taskParseTypeScript;

        await Task.WhenAll(taskParseCSharp, taskParseDetail, taskParseTypeScript)
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);

        return repeat;
    }

    public async Task WriteDetailAsync(WriterContext writerContext, CancellationToken cancellationToken) {
        var taskWriteMarkdownDetail = this._MarkdownService.WriteMarkdownDetail(writerContext, cancellationToken);
        var taskWriteCSharpDetail = this._CSharpService.WriteCSharpDetail(writerContext, cancellationToken);

        await Task.WhenAll(
                taskWriteMarkdownDetail,
                taskWriteCSharpDetail)
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
public interface IRepeat : IDisposable {
    Task<IRepeat?> RepeatDifferentialAsync(
        IWatchServiceChanges watchServiceChanges,
        CancellationToken cancellationToken);

    Task<IRepeat?> RepeatCompleteAsync(
        IWatchServiceConfigurator watchServiceConfigurator,
        CancellationToken cancellationToken);
}
public class RepeatComplete
    : IRepeat
    , IDisposable {
    private RepeatDifferential? _RepeatDifferential;
    private SolutionAnalyzer _SolutionAnalyzer;

    public RepeatComplete(SolutionAnalyzer solutionAnalyzer) {
        this._SolutionAnalyzer = solutionAnalyzer;
    }

    public async Task<IRepeat?> RepeatDifferentialAsync(IWatchServiceChanges watchServiceChanges, CancellationToken cancellationToken) {
        await Task.CompletedTask;
        return null;
    }

    public async Task<IRepeat?> RepeatCompleteAsync(IWatchServiceConfigurator watchServiceConfigurator, CancellationToken cancellationToken) {
        await Task.CompletedTask;
        return null;
    }

    public void Dispose() {
    }
}
public class RepeatDifferential
    : IRepeat
    , IDisposable {
    private readonly RepeatComplete _RepeatComplete;
    private readonly ISolutionAnalyzer _SolutionAnalyzer;
    private CSharpContext? _CsharpContext;
    private MarkdownContext? _MarkdownContext;

    public RepeatDifferential(
        RepeatComplete repeatComplete,
        ISolutionAnalyzer solutionAnalyzer,
        CSharpContext? csharpContext,
        MarkdownContext? markdownContext) {
        this._RepeatComplete = repeatComplete;
        this._SolutionAnalyzer = solutionAnalyzer;
        this._CsharpContext = csharpContext;
        this._MarkdownContext = markdownContext;
    }

    public async Task<IRepeat?> RepeatDifferentialAsync(
        IWatchServiceChanges watchServiceChanges,
        CancellationToken cancellationToken) {
        await Task.CompletedTask;
        return null;
    }

    public async Task<IRepeat?> RepeatCompleteAsync(
        IWatchServiceConfigurator watchServiceConfigurator,
        CancellationToken cancellationToken) {
        await Task.CompletedTask;
        return null;
    }

    private void Dispose(bool disposing) {
        using (var csharpContext = this._CsharpContext) {
            if (disposing) {
                this._CsharpContext = null;
            }
        }
    }

    ~Repeat() {
        this.Dispose(disposing: false);
    }

    public void Dispose() {
        this.Dispose(disposing: true);
        System.GC.SuppressFinalize(this);
    }
}