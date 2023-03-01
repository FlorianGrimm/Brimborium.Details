using System.Diagnostics.Metrics;
using System.Drawing;
using System.Xml.Linq;

namespace Brimborium.Details;

public static class MatchUtility {
    private static readonly char[] arrCharParagraph = new char[] { '§' };

    /*
    public static MatchInfo? parseMatchIfMatches(string value) {
        if (!value.Contains('§')) { return null; }
        string normalized = value.TrimStart();
        if (!normalized.StartsWith("§")) {
            return null;
        }
        return parseMatch(normalized);
    }

    private readonly static Regex _RegexParagraphPath = new Regex(
        @"^[§](?<Path>[^>][^#§]{0,256})(?<Kind>[^§#]{0,64})(?<Anchor>[#][^§]{0,64})(?<Comment>[§][^§]{0,64})[§]?",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline);

    private readonly static Regex _RegexParagraphCommand = new Regex(
        @"^[§][>](?<Command>[^#§]{0,256})(?<Kind>[^§#]{0,64})(?<Anchor>[#][^§]{0,64})(?<Comment>[§][^§]{0,64})[§]?",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline);
    */
    /*
     details
     detailscode
     */
    public static MatchInfo? parseMatch(
        string value, 
        PathInfo ownMatchPath,
        int line, int offset) {
        var lexer = new Lexer();
        bool eof = false;
        int end = offset;
        var spanValue = value.AsSpan();

        lexer.EatWhile(lexer.Whitespace, ref spanValue, ref end, ref eof);

        if (lexer.EatWord(lexer.SlashSlash, ref spanValue, ref end)) {
            lexer.EatWhile(lexer.Whitespace, ref spanValue, ref end, ref eof);
        }

        int start = end;
        if (lexer.EatWord(lexer.ParagraphHash, ref spanValue, ref end)) {
            lexer.EatWhile(lexer.Whitespace, ref spanValue, ref end, ref eof);
            if (!eof) {
                var anchorValue = lexer.EatUntil(lexer.Whitespace, ref spanValue, ref end, ref eof);
                if (anchorValue.Length > 0) {
                    var Anchor = anchorValue.ToString();
                    string Comment = String.Empty;
                    lexer.EatWhile(lexer.Whitespace, ref spanValue, ref end, ref eof);
                    var commentValue = lexer.EatUntil(lexer.NewLine, ref spanValue, ref end, ref eof);
                    if (commentValue.Length > 0) {
                        Comment = anchorValue.ToString();
                    }
                    return new MatchInfo(
                        Kind: MatchInfoKind.Anchor,
                        MatchPath: ownMatchPath,
                        MatchRange: new Range(start, end),
                        Command: string.Empty,
                        Anchor: PathInfo.Create(ownMatchPath.FilePath, Anchor),
                        Path: PathInfo.Empty,
                        Comment: Comment,
                        Line: line
                        );
                }
            } else {
                return null;
            }
        }

        if (lexer.EatWord(lexer.ParagraphGreater, ref spanValue, ref end)) {
            lexer.EatWhile(lexer.Whitespace, ref spanValue, ref end, ref eof);
            if (eof) { return null; }
            // §>Command_Path§
            var commandValue = lexer.EatUntil(lexer.Whitespace, ref spanValue, ref end, ref eof);
            if (commandValue.Length == 0) { return null; }
            string Command = commandValue.ToString();
            string Path = string.Empty;
            string Comment = string.Empty;

            if (!eof) {
                lexer.EatWhile(lexer.Whitespace, ref spanValue, ref end, ref eof);
            }

            if (!eof) {
                if (lexer.EatWord(lexer.Paragraph, ref spanValue, ref end)) {
                    if (!eof) {
                        lexer.EatWhile(lexer.Whitespace, ref spanValue, ref end, ref eof);
                    }
                }
            }
            if (!eof) {
                var pathValue = lexer.EatUntil(lexer.Whitespace, ref spanValue, ref end, ref eof);
                if (pathValue.Length > 0) {
                    Path = pathValue.ToString();
                    if (!eof) {
                        lexer.EatWhile(lexer.Whitespace, ref spanValue, ref end, ref eof);
                    }
                    if (!eof) {
                        var commentValue = lexer.EatUntil(lexer.Paragraph, ref spanValue, ref end, ref eof);
                        if (commentValue.Length > 0) {
                            Comment = commandValue.ToString();
                            if (!eof && lexer.EatWord(lexer.Paragraph, ref spanValue, ref end)) {
                            }
                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(Command)) {
                return default;
            }
            return new MatchInfo(
                Kind: MatchInfoKind.ParagraphCommand,
                MatchPath: ownMatchPath,
                MatchRange: new Range(start, end),
                Command: Command,
                Anchor: PathInfo.Empty,
                Path: PathInfo.Parse(Path),
                Comment: Comment,
                Line: line
                );
        }

        if (lexer.EatWord(lexer.Paragraph, ref spanValue, ref end)) {
            lexer.EatWhile(lexer.Whitespace, ref spanValue, ref end, ref eof);
            if (eof) { return null; }
            // §_Path§Comment§
            string Path = string.Empty;
            string Anchor = string.Empty;
            string Comment = string.Empty;

            var pathValue = lexer.EatUntil(lexer.Paragraph, ref spanValue, ref end, ref eof);
            if (pathValue.Length > 0) {
                Path = pathValue.TrimEnd().ToString();
            } else {
                return default;
            }

            if (!eof && lexer.EatWord(lexer.Paragraph, ref spanValue, ref end)) {
                lexer.EatWhile(lexer.Whitespace, ref spanValue, ref end, ref eof);

                var commentValue = lexer.EatUntil(lexer.Paragraph, ref spanValue, ref end, ref eof);
                if (commentValue.Length > 0) {
                    Comment = commentValue.TrimEnd().ToString();

                    if (!eof) {
                        lexer.EatWord(lexer.Paragraph, ref spanValue, ref end);
                    }
                }
            }

            return new MatchInfo(
                MatchInfoKind.Paragraph,
                MatchPath: ownMatchPath,
                MatchRange: new Range(start, end),
                Path: PathInfo.Parse(Path),
                Command: string.Empty,
                Anchor: PathInfo.Parse(Anchor),
                Comment: Comment,
                Line: line);
        }

        {
            var kind = MatchInfoKind.Invalid;
            if (lexer.EatWord(lexer.DetailsProtocol, ref spanValue, ref end)) {
                kind = MatchInfoKind.DetailsLink;
            } else if (lexer.EatWord(lexer.DetailsCodeProtocol, ref spanValue, ref end)) {
                kind = MatchInfoKind.DetailscodeLink;
            }
            if (kind != MatchInfoKind.Invalid) {
                string Path = string.Empty;
                string Comment = string.Empty;
                var pathValue = lexer.EatUntil(lexer.Whitespace, ref spanValue, ref end, ref eof);
                if (pathValue.Length > 0) {
                    Path = pathValue.ToString();
                } else {
                    return null;
                }
                if (!eof) {
                    lexer.EatWhile(lexer.Whitespace, ref spanValue, ref end, ref eof);
                }
                if (!eof) {
                    var commentValue = lexer.EatUntil(lexer.NewLine, ref spanValue, ref end, ref eof);
                    if (commentValue.Length > 0) {
                        Comment = commentValue.ToString();
                    }
                }

                return new MatchInfo(
                    Kind: kind,
                    MatchPath: ownMatchPath,
                    MatchRange: new Range(start, end),
                    Command: string.Empty,
                    Anchor: PathInfo.Empty,
                    Path: PathInfo.Parse(Path),
                    Comment: Comment,
                    Line: line
                    );
            }
        }
        if (lexer.EatWord(lexer.OpenSquareBrackets, ref spanValue, ref end)) {
            var kind = MatchInfoKind.Paragraph;
            if (lexer.EatWord(lexer.DetailsProtocol, ref spanValue, ref end)) {
                kind = MatchInfoKind.DetailsLink;
            } else if (lexer.EatWord(lexer.DetailsCodeProtocol, ref spanValue, ref end)) {
                kind = MatchInfoKind.DetailscodeLink;
            }
            if (kind != MatchInfoKind.Invalid) {
                var pathValue = lexer.EatUntil(lexer.CloseSquareBrackets, ref spanValue, ref end, ref eof);
                if (!eof && pathValue.Length > 0) {
                    string Path = pathValue.ToString();
                    if (lexer.EatWord(lexer.CloseSquareBrackets, ref spanValue, ref end)) {
                        if (lexer.EatWord(lexer.OpenRoundBrackets, ref spanValue, ref end)) {
                            var commentValue = lexer.EatUntil(lexer.CloseRoundBrackets, ref spanValue, ref end, ref eof);
                            var Comment = commentValue.ToString();
                            lexer.EatWord(lexer.CloseRoundBrackets, ref spanValue, ref end);
                            return new MatchInfo(
                                Kind: kind,
                                MatchPath: ownMatchPath,
                                MatchRange: new Range(start, end),
                                Command: string.Empty,
                                Anchor: PathInfo.Empty,
                                Path: PathInfo.Parse(Path),
                                Comment: Comment,
                                Line: line
                                );
                        }
                    }
                }
            }
        }
        return null;
    }
}
