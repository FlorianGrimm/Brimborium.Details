namespace Brimborium.Details;

[Brimborium.Registrator.Singleton]
public class TypeScriptService {
    /*
    https://github.com/dsherret/ts-morph/tree/latest/packages/ts-morph
    https://www.jameslmilner.com/posts/ts-ast-and-ts-morph-intro/

    https://github.com/sebastienros/jint
    https://satellytes.com/blog/post/typescript-ast-type-checker/
    https://github.com/georgiee/typescript-type-checker-beyond-ast

    https://basarat.gitbooks.io/typescript/content/docs/compiler/program.html
    https://basarat.gitbooks.io/typescript/content/docs/compiler/checker.html
    */
    public readonly SolutionInfo SolutionInfo;
    private readonly Dictionary<ProjectInfo, List<SourceCodeMatch>> _SourceCodeMatchByProject;

    public TypeScriptService(SolutionInfo solutionInfo) {
        this.SolutionInfo = solutionInfo;
        this._SourceCodeMatchByProject = new Dictionary<ProjectInfo, List<SourceCodeMatch>>();
    }

    public async Task ParseTypeScript(CancellationToken cancellationToken) {
        var lstTypeSciptProjectInfo = SolutionInfo.ListMainProjectInfo
            .Where(item => item.Language == "TypeScript")
            .ToList();

        await System.Threading.Tasks.Parallel.ForEachAsync(
            lstTypeSciptProjectInfo,
            cancellationToken,
            async (typescriptProject, cancellationToken) => {
                var lstMatchInfo = await this.ParseTypeScriptProject(typescriptProject, cancellationToken);
                lock (this._SourceCodeMatchByProject) {
                    this._SourceCodeMatchByProject[typescriptProject] = lstMatchInfo;
                }
            });
    }

    public async Task<List<SourceCodeMatch>> ParseTypeScriptProject(ProjectInfo typescriptProject, CancellationToken cancellationToken) {
        var lstMatchInfo = new List<SourceCodeMatch>();

        Console.WriteLine($"typescriptProject {typescriptProject.FolderPath}");
        var lstTsFile = System.IO.Directory.EnumerateFiles(typescriptProject.FolderPath, "*.ts", System.IO.SearchOption.AllDirectories);
        //var htmlFiles = System.IO.Directory.EnumerateFiles(typescriptProject.FolderPath, "*.html", System.IO.SearchOption.AllDirectories)

        foreach (var tsFile in lstTsFile) {
            // var fi = new System.IO.FileInfo(tsFile);
            // fi.LastWriteTimeUtc            
            var content = await System.IO.File.ReadAllLinesAsync(tsFile);
            for (var idx = 0; idx < content.Length; idx++) {
                var line = content[idx];
                if (line.Contains('§')) {
                    var pos = line.IndexOf("//");
                    if (pos >= 0) {
                        line = line.Substring(pos + 2);
                    }
                    var match = MatchUtility.parseMatchIfMatches(line);
                    if (match is not null) {
                        var sourceCodeMatch = new SourceCodeMatch(
                                FilePath: this.getRelativePath(typescriptProject, tsFile),
                                Index: 0,
                                Line: idx + 1,
                                Match: match
                            );
                    }
                }
            }
        }
        return lstMatchInfo;
    }

    private string getRelativePath(ProjectInfo typescriptProject, string tsFile) {
        return System.IO.Path.GetRelativePath(typescriptProject.FolderPath, tsFile);
    }
}
