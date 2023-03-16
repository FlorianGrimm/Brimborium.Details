namespace Brimborium.Details.Parse;

[System.Text.Json.Serialization.JsonDerivedType(typeof(DetailData), "DetailData")]
public record DetailData(
    MatchInfoKind Kind,
    PathData MatchPath,
    [property: JsonIgnore] Range MatchRange,
    PathData Path,
    string Command,
    string Comment,
    int Line
) {
    public bool IsCommand => !string.IsNullOrEmpty(this.Command);

    public int MatchLength {
        get {
            return this.MatchRange.End.Value - this.MatchRange.Start.Value;
        }
    }
}
