namespace Brimborium.Details.Cfg;

public class SolutionInfoConfiguration {
    public SolutionInfoConfiguration() {
        this.ListMainProjectName = new List<string>();
        this.ListMainProjectInfo = new List<ProjectInfoPersitence>();
        this.ListProject = new List<ProjectInfoPersitence>();
    }
    public string? DetailsRoot { get; set; }
    public string? SolutionFile { get; set; }
    public string? DetailsFolder { get; set; }
    public List<string> ListMainProjectName { get; set; }
    public List<ProjectInfoPersitence> ListMainProjectInfo { get; set; }
    public List<ProjectInfoPersitence> ListProject { get; set; }
}
