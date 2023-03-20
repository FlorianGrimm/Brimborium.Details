using Meziantou.Extensions.Logging.Xunit;

using Xunit.Abstractions;

namespace Brimborium.Details.Parse;

public class CSharpServiceTests {

    private static string getLocation([CallerFilePath] string? callerFilePath = default) {
        while (!string.IsNullOrEmpty(callerFilePath)
            && System.IO.Path.GetFileName(callerFilePath) != "Brimborium.Details") {
            callerFilePath = System.IO.Path.GetDirectoryName(callerFilePath);
        }
        return callerFilePath ?? string.Empty;
    }

    private static Brimborium.Details.Parse.SolutionData createSolutionData() {
        FileName detailsRoot = FileName.FromAbsolutePath(getLocation());
        FileName solutionFile = detailsRoot.CreateWithRelativePath("Brimborium.Details.sln");
        FileName detailsFolder = detailsRoot.CreateWithRelativePath("details");

        var result = new Brimborium.Details.Parse.SolutionData(detailsRoot, solutionFile, detailsFolder);
        return result;
    }

    private readonly ITestOutputHelper _TestOutputHelper;

    public CSharpServiceTests(ITestOutputHelper testOutputHelper) {
        this._TestOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Test001_SolutionData() {
        var solutionData = createSolutionData();
        Assert.NotNull(solutionData);
        Assert.Equal(true, System.IO.Directory.Exists(solutionData.DetailsRoot.AbsolutePath ?? throw new InvalidOperationException()));
        Assert.Equal(true, System.IO.File.Exists(solutionData.SolutionFile.AbsolutePath ?? throw new InvalidOperationException()));
        Assert.Equal(true, System.IO.Directory.Exists(solutionData.DetailsFolder.AbsolutePath ?? throw new InvalidOperationException()));

    }

    [Fact]
    public async Task Test002_CSharpService_PrepareSolutionCSharp() {
        var solutionData = createSolutionData();
        Assert.NotNull(solutionData);

        /*
        var configurationBuilder = new Microsoft.Extensions.Configuration.ConfigurationBuilder();
        configurationBuilder.AddEnvironmentVariables();
        var configuration = configurationBuilder.Build();
        var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        serviceCollection.AddSingleton<IConfiguration>(configuration);
        serviceCollection.AddLogging(loggingBuilder => {
            loggingBuilder.Services.AddSingleton<ILoggerProvider>(serviceProvider => new XUnitLoggerProvider(this._TestOutputHelper));
        });
        serviceCollection.AddServicesWithRegistrator(
                a => {
                    a.FromDependencyContext(
                        Microsoft.Extensions.DependencyModel.DependencyContext.Default ?? throw new Exception("DependencyContext.Default is null"),
                        (assName) => (assName.Name is string name) && name.StartsWith("Brimborium.")
                    )
                    .AddClasses()
                    .UsingAttributes();
                });
        serviceCollection
        */
        var sut = new CSharpService(XUnitLogger.CreateLogger<CSharpService>(this._TestOutputHelper));
        var projectRepository = new ProjectRepository(solutionData);
        var projectDocumentRepository = new ProjectDocumentRepository(solutionData);
        var documentRepository = new DocumentRepository(solutionData);

        var detailsRepository = new RootRepository(solutionData, projectRepository, projectDocumentRepository, documentRepository);
        var watchServiceConfigurator = DummyWatchServiceConfigurator.Instance;
        IParserSinkContext parserSinkContext = new ParserSinkContext(detailsRepository, solutionData, watchServiceConfigurator);
        using var act = await sut.PrepareSolutionCSharp(parserSinkContext, CancellationToken.None);
        Assert.NotNull(act);
        
        Assert.True(0 < act.Solution.Projects.Count());
        var thisCSProj = act.Solution.Projects.FirstOrDefault(prj => (prj.FilePath is string filePath) && (filePath.EndsWith("Brimborium.Details.Library.Tests.csproj")));
        Assert.NotNull(thisCSProj);

        Assert.True(0 < act.lstRelevantProjectProjectInfo.Count);
        var thisPPI = act.lstRelevantProjectProjectInfo.FirstOrDefault(ppi => ppi.ProjectData.Name.Equals("Brimborium.Details.Library.Tests", StringComparison.OrdinalIgnoreCase));
        Assert.NotNull(thisPPI);

        Assert.True(thisPPI.Project.HasDocuments);
    }

    //[Fact]
    //public async void Test003_CSharpService_ParseDocument() {
    //    /*
    //    sut.ParseDocument
    //        SolutionData solutionData,
    //    Solution solution,
    //    Compilation compilation,
    //    Document document,
    //    CSharpDocumentInfo documentInfo
    //        */
    //}
    //[Fact]
    //public void Test010_CSharpService_ParseDocument() {
    //    //var sut = new CSharpService();
    //    createSolutionData();
    //    /*
    //    sut.ParseDocument
    //        SolutionData solutionData,
    //    Solution solution,
    //    Compilation compilation,
    //    Document document,
    //    CSharpDocumentInfo documentInfo
    //        */
    //}

}
