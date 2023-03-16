namespace Brimborium.Details.Repository;

public class WriterContext {
    private readonly RootRepositorySnapshot _RootRepositorySnapshot;
    private readonly SolutionData _SolutionData;

    public WriterContext(RootRepositorySnapshot rootRepositorySnapshot, SolutionData solutionData) {
        this._RootRepositorySnapshot = rootRepositorySnapshot;
        this._SolutionData = solutionData;
    }

    [System.Text.Json.Serialization.JsonInclude]
    public FileName DetailsFolder => this._SolutionData.DetailsFolder;

    [System.Text.Json.Serialization.JsonInclude]
    public List<ProjectData> ListProjectDatas => this._RootRepositorySnapshot.ProjectRepository.GetProjectDatas();

    [System.Text.Json.Serialization.JsonInclude]
    public List<ProjectDocumentInfo> ProjectDocumentInfo => this._RootRepositorySnapshot.ProjectDocumentRepository.GetAllProjectDocumentInfoProjectRootRelative();

    [System.Text.Json.Serialization.JsonInclude]
    public List<IDocumentInfo> ListDocumentInfo
        => this._RootRepositorySnapshot.DocumentRepository.GetAllDocumentInfo();

    [System.Text.Json.Serialization.JsonInclude]
    public List<DocumentInfoSourceCodeMatch> ListProvide
        => this._RootRepositorySnapshot.DocumentRepository.GetAllProvide();

    [System.Text.Json.Serialization.JsonInclude]
    public List<DocumentInfoSourceCodeMatch> ListConsume 
        => this._RootRepositorySnapshot.DocumentRepository.GetAllConsume();

    public List<MarkdownDocumentInfo> GetAllMarkdownDocumentInfo()
        => this._RootRepositorySnapshot.DocumentRepository.GetAllMarkdownDocumentInfo();

    public List<IDocumentInfo> GetAllDocumentInfo()
        => this._RootRepositorySnapshot.DocumentRepository.GetAllDocumentInfo();

    public List<ProjectDocumentInfo> GetAllProjectDocumentInfoAbsolute()
        => this._RootRepositorySnapshot.ProjectDocumentRepository.GetAllProjectDocumentInfoAbsolute();

    public List<ProjectDocumentInfo> GetAllProjectDocumentInfoProjectRootRelative()
        => this._RootRepositorySnapshot.ProjectDocumentRepository.GetAllProjectDocumentInfoProjectRootRelative();

    public List<ProjectDocumentInfo> GetAllProjectDocumentInfoProjectProjectRelative()
        => this._RootRepositorySnapshot.ProjectDocumentRepository.GetAllProjectDocumentInfoProjectProjectRelative();

    public List<ProjectDocumentInfo> GetAllProjectDocumentInfoRootRelative()
        => this._RootRepositorySnapshot.ProjectDocumentRepository.GetAllProjectDocumentInfoRootRelative();

    public List<ProjectDocumentInfo> GetAllProjectDocumentInfoProjectRelative()
        => this._RootRepositorySnapshot.ProjectDocumentRepository.GetAllProjectDocumentInfoProjectRelative();


    public List<DocumentInfoSourceCodeMatch> GetAllConsumes() {
        return this._RootRepositorySnapshot.DocumentRepository.GetAllConsume();
    }

    public List<DocumentInfoSourceCodeMatch> GetListProvides() {
        return this._RootRepositorySnapshot.DocumentRepository.GetAllProvide();
    }

    public List<DocumentInfoSourceCodeMatch> QueryPath(
        PathData searchPath
        ) {
        var result = new List<DocumentInfoSourceCodeMatch>();
        var searchPathProjectDocumentInfo = this.FindProjectDocumentInfo(searchPath);
        if (!searchPathProjectDocumentInfo.HasValue) { return result; }
        var searchforProjectDocumentInfo = searchPathProjectDocumentInfo.Value;

        foreach (var item in this.GetListProvides()) {
            if (MatchInfoKind.Paragraph == item.SourceCodeMatch.DetailData.Kind) {
            var detailDataPath = item.SourceCodeMatch.DetailData.Path;
            var itemPathFilePath = detailDataPath.FilePath;
                if (string.IsNullOrEmpty(itemPathFilePath)
                    || string.Equals(
                        itemPathFilePath,
                        searchforProjectDocumentInfo.DocumentFilePathProjectRelative.RelativePath,
                        StringComparison.OrdinalIgnoreCase
                        )
                    || string.Equals(
                        itemPathFilePath,
                        searchforProjectDocumentInfo.DocumentFilePathRootRelative.RelativePath,
                        StringComparison.OrdinalIgnoreCase
                        )
                    ) {
                if (searchPath.IsContentPathEqual(detailDataPath)) {
                        result.Add(item);
                    }
                } else {
                    continue;
                }
            }
            //var matchProjectDocumentInfo = this.FindProjectDocumentInfo(item.SourceCodeMatch.DetailData.Path);
            //if (!matchProjectDocumentInfo.HasValue) { continue; }
            //if (matchProjectDocumentInfo.Value.DocumentFilePathRootRelative.Equals(
            //    searchforProjectDocumentInfo.DocumentFilePathRootRelative)) {
            //    if (searchPath.IsContentPathEqual(item.SourceCodeMatch.DetailData.Path)){
            //        result.Add(item);
            //    }
            //}


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

    //private static int ComparerDocumentFilePathRootRelative(
    //    ProjectDocumentInfo projectDocumentInfo,
    //    string match) {
    //    return string.Compare(
    //         projectDocumentInfo.DocumentFilePathRootRelative.AbsolutePath,
    //         match,
    //         StringComparison.OrdinalIgnoreCase);

    //}

    //private static int ComparerDocumentFilePathProjectRelative(
    //    ProjectDocumentInfo projectDocumentInfo, 
    //    string match) {
    //    return string.Compare(
    //         projectDocumentInfo.DocumentFilePathProjectRelative.AbsolutePath,
    //         match,
    //         StringComparison.OrdinalIgnoreCase);

    //}

    private static int ComparerDocumentFilePathAbsolutePath(
       ProjectDocumentInfo projectDocumentInfo,
       string match) {
        return string.Compare(
             projectDocumentInfo.DocumentFilePathProjectRelative.AbsolutePath,
             match,
             StringComparison.OrdinalIgnoreCase);

    }

    public ProjectDocumentInfo? FindProjectDocumentInfo(PathData path) {
        return this.FindProjectDocumentInfo(path.FilePath);
    }
    
    public ProjectDocumentInfo? FindProjectDocumentInfo(string filePath) {
        var resultDetailsRoot = this._SolutionData.DetailsRoot.CreateWithRelativePath(filePath);
        var resultDetailsFolder = this._SolutionData.DetailsFolder.CreateWithRelativePath(filePath);
        if (resultDetailsRoot.AbsolutePath is not null) {
            var listProjectDocumentInfo = this.GetAllProjectDocumentInfoAbsolute();
            var index = listProjectDocumentInfo.BinarySearch(resultDetailsRoot.AbsolutePath, ComparerDocumentFilePathAbsolutePath);
            if (index >= 0) {
                return listProjectDocumentInfo[index];
            }
        }
        if (resultDetailsFolder.AbsolutePath is not null) {
            var listProjectDocumentInfo = this.GetAllProjectDocumentInfoAbsolute();
            var index = listProjectDocumentInfo.BinarySearch(resultDetailsFolder.AbsolutePath, ComparerDocumentFilePathAbsolutePath);
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
