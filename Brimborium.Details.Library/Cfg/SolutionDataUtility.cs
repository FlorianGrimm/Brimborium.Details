namespace Brimborium.Details.Cfg;

public static class SolutionDataUtility {
    public static SolutionInfoPersitence LoadSolutionData(
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
    public static async Task WriteSolutionInfo(string detailJsonPath, SolutionData solutionData) {
        var detailJsonFullPath = Path.GetFullPath(detailJsonPath);
        var externalSolutionInfo = solutionData.PreSave(detailJsonFullPath);

        await File.WriteAllTextAsync(
            detailJsonPath,
            JsonSerializer.Serialize(
                externalSolutionInfo,
                new JsonSerializerOptions() { WriteIndented = true }));
    }
}