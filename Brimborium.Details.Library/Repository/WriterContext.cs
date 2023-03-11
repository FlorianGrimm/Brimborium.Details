namespace Brimborium.Details.Repository;

public class WriterContext {
    private readonly RootRepositorySnapshot _RootRepositorySnapshot;
    private readonly SolutionData _SolutionData;

    public WriterContext(RootRepositorySnapshot rootRepositorySnapshot, SolutionData solutionData) {
        this._RootRepositorySnapshot = rootRepositorySnapshot;
        this._SolutionData = solutionData;
    }

    public FileName DetailsFolder => this._SolutionData.DetailsFolder;

    public List<MarkdownDocumentInfo> GetAllMarkdownDocumentInfo()
        => this._RootRepositorySnapshot.DocumentRepository.GetAllMarkdownDocumentInfo();

    public List<IDocumentInfo> GetAllDocumentInfo()
        => this._RootRepositorySnapshot.DocumentRepository.GetAllDocumentInfo();

    public List<ProjectDocumentInfo> GetAllProjectDocumentInfo() {
        return this._RootRepositorySnapshot.ProjectDocumentRepository.GetAllProjectDocumentInfo();
    }


    public List<DocumentInfoSourceCodeMatch> GetAllConsumes() {
        return this._RootRepositorySnapshot.DocumentRepository.GetAllConsumes();
        //var result = new List<ProjectDocumentInfoSourceCodeMatch>();
        //foreach (var projectDocumentInfo in this.GetLstProjectDocumentInfo()) {
        //    if (projectDocumentInfo.DocumentInfo.LstConsumes is null) { continue; }
        //    foreach (var sourceCodeMatch in projectDocumentInfo.DocumentInfo.LstConsumes) {
        //        result.Add(new ProjectDocumentInfoSourceCodeMatch(
        //            projectDocumentInfo.ProjectInfo,
        //            projectDocumentInfo.DocumentInfo,
        //            sourceCodeMatch));
        //    }
        //}
        //return result;
    }

    public List<DocumentInfoSourceCodeMatch> GetListProvides() {
        return this._RootRepositorySnapshot.DocumentRepository.GetAllProvides();
        // this._RootRepositorySnapshot.DocumentRepository.GetAllMarkdownDocumentInfo();
        // var result = new List<ProjectDocumentInfoSourceCodeMatch>();
        // foreach (var projectDocumentInfo in this.GetLstProjectDocumentInfo(cache)) {
        //    if (projectDocumentInfo.DocumentInfo.LstProvides is null) { continue; }
        //    foreach (var sourceCodeMatch in projectDocumentInfo.DocumentInfo.LstProvides) {
        //        result.Add(new ProjectDocumentInfoSourceCodeMatch(
        //            projectDocumentInfo.ProjectInfo,
        //            projectDocumentInfo.DocumentInfo,
        //            sourceCodeMatch));
        //    }
        // }
        // return result;
    }

    public List<DocumentInfoSourceCodeMatch> QueryPath(
        PathData searchPath
        ) {
        var result = new List<DocumentInfoSourceCodeMatch>();
        var (searchPathFileName, searchPathDocumentInfo) = this.FindDocumentInfo(searchPath);
        //if (searchPathDocumentInfo is null) { return result; }
        //foreach (var item in this.GetLstProvides()) {
        //    var (itemFileName, itemDocumentInfo) = this.FindDocumentInfo(item.SourceCodeMatch.DetailData.Path, cache);
        //    if (itemFileName is null || itemDocumentInfo is null) { continue; }
        //    if (itemFileName.Equals(searchPathFileName)) {
        //        if (searchPath.IsContentPathEqual(item.SourceCodeMatch.DetailData.Path)) {
        //            result.Add(item);
        //        }
        //    }
        //}
        //return result;
        throw new NotImplementedException();
    }

    public List<ProjectDocumentInfoSourceCodeMatch> QueryPathChildren(
        PathData searchPath
        ) {
        //var result = new List<ProjectDocumentInfoSourceCodeMatch>();
        //var (searchPathFileName, searchPathDocumentInfo) = this.FindDocumentInfo(searchPath, cache);
        //if (searchPathDocumentInfo is null) { return result; }
        //foreach (var item in this.GetLstProvides(cache)) {
        //    var (itemFileName, itemDocumentInfo) = this.FindDocumentInfo(item.SourceCodeMatch.DetailData.Path, cache);
        //    if (itemFileName is null || itemDocumentInfo is null) { continue; }
        //    if (itemFileName.Equals(searchPathFileName)) {
        //        if (item.SourceCodeMatch.DetailData.Path.IsContentPathParent(searchPath)) {
        //            result.Add(item);
        //        }
        //    }
        //}
        //return result;
        throw new NotImplementedException();
    }

    public IDocumentInfo? FindDocumentInfo(PathData path) {
        var resultDetailsRoot = this._SolutionData.DetailsRoot.CreateWithRelativePath(path.FilePath);
        var resultDetailsFolder = this._SolutionData.DetailsFolder.CreateWithRelativePath(path.FilePath);
        var listAllDocumentInfo = this.GetAllDocumentInfo();
        //var lstProjectDocumentInfo = this.GetLstProjectDocumentInfo(cache);
        //var lstWithRelativePath = new List<FileName>();
        foreach (var projectDocumentInfo in listAllDocumentInfo) {
            var rootRelativePath = projectDocumentInfo.FileName;
            if (path.FilePath.Equals(rootRelativePath.RelativePath, StringComparison.OrdinalIgnoreCase)) {
                return projectDocumentInfo;
            }

            var projectRelativePath = rootRelativePath.GetFileNameProjectRebased(projectDocumentInfo.ProjectInfo);
            if (path.FilePath.Equals(projectRelativePath.RelativePath, StringComparison.OrdinalIgnoreCase)) {
                return projectDocumentInfo;
            }
        }
        return null;
    }

}


public readonly record struct DocumentInfoSourceCodeMatch(
    IDocumentInfo DocumentInfo,
    SourceCodeData SourceCodeMatch);

