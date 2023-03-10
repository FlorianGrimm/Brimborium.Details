namespace Brimborium.Details.Parse;

public record SourceCodeData(
    FileName FilePath,
    //int Index,
    //int Line,
    DetailData DetailData,
    SourceCodeMatchCSContext? CSContext = null
) {
}
