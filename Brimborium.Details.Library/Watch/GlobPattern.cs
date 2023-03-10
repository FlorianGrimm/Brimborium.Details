namespace Brimborium.Details.Watch;

public record GlobPattern(
    string Extension,
    string RelativePath,
    Regex? Include,
    Regex? Exclude,
    Func<string, bool>? IsMatch
);
