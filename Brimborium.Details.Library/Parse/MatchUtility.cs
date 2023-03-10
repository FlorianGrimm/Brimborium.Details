namespace Brimborium.Details.Parse;

public static class MatchUtility {
    private static readonly string Paragraph = "§";
    private static readonly string ParagraphGreater = "§>";
    private static readonly string ParagraphHash = "§#";
    private static readonly string DetailsProtocol = "details://";
    private static readonly string DetailsCodeProtocol = "details-code://";
    private static readonly string OpenSquareBrackets = "[";
    private static readonly string CloseSquareBrackets = "]";
    private static readonly string OpenRoundBrackets = "(";
    private static readonly string CloseRoundBrackets = ")";

    public static int IsWhitespaceNorNewLine(char value, int index) {
        if (value == ' ' || value == '\t') {
            return 0;
        }
        if (value == '\r' || value == '\\') {
            return -1;
        }
        return 1;
    }

    public static int IsNotWhitespaceNorNewLine(char value, int index) {
        if (value == ' ' || value == '\t') {
            return 1;
        }
        if (value == '\r' || value == '\\') {
            return -1;
        }
        return 0;
    }

    public static int IsAnyThingButNewLine(char value, int index) {
        if (value == '\r' || value == '\\') {
            return -1;
        }
        return 0;
    }

    public static DetailData? parseMatch(
        StringSlice value,
        PathData ownMatchPath,
        string? optionalPrefix,
        int line, int offset) {
        var startAsOffset = value.Range.Start.Value;

        var text = value.TrimWhile(IsWhitespaceNorNewLine);

        if (optionalPrefix is not null
            && optionalPrefix.ReadWordIfMatches(ref text)) { text = text.TrimWhile(IsWhitespaceNorNewLine); }

        var startAfterOptional = text.Range.Start.Value;

        // §# Anchor Comment
        if (ParagraphHash.ReadWordIfMatches(ref text)) {
            text = text.TrimWhile(IsWhitespaceNorNewLine);

            var anchorSlice = StringSlice.Empty;
            var commentSlice = StringSlice.Empty;
            (anchorSlice, text) = text.SplitIntoWhile(IsNotWhitespaceNorNewLine);
            if (anchorSlice.IsNullOrEmpty()) { return default; }

            text = text.TrimWhile(IsWhitespaceNorNewLine);

            (commentSlice, text) = text.SplitIntoWhile(IsAnyThingButNewLine);

            return new DetailData(
                Kind: MatchInfoKind.Anchor,
                MatchPath: ownMatchPath,
                MatchRange: getMatchRange(),
                Command: string.Empty,
                Anchor: PathData.Create(ownMatchPath.FilePath, anchorSlice.ToString()),
                Path: PathData.Empty,
                Comment: commentSlice.ToString(),
                Line: line
                );
        }

        // §>Command_Path§ Comment
        if (ParagraphGreater.ReadWordIfMatches(ref text)) {
            text = text.TrimWhile(IsWhitespaceNorNewLine);

            var commandSlice = StringSlice.Empty;
            var pathSlice = StringSlice.Empty;
            var commentSlice = StringSlice.Empty;
            (commandSlice, text) = text.SplitIntoWhile(IsNotWhitespaceNorNewLine);
            if (commandSlice.IsNullOrEmpty()) { return default; }

            text = text.TrimWhile(IsWhitespaceNorNewLine);

            (pathSlice, text) = text.SplitIntoWhile(IsNotWhitespaceNorNewLine);

            text = text.TrimWhile(IsWhitespaceNorNewLine);

            if (Paragraph.ReadWordIfMatches(ref text)) {
                text = text.TrimWhile(IsWhitespaceNorNewLine);
            }

            (commentSlice, text) = text.SplitIntoWhile(IsAnyThingButNewLine);

            return new DetailData(
                Kind: MatchInfoKind.ParagraphCommand,
                MatchPath: ownMatchPath,
                MatchRange: getMatchRange(),
                Path: PathData.Create(ownMatchPath.FilePath, pathSlice.ToString()),
                Command: commandSlice.ToString(),
                Anchor: PathData.Empty,
                Comment: commentSlice.ToString(),
                Line: line
                );
        }

        // §_Path§Comment

        if (Paragraph.ReadWordIfMatches(ref text)) {
            text = text.TrimWhile(IsWhitespaceNorNewLine);

            var pathSlice = StringSlice.Empty;
            var commentSlice = StringSlice.Empty;
            (pathSlice, text) = text.SplitIntoWhile(IsNotWhitespaceNorNewLine);
            if (pathSlice.IsNullOrEmpty()) { return default; }

            text = text.TrimWhile(IsWhitespaceNorNewLine);

            if (Paragraph.ReadWordIfMatches(ref text)) {
                text = text.TrimWhile(IsWhitespaceNorNewLine);
            }

            (commentSlice, text) = text.SplitIntoWhile(IsAnyThingButNewLine);

            return new DetailData(
                Kind: MatchInfoKind.Paragraph,
                MatchPath: ownMatchPath,
                Path: PathData.Parse(pathSlice.ToString()),
                MatchRange: getMatchRange(),
                Command: string.Empty,
                Anchor: PathData.Empty,
                Comment: commentSlice.ToString(),
                Line: line
                );
        }
        {
            var kind = MatchInfoKind.Invalid;
            if (DetailsProtocol.ReadWordIfMatches(ref text)) {
                kind = MatchInfoKind.DetailsLink;
            } else if (DetailsCodeProtocol.ReadWordIfMatches(ref text)) {
                kind = MatchInfoKind.DetailscodeLink;
            }
            if (kind != MatchInfoKind.Invalid) {
                var pathSlice = StringSlice.Empty;
                var commentSlice = StringSlice.Empty;
                (pathSlice, text) = text.SplitIntoWhile(IsNotWhitespaceNorNewLine);
                if (pathSlice.IsNullOrEmpty()) { return default; }
                text = text.TrimWhile(IsWhitespaceNorNewLine);
                (commentSlice, text) = text.SplitIntoWhile(IsAnyThingButNewLine);

                return new DetailData(
                    Kind: kind,
                    MatchPath: ownMatchPath,
                    MatchRange: getMatchRange(),
                    Command: string.Empty,
                    Anchor: PathData.Empty,
                    Path: PathData.Parse(pathSlice.ToString()),
                    Comment: commentSlice.ToString(),
                    Line: line
                    );
            }
        }

        // [Path](Comment)
        if (OpenSquareBrackets.ReadWordIfMatches(ref text)) {
            var kind = MatchInfoKind.Invalid;
            if (DetailsProtocol.ReadWordIfMatches(ref text)) {
                kind = MatchInfoKind.DetailsLinkMarkdown;
            } else if (DetailsCodeProtocol.ReadWordIfMatches(ref text)) {
                kind = MatchInfoKind.DetailscodeLinkMarkdown;
            }
            if (kind != MatchInfoKind.Invalid) {
                var pathSlice = StringSlice.Empty;
                var commentSlice = StringSlice.Empty;
                (pathSlice, text) = text.SplitIntoWhile(IsNotWhitespaceNorNewLine);
                if (pathSlice.IsNullOrEmpty()) { return default; }
                text = text.TrimWhile(IsWhitespaceNorNewLine);
                if (CloseSquareBrackets.ReadWordIfMatches(ref text)) {
                    text = text.TrimWhile(IsWhitespaceNorNewLine);
                    if (OpenRoundBrackets.ReadWordIfMatches(ref text)) {
                        text = text.TrimWhile(IsWhitespaceNorNewLine);
                        (commentSlice, text) = text.SplitIntoWhile(IsAnyThingButNewLine);
                        if (CloseRoundBrackets.ReadWordIfMatches(ref text)) {
                            return new DetailData(
                                Kind: kind,
                                MatchPath: ownMatchPath,
                                MatchRange: getMatchRange(),
                                Command: string.Empty,
                                Anchor: PathData.Empty,
                                Path: PathData.Parse(pathSlice.ToString()),
                                Comment: commentSlice.ToString(),
                                Line: line
                                );
                        }
                    }
                }
            }
        }

        return null;

        Range getMatchRange() {
            var nextStart = startAfterOptional - startAsOffset + offset;
            var length = text.Range.End.Value - startAfterOptional;
            var result = new Range(nextStart, nextStart + length);
            return result;
        }
    }
}
