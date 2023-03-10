namespace Brimborium.Details.Parse;

public record SourceCodeMatchCSContext(
    //FileName FilePath,
    //int Line,
    string FullName,
    string? Namespace,
    string? Type,
    string? Method
);
