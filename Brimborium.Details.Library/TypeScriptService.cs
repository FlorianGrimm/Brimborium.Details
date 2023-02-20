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

    public TypeScriptService(SolutionInfo solutionInfo) {
        this.SolutionInfo = solutionInfo;
    }

    public async Task ParseTypeScript(CancellationToken cancellationToken) {
        var lstTypeSciptProjectInfo = SolutionInfo.ListMainProjectInfo
            .Where(item => item.Language == "TypeScript")
            .ToList();

        foreach (var typescriptProject in lstTypeSciptProjectInfo) {
            await this.ParseTypeScriptProject(typescriptProject, cancellationToken);
        }
    }

    public async Task ParseTypeScriptProject(ProjectInfo typescriptProject, CancellationToken cancellationToken) {
        Console.WriteLine($"typescriptProject {typescriptProject.FolderPath}");
        await Task.CompletedTask;
    }
}
