namespace Brimborium.Details.Parse;

[System.Text.Json.Serialization.JsonDerivedType(typeof(SourceCodeMatchCSContext), "SourceCodeMatchCSContext")]
public record SourceCodeMatchCSContext(
    //FileName FilePath,
    //int Line,
    string FullName,
    string? Namespace,
    string? Type,
    string? Method
);
