namespace Brimborium.Details.Parse;

[System.Text.Json.Serialization.JsonDerivedType(typeof(SourceCodeData), "SourceCodeData")]
public record SourceCodeData(
    FileName FilePath,
    //int Index,
    //int Line,
    DetailData DetailData,
    SourceCodeMatchCSContext? CSContext = null
) {
}
