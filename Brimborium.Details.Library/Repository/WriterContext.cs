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

    public List<ProjectDocumentInfo> GetAllProjectDocumentInfoProjectRootRelative()
        => this._RootRepositorySnapshot.ProjectDocumentRepository.GetAllProjectDocumentInfoProjectRootRelative();

    public List<ProjectDocumentInfo> GetAllProjectDocumentInfoProjectProjectRelative()
        => this._RootRepositorySnapshot.ProjectDocumentRepository.GetAllProjectDocumentInfoProjectProjectRelative();

    public List<ProjectDocumentInfo> GetAllProjectDocumentInfoRootRelative()
        => this._RootRepositorySnapshot.ProjectDocumentRepository.GetAllProjectDocumentInfoRootRelative();

    public List<ProjectDocumentInfo> GetAllProjectDocumentInfoProjectRelative()
        => this._RootRepositorySnapshot.ProjectDocumentRepository.GetAllProjectDocumentInfoProjectRelative();


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
        var searchPathProjectDocumentInfo = this.FindProjectDocumentInfo(searchPath);
        if (!searchPathProjectDocumentInfo.HasValue) { return result; }
        var searchforProjectDocumentInfo = searchPathProjectDocumentInfo.Value;

        foreach (var item in this.GetListProvides()) {
            var matchProjectDocumentInfo = this.FindProjectDocumentInfo(item.SourceCodeMatch.DetailData.Path);
            if (!matchProjectDocumentInfo.HasValue) { continue; }
            if (matchProjectDocumentInfo.Value.DocumentFilePathRootRelative.Equals(
                searchforProjectDocumentInfo.DocumentFilePathRootRelative)) {
                if (searchPath.IsContentPathEqual(item.SourceCodeMatch.DetailData.Path)){
                    result.Add(item);
                }
            }


            //var (itemFileName, itemDocumentInfo) = this.FindDocumentInfo(item.SourceCodeMatch.DetailData.Path);
            //if (itemFileName is null || itemDocumentInfo is null) { continue; }
            //    if (itemFileName.Equals(searchPathFileName)) {
            //        if (searchPath.IsContentPathEqual(item.SourceCodeMatch.DetailData.Path)) {
            //            result.Add(item);
            //        }
            //    }
        }
        return result;
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

    private static int ComparerDocumentFilePathRootRelative(ProjectDocumentInfo projectDocumentInfo, string match) {
        return string.Compare(
             projectDocumentInfo.DocumentFilePathRootRelative.RelativePath,
             match,
             StringComparison.OrdinalIgnoreCase);

    }

    private static int ComparerDocumentFilePathProjectRelative(ProjectDocumentInfo projectDocumentInfo, string match) {
        return string.Compare(
             projectDocumentInfo.DocumentFilePathProjectRelative.RelativePath,
             match,
             StringComparison.OrdinalIgnoreCase);

    }

    public ProjectDocumentInfo? FindProjectDocumentInfo(PathData path) {
        return this.FindProjectDocumentInfo(path.FilePath);
    }

    public ProjectDocumentInfo? FindProjectDocumentInfo(string filePath) {
        var resultDetailsRoot = this._SolutionData.DetailsRoot.CreateWithRelativePath(filePath);
        var resultDetailsFolder = this._SolutionData.DetailsFolder.CreateWithRelativePath(filePath);
        {
            var listProjectDocumentInfo = this.GetAllProjectDocumentInfoRootRelative();
            var index = listProjectDocumentInfo.BinarySearch(resultDetailsRoot.RelativePath!, ComparerDocumentFilePathRootRelative);
            if (index >= 0) {
                return listProjectDocumentInfo[index];
            }
        }
        {
            var listProjectDocumentInfo = this.GetAllProjectDocumentInfoProjectRelative();
            var index = listProjectDocumentInfo.BinarySearch(resultDetailsFolder.RelativePath!, ComparerDocumentFilePathProjectRelative);
            if (index >= 0) {
                return listProjectDocumentInfo[index];
            }
        }
        /*
        {
            var listProjectDocumentInfo = this.GetAllProjectDocumentInfoProjectRootRelative();

            foreach (var projectDocumentInfo in listProjectDocumentInfo) {

                if (path.FilePath.Equals(
                    projectDocumentInfo.DocumentFilePathRootRelative.RelativePath, StringComparison.OrdinalIgnoreCase)) {
                    return projectDocumentInfo;
                }

                if (path.FilePath.Equals(
                    projectDocumentInfo.DocumentFilePathProjectRelative.RelativePath, StringComparison.OrdinalIgnoreCase)) {
                    return projectDocumentInfo;
                }
            }
        }
        */
        return null;
    }

}


public readonly record struct DocumentInfoSourceCodeMatch(
    IDocumentInfo DocumentInfo,
    SourceCodeData SourceCodeMatch);

