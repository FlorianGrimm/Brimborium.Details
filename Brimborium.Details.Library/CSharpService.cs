namespace Brimborium.Details;

[Brimborium.Registrator.Transient]
public class CSharpService {
    private static bool _RegisterDefaultsIsCalled = false;
    public static void RegisterDefaults() {
        if (!_RegisterDefaultsIsCalled) {
            Microsoft.Build.Locator.MSBuildLocator.RegisterDefaults();
            _RegisterDefaultsIsCalled = true;
        }
    }

    private static Regex regexSimple = new System.Text.RegularExpressions.Regex("//[ \t]*§([^\\r\\n]+)");

    public CSharpService() {
    }


    public async Task<CSharpContext?> PrepareSolutionCSharp(
        DetailContext detailContext,
        CancellationToken cancellationToken) {
        SolutionInfo solutionInfo = detailContext.SolutionInfo;
        var solutionFile = solutionInfo.SolutionFile.AbsolutePath;
        if (solutionFile is null) { return null; }
        Console.WriteLine($"Loading solution '{solutionFile}'");
        RegisterDefaults();

        using var workspace = MSBuildWorkspace.Create();
        workspace.WorkspaceFailed += (sender, args) => Console.WriteLine(args.Diagnostic.Message);
        var solution = await workspace.OpenSolutionAsync(solutionFile);

        /*
        foreach (var project in solution.Projects) {
            if (project is not null) {
                System.Console.Out.WriteLine($"{project.Name} - {project.Id}");
            }
        }
        */

        var lstMainProject = new List<Microsoft.CodeAnalysis.Project>();
        var queue = new Queue<Microsoft.CodeAnalysis.Project>();
        foreach (var projectName in solutionInfo.ListMainProjectName) {
            var project = solution.Projects.Where(project => project.Name == projectName).FirstOrDefault();
            if (project is null) {
                System.Console.Out.WriteLine($"WARNING: ListMainProjectName {projectName} - not found");
                continue;
            } else {
                System.Console.Out.WriteLine($"INFO: ListMainProjectName {project.Name} - {project.Id}");
                lstMainProject.Add(project);
                queue.Enqueue(project);
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
        var filterPath = solutionInfo.DetailsRoot.AbsolutePath ?? string.Empty;
        System.Console.Out.WriteLine($"INFO: filterPath {filterPath}");

        var projectDependencyGraph = solution.GetProjectDependencyGraph();
        var lstRelevantProject = new List<Microsoft.CodeAnalysis.Project>();
        var hsRelevantProject = new HashSet<Microsoft.CodeAnalysis.ProjectId>();
        while (queue.Count > 0) {
            var project = queue.Dequeue();
            if (hsRelevantProject.Contains(project.Id)) {
                continue;
            }
            hsRelevantProject.Add(project.Id);
            if (project.FilePath is null) {
                System.Console.Out.WriteLine($"WARNING: ProjectDependency {project.Name} - {project.Id} - no file path");
                continue;
            }
            if (project.FilePath.EndsWith("Tests.csproj", StringComparison.InvariantCultureIgnoreCase)
                || project.FilePath.EndsWith("Test.csproj", StringComparison.InvariantCultureIgnoreCase)) {
                System.Console.Out.WriteLine($"INFO: ProjectDependency {project.Name} - {project.Id} - skipped");
                continue;
            }
            if (!project.FilePath.StartsWith(filterPath, StringComparison.InvariantCultureIgnoreCase)) {
                System.Console.Out.WriteLine($"INFO: ProjectDependency {project.Name} - {project.Id} - skipped");
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
        var lstRelevantProjectProjectInfo = new List<ProjectProjectInfo>();
        foreach (var project in lstRelevantProject) {
            if (project.FilePath is null) {
                System.Console.Error.WriteLine($"ERROR: project.FilePath is null");
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
            
            lstRelevantProjectProjectInfo.Add(
                new ProjectProjectInfo(
                    project,
                    detailContext.AddCSharpProject(
                        project.FilePath,
                        project.Name,
                        project.Id,
                        lstDocument)));
        }
        var csharpContext = new CSharpContext(
            solution,
            lstRelevantProjectProjectInfo);
        return csharpContext;
    }
    
    public async Task ParseCSharp(
        DetailContext detailContext,
        CSharpContext csharpContext,
        CancellationToken cancellationToken
        ) {
        var solutionInfo = detailContext.SolutionInfo;
        var solution = csharpContext.Solution;
        var lstCompilingProjects = new List<ProjectProjectInfo>();

        await ParallelUtility.ForEachAsync(
            csharpContext.lstRelevantProjectProjectInfo,
            cancellationToken,
            async (projectProjectInfo, cancellationToken) => {
                var project = projectProjectInfo.Project;
                var compilation = await project.GetCompilationAsync();
                if (compilation is null) {
                    System.Console.Out.WriteLine($"INFO: Compiles: {project.Name} - {project.Id} compilation is null");
                } else {
                    System.Console.Out.WriteLine($"INFO: Compiles: {project.Name} - {project.Id} - OK");
                    lock (lstCompilingProjects) {
                        lstCompilingProjects.Add(projectProjectInfo);
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
                System.Console.Error.WriteLine($"ERROR: compilation is null");
                continue;
            }
            var lstCSharpDocumentInfo = new List<CSharpDocumentInfo>();

            await ParallelUtility.ForEachAsync(
                project.Documents,
                cancellationToken,
                async (document, cancellationToken) => {
                    var documentInfo = new CSharpDocumentInfo(
                            detailContext.SolutionInfo.DetailsRoot.CreateWithAbsolutePath(document.FilePath ?? string.Empty)
                            );
                    await parseDocument(
                        solutionInfo, solution, compilation, document, documentInfo);
                    lock (lstCSharpDocumentInfo) {
                        lstCSharpDocumentInfo.Add(documentInfo);
                    }
                });
            detailContext.AddCSharpDocumentInfo(project.Id, lstCSharpDocumentInfo);
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

    private async Task parseDocument(
        SolutionInfo solutionInfo,
        Solution solution,
        Compilation compilation,
        Document document,
        CSharpDocumentInfo documentInfo
        ) {
        var documentFilePath = document.FilePath;
        if (documentFilePath is null) { return; }

        var sourceText = await document.GetTextAsync();
        if (sourceText is null) {
            System.Console.Out.WriteLine($"INFO : {document.Name} - {document.FilePath} sourceCode is null");
        } else {
            var sourceCode = sourceText.ToString();
            if (sourceCode.Contains('§')) {
                foreach (System.Text.RegularExpressions.Match match in regexSimple.Matches(sourceCode)) {
                    var ownMatchPath = PathInfo.Create(documentInfo.FileName.RelativePath ?? string.Empty, string.Empty);
                    var matchInfo = MatchUtility.parseMatch(match.Value, ownMatchPath, 0, match.Index);
                    if (matchInfo is null) { continue; }

                    var sourceCodeMatch = new SourceCodeMatch(
                        documentInfo.FileName,
                        Match: matchInfo
                    );
                    var syntaxTree = await document.GetSyntaxTreeAsync();
                    if (syntaxTree is null) {
                        System.Console.Out.WriteLine($"INFO : {document.Name} - {document.FilePath} syntaxTree is null");

                        (matchInfo.IsCommand
                            ? documentInfo.GetLstConsumes()
                            : documentInfo.GetLstProvides()
                            ).Add(sourceCodeMatch);
                        continue;
                    }

                    var startIndex = matchInfo.MatchRange.Start.Value;
                    var textSpan = new Microsoft.CodeAnalysis.Text.TextSpan(
                        startIndex,
                        matchInfo.MatchLength);
                    var location = syntaxTree.GetLocation(textSpan);
                    var lineSpan = syntaxTree.GetLineSpan(textSpan);
                    var line = lineSpan.StartLinePosition.Line;
                    matchInfo = matchInfo with { Line = line };
                    sourceCodeMatch = sourceCodeMatch with {
                        Match = matchInfo
                    };
                    var syntaxToken = syntaxTree.GetRoot().FindToken(
                        startIndex, true);
                    var semanticModel = compilation.GetSemanticModel(syntaxTree);
                    var syntaxNode = syntaxTree.GetRoot().FindNode(syntaxToken.GetLocation().SourceSpan);
                    if (syntaxNode is null) {
                        System.Console.Out.WriteLine($"INFO : {document.Name} - {document.FilePath} syntaxNode is null");
                        (matchInfo.IsCommand
                          ? documentInfo.GetLstConsumes()
                          : documentInfo.GetLstProvides()
                          ).Add(sourceCodeMatch);
                        continue;
                    }
                    ISymbol? symbol = null;
                    symbol = WalkParentUntilDeclaredSymbol(semanticModel, syntaxNode, symbol);

                    if (symbol is null) {
                        System.Console.Out.WriteLine($"INFO : {document.Name} - {document.FilePath} symbol is null");
                        (matchInfo.IsCommand
                          ? documentInfo.GetLstConsumes()
                          : documentInfo.GetLstProvides()
                          ).Add(sourceCodeMatch);
                        continue;
                    }

                    var (methodSymbol, fullName, methodName, typeName, namespaceName) = GetNamesToSymbol(symbol);

                    if (fullName is null) {
                        System.Console.Out.WriteLine($"INFO : {document.Name} - {document.FilePath} fullName is null");
                        (matchInfo.IsCommand
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
                    sourceCodeMatch = sourceCodeMatch with {
                        CSContext = csContext
                    };
                    (matchInfo.IsCommand
                          ? documentInfo.GetLstConsumes()
                          : documentInfo.GetLstProvides()
                          ).Add(sourceCodeMatch);
#warning TODO
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

        for (int idx = lstSymbol.Count - 1; idx >= 0; idx--) {
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

        var fullName = ((ISymbol?)methodSymbol ?? (ISymbol?)typeSymbol)?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var methodName = methodSymbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var typeName = typeSymbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var namespaceName = namespaceSymbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        return (methodSymbol, fullName, methodName, typeName, namespaceName);
    }

    public static ISymbol? WalkParentUntilDeclaredSymbol(SemanticModel semanticModel, SyntaxNode? syntaxNode, ISymbol? symbol) {
        for (SyntaxNode? sn = syntaxNode; symbol is null && sn is not null; sn = sn.Parent) {
            symbol = Microsoft.CodeAnalysis.ModelExtensions.GetDeclaredSymbol(semanticModel, sn, default);
        }
        return symbol;
    }

    public async Task WriteDetail(
        DetailContext detailContext,
        CancellationToken cancellationToken) {
        // § todo.md
        Console.WriteLine($"DetailPath {detailContext.SolutionInfo.DetailsFolder}");
        await Task.CompletedTask;
    }
}

public record CSharpContext(
    Solution Solution,
    List<ProjectProjectInfo> lstRelevantProjectProjectInfo
    );