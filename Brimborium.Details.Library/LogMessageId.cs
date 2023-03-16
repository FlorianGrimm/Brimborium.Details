namespace Brimborium.Details;

public enum LogMessageId: int {
    Stat = 1000,
    SolutionLoadStart,
    SolutionLoadSuccess,
    SolutionLoadFailed,
    SolutionWorkspaceFailed,
    ProjectLoadSuccess,
    ProjectLoadFailed,
}