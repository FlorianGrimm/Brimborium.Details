namespace Brimborium.Details.Parse;

public static class MatchUtility {
    private static readonly string Paragraph = "§";
    private static readonly string ParagraphSpace = "§ ";
    private static readonly string ParagraphTab = "§\t";
    private static readonly string ParagraphGreater = "§>";
    private static readonly string ParagraphHash = "§#";
    private static readonly string DetailsProtocol = "details://";
    private static readonly string DetailsCodeProtocol = "detailscode://";
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


    public static int IsAnyThingButParagraphNorNewLine(char value, int index) {
        if (value == '§') {
            return 1; ;
        }
        if (value == '\r' || value == '\\') {
            return -1;
        }
        return 0;
    }

    public static DetailData? ParseMatch(
        StringSlice value,
        PathData ownMatchPath,
        string[]? listOptionalPrefix,
        int line, int offset) {
        // § details/Syntax-Marker.md#Parse § Parse a line        
        var startAsOffset = value.Range.Start.Value;

        var text = value.TrimWhile(IsWhitespaceNorNewLine);

        if (listOptionalPrefix is not null && listOptionalPrefix.Length>0){
            foreach(var optionalPrefix in listOptionalPrefix) {
                if (optionalPrefix.ReadWordIfMatches(ref text)) { 
                    text = text.TrimWhile(IsWhitespaceNorNewLine); 
                    break;
                }
            }
        }

        var startAfterOptional = text.Range.Start.Value;
        {
            // § details/Syntax-Marker.md#Parse/Anchor parse §# Anchor Comment
            if (ParagraphHash.ReadWordIfMatches(ref text)) {
                text = text.TrimWhile(IsWhitespaceNorNewLine);

                var anchorSlice = StringSlice.Empty;
                var commentSlice = StringSlice.Empty;
                (anchorSlice, text) = text.SplitIntoWhile(IsNotWhitespaceNorNewLine);
                if (anchorSlice.IsNullOrEmpty()) { return default; }

                text = text.TrimWhile(IsWhitespaceNorNewLine);

                (commentSlice, text) = text.SplitIntoWhile(IsAnyThingButNewLine);

                var anchor = PathData.Parse(anchorSlice);
                //Anchor: PathData.Create(ownMatchPath.FilePath, anchorSlice.ToString()),

                return new DetailData(
                    Kind: MatchInfoKind.Anchor,
                    MatchPath: ownMatchPath,
                    MatchRange: getMatchRange(),
                    Command: string.Empty,
                    Path: anchor,
                    Comment: commentSlice.Trim().ToString(),
                    Line: line
                    );
            }
        }
        {
            // § details/Syntax-Marker.md#Parse/ParagraphCommand parse §> Command_Path Comment
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

                var pathData = PathData.Parse(pathSlice);
                //if (string.IsNullOrEmpty(pathData.FilePath)) {
                //    pathData = pathData.WithFilePath(ownMatchPath.FilePath);
                //}

                return new DetailData(
                    Kind: MatchInfoKind.ParagraphCommand,
                    MatchPath: ownMatchPath,
                    MatchRange: getMatchRange(),
                    Path: pathData,
                    Command: commandSlice.ToString(),
                    Comment: commentSlice.Trim().ToString(),
                    Line: line
                    );
            }
        }
        {
            // § details/Syntax-Marker.md#Parse/Paragraph parse §_Path§Comment
            var doesMatch=ParagraphSpace.ReadWordIfMatches(ref text);
            if (!doesMatch) {
                doesMatch = ParagraphTab.ReadWordIfMatches(ref text);
            }
            if (doesMatch) {
                text = text.TrimWhile(IsWhitespaceNorNewLine);

                var pathSlice = StringSlice.Empty;
                var commentSlice = StringSlice.Empty;
                (pathSlice, text) = text.SplitIntoWhile(IsNotWhitespaceNorNewLine);
                if (pathSlice.IsNullOrEmpty()) { return default; }

                text = text.TrimWhile(IsWhitespaceNorNewLine);

                if (Paragraph.ReadWordIfMatches(ref text)) {
                    text = text.TrimWhile(IsWhitespaceNorNewLine);
                }

                //(commentSlice, text) = text.SplitIntoWhile(IsAnyThingButParagraphNorNewLine);
                (commentSlice, text) = text.SplitIntoWhile(IsAnyThingButNewLine);

                var path = PathData.Parse(pathSlice);
                
                return new DetailData(
                    Kind: MatchInfoKind.Paragraph,
                    MatchPath: ownMatchPath,
                    Path: path,
                    MatchRange: getMatchRange(),
                    Command: string.Empty,
                    Comment: commentSlice.Trim().ToString(),
                    Line: line
                    );
            }
        }
        {
            // § details/Syntax-Marker.md#Parse/DetailsLink parse details://path WS Comment
            // § details/Syntax-Marker.md#Parse/DetailscodeLink parse detailscode://path WS Comment
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
                        Path: PathData.Parse(pathSlice.ToString()),
                        Comment: commentSlice.Trim().ToString(),
                        Line: line
                        );
                }
            }
        }
        {
            // § details/Syntax-Marker.md#Parse/DetailsLinkMarkdown parse detailscode://path WS Comment
            // § details/Syntax-Marker.md#Parse/DetailscodeLinkMarkdown parse detailscode://path WS Comment
            // parse [Path](Comment)
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
                                    Path: PathData.Parse(pathSlice.ToString()),
                                    Comment: commentSlice.Trim().ToString(),
                                    Line: line
                                    );
                            }
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
