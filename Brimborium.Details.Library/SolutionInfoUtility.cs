namespace Brimborium.Details;

public static class SolutionInfoUtility {
    public static SolutionInfoPersitence LoadSolutionInfo(
        IConfiguration configuration
        ) {
        var solutionInfoConfiguration = new SolutionInfoConfiguration();
        configuration.Bind(solutionInfoConfiguration);
        var solutionInfo = new SolutionInfoPersitence(
            solutionInfoConfiguration.DetailsRoot ?? "",
            solutionInfoConfiguration.SolutionFile ?? "",
            solutionInfoConfiguration.DetailsFolder ?? ""
        );
        solutionInfo.ListMainProjectName.AddRange(
            solutionInfoConfiguration.ListMainProjectName
            );
        solutionInfo.ListMainProjectInfo.AddRange(solutionInfoConfiguration.ListMainProjectInfo);
        solutionInfo.ListProject.AddRange(solutionInfoConfiguration.ListProject);
        return solutionInfo;
    }

#if false
    public static async Task<SolutionInfo> ReadSolutionInfo(
        string detailJsonPath
    ) {
        var detailJsonFullPath = System.IO.Path.GetFullPath(detailJsonPath);
        var detailJsonContent = await System.IO.File.ReadAllTextAsync(detailJsonFullPath);
        var existingSolutionInfo = System.Text.Json.JsonSerializer.Deserialize<SolutionInfo>(
            detailJsonContent
            )
            ?? throw new InvalidOperationException();

        var result = existingSolutionInfo.PostLoad(
            System.IO.Path.GetDirectoryName(detailJsonFullPath)
                ?? throw new InvalidOperationException());
        return result;
    }
#endif
    public static async Task WriteSolutionInfo(string detailJsonPath, SolutionInfo solutionInfo) {
        var detailJsonFullPath = System.IO.Path.GetFullPath(detailJsonPath);
        var externalSolutionInfo = solutionInfo.PreSave(detailJsonFullPath);

        await System.IO.File.WriteAllTextAsync(
            detailJsonPath,
            System.Text.Json.JsonSerializer.Serialize(
                externalSolutionInfo,
                new System.Text.Json.JsonSerializerOptions() { WriteIndented = true }));
    }
}