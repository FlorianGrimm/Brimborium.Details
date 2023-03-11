using Brimborium.Details.Utility;

using Microsoft.CodeAnalysis;

namespace Brimborium.Details.Parse;

[Singleton]
public class TypeScriptService {
    private static Regex regexSimple = new Regex("//[ \t]*§([^\\r\\n]+)");
    /*
    https://github.com/dsherret/ts-morph/tree/latest/packages/ts-morph
    https://www.jameslmilner.com/posts/ts-ast-and-ts-morph-intro/

    https://github.com/sebastienros/jint
    https://satellytes.com/blog/post/typescript-ast-type-checker/
    https://github.com/georgiee/typescript-type-checker-beyond-ast

    https://basarat.gitbooks.io/typescript/content/docs/compiler/program.html
    https://basarat.gitbooks.io/typescript/content/docs/compiler/checker.html
    */
    private readonly IFileSystem _FileSystem;

    public TypeScriptService(
        IFileSystem fileSystem
        ) {
        this._FileSystem = fileSystem;
    }

    public async Task ParseTypeScript(
        ParserSinkContext detailContext,
        CancellationToken cancellationToken) {
        var lstTypeSciptProjectInfo = detailContext.SolutionData.ListMainProjectInfo
            .Where(item => item.Language == "TypeScript")
            .ToList();

        await ParallelUtility.ForEachAsync(
            lstTypeSciptProjectInfo,
            cancellationToken,
            async (typescriptProject, cancellationToken) => {
                var lstDocumentInfo = await this.ParseTypeScriptProject(typescriptProject, cancellationToken);
                detailContext.AddTypescriptProject(typescriptProject, lstDocumentInfo);
            });
    }

    public async Task<List<TypescriptDocumentInfo>> ParseTypeScriptProject(ProjectData typescriptProject, CancellationToken cancellationToken) {
        var result = new List<TypescriptDocumentInfo>();

        Console.WriteLine($"typescriptProject {typescriptProject.FolderPath}");
        var lstTsFile = this._FileSystem.EnumerateFiles(typescriptProject.FolderPath, "*.ts", SearchOption.AllDirectories);
        //var htmlFiles = System.IO.Directory.EnumerateFiles(typescriptProject.FolderPath, "*.html", System.IO.SearchOption.AllDirectories)

        foreach (var tsFile in lstTsFile) {
            // var fi = new System.IO.FileInfo(tsFile);
            // fi.LastWriteTimeUtc
            var contentText = await this._FileSystem.ReadAllTextAsync(tsFile, Encoding.UTF8, cancellationToken);
            var lstSourceCodeMatch = this.ParseTypeScriptDocument(tsFile, contentText);
            if (lstSourceCodeMatch is not null && lstSourceCodeMatch.Count > 0) {
                var documentInfo = new TypescriptDocumentInfo(tsFile) {
                    ListProvides = lstSourceCodeMatch
                };
                result.Add(documentInfo);
            }

            //for (var indexLine = 0; indexLine < contentLines.Length; indexLine++) {
            //    var lineText = contentLines[indexLine];
            //    if (lineText.Contains('§')) {
            //        var pos = lineText.IndexOf("//");
            //        if (pos >= 0) {
            //            lineText = lineText.Substring(pos + 2);
            //        }
            //        var match = MatchUtility.parseMatch(lineText, indexLine+1, 0);
            //        if (match is not null) {
            //            var sourceCodeMatch = new SourceCodeMatch(
            //                    FilePath: tsFile,
            //                    Index: 0,
            //                    Line: indexLine + 1,
            //                    Match: match
            //                ); ;
            //        }
            //    }
            //}
        }
        return result;
    }

    private List<SourceCodeData>? ParseTypeScriptDocument(FileName tsFile, string sourceCode) {
        List<SourceCodeData>? result = null;
        if (sourceCode.Contains('§')) {
            var ownMatchPath = PathData.Create(tsFile.RelativePath!, string.Empty);
            foreach (Match match in regexSimple.Matches(sourceCode)) {
                var matchInfo = MatchUtility.parseMatch(match.Value, ownMatchPath, null, 0, match.Index);
                if (matchInfo is null) { continue; }

                if (result is null) {
                    result = new List<SourceCodeData>();
                }
            }
            return result;
        }
        return result;
    }
}
