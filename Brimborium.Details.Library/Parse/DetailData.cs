namespace Brimborium.Details.Parse;

public record DetailData(
    MatchInfoKind Kind,
    PathData MatchPath,
    Range MatchRange,
    PathData Path,
    string Command,
    PathData Anchor,
    string Comment,
    int Line = 0
) {
    public bool IsCommand => !string.IsNullOrEmpty(this.Command);

    public int MatchLength {
        get {
            return this.MatchRange.End.Value - this.MatchRange.Start.Value;
        }
    }
}
