using System.Runtime.CompilerServices;

namespace Brimborium.Details.Parse;

[Transient]
public partial class CSharpService {
    private static bool _RegisterDefaultsIsCalled = false;
    public static void RegisterDefaults() {
        if (!_RegisterDefaultsIsCalled) {
            Microsoft.Build.Locator.MSBuildLocator.RegisterDefaults();
            _RegisterDefaultsIsCalled = true;
        }
    }

    private static Regex regexSimple = new Regex("//[ \t]*§([^\\r\\n]+)");
    
    private readonly ILogger<CSharpService> _Logger;

    public CSharpService(
        ILogger<CSharpService> logger
        ) {
        this._Logger = logger;
    }

    [LoggerMessage(
        EventId = (int)LogMessageId.SolutionLoadStart,
        EventName = nameof(LogMessageId.SolutionLoadStart),
        Level = LogLevel.Information,
        Message = "Loading solution '{solutionFile}'")]
    partial void LogSolutionLoadStart(string? solutionFile);

    [LoggerMessage(
        EventId = (int)LogMessageId.SolutionLoadSuccess,
        EventName = nameof(LogMessageId.SolutionLoadSuccess),
        Level = LogLevel.Information,
        Message = "Solution loaded '{solutionFile}'")]
    partial void LogSolutionLoadSuccess(string? solutionFile);
    
    [LoggerMessage(
        EventId = (int)LogMessageId.SolutionLoadFailed,
        EventName = nameof(LogMessageId.SolutionLoadFailed),
        Level = LogLevel.Warning,
        Message = "Loading solution failed '{solutionFile}'")]
    partial void LogSolutionLoadFailed(string? solutionFile);

    [LoggerMessage(
        EventId = (int)LogMessageId.SolutionWorkspaceFailed,
        EventName = nameof(LogMessageId.SolutionWorkspaceFailed),
        Level = LogLevel.Warning,
        Message = "Loading solution failed '{solutionFile}': {message}")]
    partial void LogSolutionWorkspaceFailed(string? solutionFile, string? message);

    [LoggerMessage(
        EventId = (int)LogMessageId.ProjectLoadSuccess,
        EventName = nameof(LogMessageId.ProjectLoadSuccess),
        Level = LogLevel.Information,
        Message = "Loaded Project: {projectName} (ListMainProjectName).")]
    partial void LogProjectLoadSuccess(string? projectName);

    [LoggerMessage(
        EventId = (int)LogMessageId.ProjectLoadFailed,
        EventName = nameof(LogMessageId.ProjectLoadFailed),
        Level = LogLevel.Warning,
        Message = "Cannot load {projectName} (ListMainProjectName)- {reason}.")]
    partial void LogProjectLoadFailed(string? projectName, string reason);

    public async Task<CSharpContext?> PrepareSolutionCSharp(
        IParserSinkContext parserSinkContext,
        CancellationToken cancellationToken) {
        var solutionInfo = parserSinkContext.SolutionData;
        var solutionFile = solutionInfo.SolutionFile.AbsolutePath;
        if (solutionFile is null) { return null; }
        this.LogSolutionLoadStart(solutionFile);
        RegisterDefaults();

        var workspace = MSBuildWorkspace.Create();
        workspace.WorkspaceFailed += (sender, args) => {
            //Console.WriteLine(args.Diagnostic.Message);
            this.LogSolutionWorkspaceFailed(solutionFile, args.Diagnostic.Message);
        };
        var solution = await workspace.OpenSolutionAsync(solutionFile);
        
        /*
        foreach (var project in solution.Projects) {
            if (project is not null) {
                System.Console.Out.WriteLine($"{project.Name} - {project.Id}");
            }
        }
        */

        var lstMainProject = new List<Project>();
        var queue = new Queue<Project>();
        foreach (var projectName in solutionInfo.ListMainProjectName) {
            var project = solution.Projects.Where(project => project.Name == projectName).FirstOrDefault();
            if (project is null) {
                this.LogProjectLoadFailed(projectName, "not found");
                continue;
            } else {
                lstMainProject.Add(project);
                queue.Enqueue(project);
                this.LogProjectLoadSuccess(projectName);
            }
        }
        if (lstMainProject.Count == 0) {
            foreach (var project in solution.Projects.Where(project => project.Language == "C#")) {
                lstMainProject.Add(project);
                queue.Enqueue(project);
            }
        }

        // TODO: make configurable
        //var filterPath = System.IO.Path.Combine(SolutionInfo.DetailsRoot, "src");
        var filterPath = solutionInfo.DetailsRoot.AbsolutePath
            ?? throw new InvalidOperationException("SolutionInfo.DetailsRoot is empty.");
        // Console.Out.WriteLine($"INFO: filterPath {filterPath}");

        var projectDependencyGraph = solution.GetProjectDependencyGraph();
        var lstRelevantProject = new List<Project>();
        var hsRelevantProject = new HashSet<ProjectId>();
        while (queue.Count > 0) {
            var project = queue.Dequeue();
            if (hsRelevantProject.Contains(project.Id)) {
                continue;
            }
            hsRelevantProject.Add(project.Id);
            if (project.FilePath is null) {
                this.LogProjectLoadFailed(project.Name, "ProjectDependency  - no file path.");
                continue;
            }
            //if (project.FilePath.EndsWith("Tests.csproj", StringComparison.InvariantCultureIgnoreCase)
            //    || project.FilePath.EndsWith("Test.csproj", StringComparison.InvariantCultureIgnoreCase)) {
            //    Console.Out.WriteLine($"INFO: ProjectDependency {project.Name} - {project.Id} - skipped");
            //    continue;
            //}
            if (!project.FilePath.StartsWith(filterPath, StringComparison.InvariantCultureIgnoreCase)) {
                this.LogProjectLoadFailed(project.Name, "skipped.");
                continue;
            }
            lstRelevantProject.Add(project);

            var lstProjectId = projectDependencyGraph.GetProjectsThatThisProjectTransitivelyDependsOn(project.Id);
            foreach (var projectId in lstProjectId) {
                var projectReference = solution.GetProject(projectId);
                if (projectReference is not null) {
                    queue.Enqueue(projectReference);
                }
            }
        }

        lstRelevantProject.Sort((a, b) => StringComparer.InvariantCulture.Compare(a.Name, b.Name));
        var lstRelevantProjectProjectInfo = new List<RoslynProjectProjectData>();
        foreach (var project in lstRelevantProject) {
            if (project.FilePath is null) {
                this.LogProjectLoadFailed(project.Name, "project.FilePath is null.");
                continue;
            }

            var lstDocument = new List<FileName>();
            foreach (var document in project.Documents) {
                var filePath = document.FilePath;
                if (filePath is null) { continue; }
                lstDocument.Add(new FileName() {
                    AbsolutePath = filePath,
                });
            }

            var projectFileName = parserSinkContext.ConvertToFileName(project.FilePath);
            var projectData = new ProjectData(
                    project.Name,
                    projectFileName,
                    "CSharp",
                    projectFileName.GetParentDirectory() ?? FileName.Empty
                    );
            projectData = parserSinkContext.GetOrAddProject(projectData);
            parserSinkContext.SetProjectDocuments(projectData, lstDocument);
            lstRelevantProjectProjectInfo.Add(
                new RoslynProjectProjectData(
                    project,
                    projectData));
        }
        var csharpContext = new CSharpContext(
            workspace,
            solution,
            lstRelevantProjectProjectInfo);
        return csharpContext;
    }

    public async Task ParseCSharp(
        IParserSinkContext parserSinkContext,
        CSharpContext csharpContext,
        CancellationToken cancellationToken
        ) {
        var solutionInfo = parserSinkContext.SolutionData;
        var solution = csharpContext.Solution;
        var lstCompilingProjects = new List<RoslynProjectProjectData>();

        await ParallelUtility.ForEachAsync(
            csharpContext.lstRelevantProjectProjectInfo,
            cancellationToken,
            async (roslynProjectProjectData, cancellationToken) => {
                var project = roslynProjectProjectData.Project;
                var compilation = await project.GetCompilationAsync();
                if (compilation is null) {
                    Console.Out.WriteLine($"INFO: Compiles: {project.Name} - {project.Id} compilation is null");
                } else {
                    Console.Out.WriteLine($"INFO: Compiles: {project.Name} - {project.Id} - OK");
                    lock (lstCompilingProjects) {
                        lstCompilingProjects.Add(roslynProjectProjectData);
                    }
                }
            }
        );

#if false
        foreach (var project in lstRelevantProject) {
            var compilation = await project.GetCompilationAsync();
            if (compilation is null) {
                System.Console.Out.WriteLine($"INFO: Compiles: {project.Name} - {project.Id} compilation is null");
            } else {
                System.Console.Out.WriteLine($"INFO: Compiles: {project.Name} - {project.Id} - OK");
                lstCompilingProjects.Add(project);
                /*
                var visitor=new MethodSymbolVisitor();
                visitor.Visit(compilation.Assembly);
                */

                /*
                var lstSymbolsWithName = compilation.GetSymbolsWithName(symbol => true, SymbolFilter.TypeAndMember, CancellationToken.None).ToList();
                foreach (var symbolsWithName in lstSymbolsWithName) {
                    if (symbolsWithName is INamedTypeSymbol
                        || symbolsWithName is IMethodSymbol
                        || symbolsWithName is IPropertySymbol
                        ) {
                        // OK
                    } else {
                        continue;
                    }
                    var declaringSyntaxReferences = symbolsWithName.DeclaringSyntaxReferences;
                        var declaringSyntax =
                            declaringSyntaxReferences
                                .OrderBy(item => item.SyntaxTree.FilePath).ThenBy(item => item.Span.Start)
                                .Where(item => !string.IsNullOrEmpty(item.SyntaxTree.FilePath))
                                .FirstOrDefault();
                        if (declaringSyntax is null) { continue; }
var filePath =                         solutionInfo.GetRelativePath(declaringSyntax.SyntaxTree.FilePath);
                    if (symbolsWithName is INamedTypeSymbol namedTypeSymbol) {
                        // declaringSyntax.SyntaxTree.FilePath.Substring(rootPath.Length + 1);
                        // namedTypeSymbol.ContainingNamespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                        // namedTypeSymbol.Name
                        // namedTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    }
                    if (symbolsWithName is IMethodSymbol methodSymbol) {
                        //var sematicModel = compilation.GetSemanticModel(declaringSyntax.SyntaxTree);
                        )
                    }
                    if (symbolsWithName is IPropertySymbol propertySymbol) {

                    }
                }
            
                */
            }
        }
#endif

        foreach (var projectProjectInfo in lstCompilingProjects) {
            var project = projectProjectInfo.Project;
            var compilation = await project.GetCompilationAsync();
            if (compilation is null) {
                Console.Error.WriteLine($"ERROR: compilation is null");
                continue;
            }
            var listCSharpDocumentInfo = new List<CSharpDocumentInfo>();

            await ParallelUtility.ForEachAsync(
                project.Documents,
                cancellationToken,
                async (document, cancellationToken) => {
                    var documentInfo = new CSharpDocumentInfo(
                            parserSinkContext.SolutionData.DetailsRoot.CreateWithAbsolutePath(document.FilePath ?? string.Empty)
                            );
                    await ParseDocument(
                        solutionInfo, solution, compilation, document, documentInfo);
                    lock (listCSharpDocumentInfo) {
                        listCSharpDocumentInfo.Add(documentInfo);
                    }
                });
            parserSinkContext.SetListProjectDocumentInfo(projectProjectInfo.ProjectData, listCSharpDocumentInfo);
        }

        // foreach (var project in lstRelevantProject) {
        //     foreach (var document in project.Documents) {
        //         System.Console.Out.WriteLine($"{document.Name} - {document.FilePath}");
        //     }
        // }
#if false
        {
            System.Console.Out.WriteLine("found matches:");
            var lstSourceCodeMatch = sourceCodeMatchByProject.Values.SelectMany(item => item).ToList();
            foreach (var sourceCodeMatch in lstSourceCodeMatch) {
                // if(sourceCodeMatch is SourceCodeMatchCS sourceCodeMatchCS){
                //     System.Console.Out.WriteLine($"{sourceCodeMatch.RelativePath} - {sourceCodeMatch.Line} - {sourceCodeMatch.MatchingText}");
                //     System.Console.Out.WriteLine($"  {sourceCodeMatchCS.Namespace} - {sourceCodeMatchCS.Type} - {sourceCodeMatchCS.Method}");
                // } else {
                //     System.Console.Out.WriteLine($"{sourceCodeMatch.RelativePath} - {sourceCodeMatch.Line} - {sourceCodeMatch.MatchingText}");
                // }
                System.Console.Out.WriteLine(sourceCodeMatch.ToString());
            }
        }
#endif
#if false
        foreach (var project in lstCompilingProjects) {
            var compilation = await project.GetCompilationAsync();
            if (compilation is null) {
                System.Console.Error.WriteLine($"ERROR: compilation is null");
                continue;
            }
            foreach (var syntaxTree in compilation.SyntaxTrees) {
                var root = await syntaxTree.GetRootAsync();
                var model = compilation.GetSemanticModel(syntaxTree);
                var lstMethodDeclarationSyntax = root.DescendantNodesAndSelf().OfType<MethodDeclarationSyntax>().ToList();
                foreach (var methodDeclarationSyntax in lstMethodDeclarationSyntax) {
                    var methodSymbol = model.GetDeclaredSymbol(methodDeclarationSyntax);

                }
                var lstInvocationExpressionSyntax = root.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>().ToList();
                foreach (var invocationExpressionSyntax in lstInvocationExpressionSyntax) {
                    var methodSymbol = model.GetSymbolInfo(invocationExpressionSyntax).Symbol;
                    if (methodSymbol is null) { continue; }
                    //var lstReferenceToMethod = await SymbolFinder.FindReferencesAsync(methodSymbol, solution);
                    //foreach(var referenceToMethod in lstReferenceToMethod){
                        // referenceToMethod.Definition
                        //referenceToMethod.Locations
                    //}
                    
                    var lstSymbolCallerInfo = await SymbolFinder.FindCallersAsync(methodSymbol, solution);
                    foreach(var symbolCallerInfo in lstSymbolCallerInfo){
                        System.Console.Out.WriteLine($"{symbolCallerInfo.CallingSymbol.Name} - {symbolCallerInfo.CallingSymbol.ContainingType.Name} - {symbolCallerInfo.CallingSymbol.ContainingNamespace.Name}");
                        // symbolCallerInfo.CalledSymbol
                        // symbolCallerInfo.CallingSymbol
                        // symbolCallerInfo.IsDirect
                    }
                }
                // lstInvocationExpressionSyntax.ForEach(x => {
                //     var semanticModel=compilation.GetSemanticModel(syntaxTree);
                //     var symbol=semanticModel.GetSymbolInfo(x).Symbol;
                //     if(symbol is IMethodSymbol methodSymbol){
                //         System.Console.Out.WriteLine($"{methodSymbol.Name} - {methodSymbol.ContainingType.Name} - {methodSymbol.ContainingNamespace.Name}");
                //     }
                // });

                var semanticModel = compilation.GetSemanticModel(syntaxTree);
            }
        }
#endif
    }

    private static string[] comments = new string[] { "/// ", "///\t" ,"// ", "//\t", "///", "//" };

    public async Task ParseDocument(
        SolutionData solutionData,
        Solution solution,
        Compilation compilation,
        Document document,
        CSharpDocumentInfo documentInfo
        ) {
        var documentFilePath = document.FilePath;
        if (documentFilePath is null) { return; }

        var sourceText = await document.GetTextAsync();
        if (sourceText is null) {
            Console.Out.WriteLine($"INFO : {document.Name} - {document.FilePath} sourceCode is null");
        } else {
            var sourceCode = sourceText.ToString();
            if (sourceCode.Contains('§')) {
                foreach (Match match in regexSimple.Matches(sourceCode)) {
                    var ownMatchPath = PathData.Create(documentInfo.FileName.RelativePath??string.Empty, 0, String.Empty);
                    var detailData = MatchUtility.ParseMatch(match.Value, ownMatchPath, comments, 0, match.Index);
                    if (detailData is null) { continue; }

                    if (MatchInfoKind.Paragraph == detailData.Kind) { 
                    }
                    var sourceCodeMatch = new SourceCodeData(
                        documentInfo.FileName,
                        DetailData: detailData
                    );
                    var syntaxTree = await document.GetSyntaxTreeAsync();
                    if (syntaxTree is null) {
                        Console.Out.WriteLine($"INFO : {document.Name} - {document.FilePath} syntaxTree is null");

                        (detailData.IsCommand
                            ? documentInfo.GetLstConsumes()
                            : documentInfo.GetLstProvides()
                            ).Add(sourceCodeMatch);
                        continue;
                    }

                    var startIndex = detailData.MatchRange.Start.Value;
                    var textSpan = new Microsoft.CodeAnalysis.Text.TextSpan(
                        startIndex,
                        detailData.MatchLength);
                    //var location = syntaxTree.GetLocation(textSpan);
                    var lineSpan = syntaxTree.GetLineSpan(textSpan);
                    var line = lineSpan.StartLinePosition.Line;
                    detailData = detailData with { Line = line };
                    sourceCodeMatch = sourceCodeMatch with {
                        DetailData = detailData
                    };
                    var syntaxToken = syntaxTree.GetRoot().FindToken(
                        startIndex, true);
                    var semanticModel = compilation.GetSemanticModel(syntaxTree);
                    var syntaxNode = syntaxTree.GetRoot().FindNode(syntaxToken.GetLocation().SourceSpan);
                    if (syntaxNode is null) {
                        Console.Out.WriteLine($"INFO : {document.Name} - {document.FilePath} syntaxNode is null");
                        (detailData.IsCommand
                          ? documentInfo.GetLstConsumes()
                          : documentInfo.GetLstProvides()
                          ).Add(sourceCodeMatch);
                        continue;
                    }
                    ISymbol? symbol = null;
                    symbol = WalkParentUntilDeclaredSymbol(semanticModel, syntaxNode, symbol);

                    if (symbol is null) {
                        Console.Out.WriteLine($"INFO : {document.Name} - {document.FilePath} symbol is null");
                        (detailData.IsCommand
                          ? documentInfo.GetLstConsumes()
                          : documentInfo.GetLstProvides()
                          ).Add(sourceCodeMatch);
                        continue;
                    }

                    var (methodSymbol, fullName, methodName, typeName, namespaceName) = GetNamesToSymbol(symbol);

                    if (fullName is null) {
                        Console.Out.WriteLine($"INFO : {document.Name} - {document.FilePath} fullName is null");
                        (detailData.IsCommand
                          ? documentInfo.GetLstConsumes()
                          : documentInfo.GetLstProvides()
                          ).Add(sourceCodeMatch);
                        continue;
                    }

                    var csContext = new SourceCodeMatchCSContext(
                            //FilePath: sourceCodeMatch.FilePath,
                            //Line: sourceCodeMatch.Match.Line,
                            FullName: fullName,
                            Namespace: namespaceName,
                            Type: typeName,
                            Method: methodName);
                    var matchPath = PathData.Create(documentInfo.FileName.RelativePath ?? string.Empty, line, fullName);
                    detailData = detailData with {
                        MatchPath = matchPath
                    };
                    sourceCodeMatch = sourceCodeMatch with {
                        DetailData = detailData,
                        
                        CSContext = csContext
                    };
                    (detailData.IsCommand
                          ? documentInfo.GetLstConsumes()
                          : documentInfo.GetLstProvides()
                          ).Add(sourceCodeMatch);
//#warning TODO
#if false
                    if (methodSymbol is not null) {
                        var lstSymbolCallerInfo = await SymbolFinder.FindCallersAsync(methodSymbol, solution, CancellationToken.None);
                        System.Console.Out.WriteLine(sourceCodeMatch);
                        foreach (var symbolCallerInfo in lstSymbolCallerInfo) {
                            var callingTypeFQN = symbolCallerInfo.CallingSymbol.ContainingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                            var callingNameFQN = symbolCallerInfo.CallingSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                            var callingFQN = $"{callingTypeFQN}.{callingNameFQN}";
                            var calledTypeFQN = symbolCallerInfo.CalledSymbol.ContainingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                            var calledNameFQN = symbolCallerInfo.CalledSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                            var calledFQN = $"{calledTypeFQN}.{calledNameFQN}";
                            var isDirect = symbolCallerInfo.IsDirect ? "direct" : "indirect";
                            System.Console.Out.WriteLine($"{callingFQN} - {calledFQN} - {isDirect}");
                        }
                        System.Console.Out.WriteLine("");
                        continue;
                    }
#endif
                }
            }
        }
    }

    public static (IMethodSymbol? methodSymbol, string? fullName, string? methodName, string? typeName, string? namespaceName) GetNamesToSymbol(ISymbol? symbol) {
        var lstSymbol = new List<ISymbol>();
        for (var s = symbol; s is not null; s = s.ContainingSymbol) {
            if (s is INamespaceSymbol) {
                lstSymbol.Add(s);
            }
            if (s is ITypeSymbol) {
                lstSymbol.Add(s);
            }
            if (s is IMethodSymbol) {
                lstSymbol.Add(s);
            }
        }
        INamespaceSymbol? namespaceSymbol = null;
        ITypeSymbol? typeSymbol = null;
        IMethodSymbol? methodSymbol = null;

        for (var idx = lstSymbol.Count - 1; idx >= 0; idx--) {
            var s = lstSymbol[idx];
            if (s is INamespaceSymbol ns) {
                namespaceSymbol = ns;
            }
            if (s is ITypeSymbol ts) {
                typeSymbol = ts;
            }
            if (s is IMethodSymbol ms) {
                methodSymbol = ms;
            }
        }

        var fullName = ((ISymbol?)methodSymbol ?? typeSymbol)?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var methodName = methodSymbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var typeName = typeSymbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var namespaceName = namespaceSymbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        return (methodSymbol, fullName, methodName, typeName, namespaceName);
    }

    public static ISymbol? WalkParentUntilDeclaredSymbol(SemanticModel semanticModel, SyntaxNode? syntaxNode, ISymbol? symbol) {
        for (var sn = syntaxNode; symbol is null && sn is not null; sn = sn.Parent) {
            symbol = semanticModel.GetDeclaredSymbol(sn, default);
        }
        return symbol;
    }

    public async Task WriteCSharpDetail(
        WriterContext writerContext,
        CancellationToken cancellationToken) {
        // § todo.md
        Console.WriteLine($"CSharpService.WriteDetail throw new NotImplementedException();");
        await Task.CompletedTask;
        
    }
}

public record CSharpContext(
    MSBuildWorkspace Workspace,
    Solution Solution,
    List<RoslynProjectProjectData> lstRelevantProjectProjectInfo
    ) : System.IDisposable {
    public void Dispose() {
        Workspace.Dispose();
    }
};