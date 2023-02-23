using System.Diagnostics.Metrics;
using System.Drawing;

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
    public static MatchInfo? parseMatch(string value) {
        // § Syntax-Marker.md / Syntax Marker
        // [Syntax Marker](details://Syntax-Marker.md/Syntax Marker)
        //var arr = value.Split(arrCharParagraph);
        //arr = arr.Select(
        //    item => item
        //        .Trim())
        //        .Where((item, idx) => (idx == 0) ? (!string.IsNullOrEmpty(item)) : true)
        //        .ToArray();
        //bool isCommand = false;
        //if (arr.Length > 0) {
        //    if (arr[0].StartsWith(">")) {
        //        arr[0] = arr[0].Substring(1).Trim();
        //        isCommand = true;
        //    }
        //}
        var lexer = new Lexer();

        //string[]? Comments = null;
        int count = 0;
        var spanValue = value.AsSpan();

        lexer.TrimStart(ref spanValue, ref count);

        if (lexer.IsParagraphStart(ref spanValue, ref count)) {
            string Path = string.Empty;
            string Anchor = string.Empty;
            string Comment = string.Empty;

            if (spanValue.Length > 1) {
                var idx = lexer.IndexOfParagraphHash(ref spanValue);
                if (idx >= 0) {
                    Path = lexer.Forward(idx, ref spanValue, ref count).ToString();
                } else {
                    Path = lexer.Forward(spanValue.Length, ref spanValue, ref count).ToString();
                }
            }
            if (spanValue.Length > 0 && spanValue[0] == '#') {
                var idx = lexer.IndexOfParagraphHash(ref spanValue);
                if (idx >= 0) {
                    Anchor = lexer.Forward(idx, ref spanValue, ref count).ToString();
                }
            }
            if (spanValue.Length > 0 && spanValue[0] == '§') {
                var idx = lexer.IndexOfParagraph(ref spanValue);
                if (idx >= 0) {
                    Comment = lexer.Forward(idx, ref spanValue, ref count).ToString();
                } else {
                    Comment = lexer.Forward(spanValue.Length, ref spanValue, ref count).ToString();
                }
            }
            return new MatchInfo(
                MatchInfoKind.Paragraph,
                MatchLength: count,
                Path: Path,
                Command: string.Empty,
                Anchor: Anchor,
                Comment: Comment);
        } else if (lexer.IsParagraphCommandStart(ref spanValue,ref count)) {
            string Path = string.Empty;
            string Comment = string.Empty;
            string Command = string.Empty;

            var idx = lexer.IndexOfWhitespace(ref spanValue);
            if (idx > 0) {
                Command = lexer.Forward(idx, ref spanValue, ref count).ToString();
                lexer.TrimStart(ref spanValue, ref count);

                idx = lexer.IndexOfWhitespace(ref spanValue);
                if (idx > 0) {
                    Path = lexer.Forward(idx, ref spanValue, ref count).ToString();
                    lexer.TrimStart(ref spanValue, ref count);
                }
                idx = lexer.IndexOfNewLine(ref spanValue);
                if (idx >= 0) {
                    Comment = lexer.Forward(idx, ref spanValue, ref count).ToString();
                }
                return new MatchInfo(
                    Kind: MatchInfoKind.ParagraphCommand,
                    MatchLength: count,
                    Command: Command,
                    Anchor: string.Empty,
                    Path: Path,
                    Comment: Comment
                    );
            } else {
                Command = lexer.Forward(spanValue.Length, ref spanValue, ref count).ToString();
                return new MatchInfo(
                    Kind: MatchInfoKind.ParagraphCommand,
                    MatchLength: count,
                    Command: Command,
                    Anchor: string.Empty,
                    Path: Path,
                    Comment: Comment
                    );
            }
        }
        return null;
        //if (spanValue.StartsWith()

        /*
        int state = 0;
        for (int idx = 0; idx < spanValue.Length; idx++){
            if (state == 0) {
                if (char.IsWhiteSpace(spanValue[idx])) {
                    //spanValue = spanValue.Slice(idx);
                    continue;
                }
                if (spanValue[idx] == '§') {
                    state = 1;
                    continue;
                }
                state = -1;
                break;
            }
            if (state == 1) {
                if (spanValue[idx] == '>') {
                    state = 3;
                    continue;
                }
                state = 2;
            }
            if (state == 2 || state == 3) {
                if (char.IsWhiteSpace(spanValue[idx])) {
                    continue;
                }
            }
            if (state == 2) { 
            }
        }
        */

        //var result = new MatchInfo(value, isCommand, arr);

    }
}

public ref struct Lexer {
    public readonly ReadOnlySpan<char> ParagraphGreater = "§>".AsSpan();
    public readonly ReadOnlySpan<char> Paragraph = "§".AsSpan();
    public readonly ReadOnlySpan<char> Hash = "#".AsSpan();
    public readonly ReadOnlySpan<char> WhitespaceNewLine = " \t\r\n".AsSpan();
    public readonly ReadOnlySpan<char> ParagraphNewLine = "§\r\n".AsSpan();
    public readonly ReadOnlySpan<char> ParagraphHashNewLine = "§#\r\n".AsSpan();
    public readonly ReadOnlySpan<char> NewLine = "\r\n".AsSpan();
    public readonly ReadOnlySpan<char> TabSpace = " \t".AsSpan();
    public Lexer() { }

    public bool IsParagraphStart(ref ReadOnlySpan<char> spanValue, ref int count) {
        if (spanValue.StartsWith(this.Paragraph)) {
            if ((spanValue.Length > 2)
                && char.IsWhiteSpace(spanValue[1])) {
                spanValue = spanValue.Slice(2);
                count += 2;
                this.TrimStart(ref spanValue, ref count);
                return true;
            }
        }
        return false;
    }

    public bool IsParagraphCommandStart(ref ReadOnlySpan<char> spanValue, ref int count) {
        if (spanValue.StartsWith(this.ParagraphGreater)) {
            if ((spanValue.Length > 3)
                && char.IsWhiteSpace(spanValue[2])) {
                spanValue = spanValue.Slice(3);
                count += 3;
                this.TrimStart(ref spanValue, ref count);
                return true;
            }
        }
        return false;
    }

    public int IndexOfWhitespace(ref ReadOnlySpan<char> spanValue) {
        return spanValue.IndexOfAny(WhitespaceNewLine);
    }

    public int IndexOfParagraph(ref ReadOnlySpan<char> spanValue) {
        return spanValue.IndexOfAny(ParagraphNewLine);
    }

    public int IndexOfParagraphHash(ref ReadOnlySpan<char> spanValue) {
        return spanValue.IndexOfAny(ParagraphHashNewLine);
    }

    public int IndexOfNewLine(ref ReadOnlySpan<char> spanValue) {
        int idx = spanValue.IndexOfAny(NewLine);
        if (idx >= 0) { return idx; }
        return spanValue.Length;
    }

    public void TrimStart(ref ReadOnlySpan<char> spanValue, ref int count) {
        int idx = 0;
        int lastFound = -1;
        for (; idx < spanValue.Length; idx++) {
            if (spanValue[idx] == ' ' || spanValue[idx] == '\t') {
                lastFound = idx;
                continue;
            }
            break;
        }
        if (lastFound >= 0) {
            spanValue = spanValue.Slice(lastFound);
            count += (lastFound + 1);
        }
    }

    public bool Eat(ReadOnlySpan<char> expected, ref ReadOnlySpan<char> spanValue, ref int count) {
        if (spanValue.StartsWith(expected)) {
            spanValue = spanValue.Slice(expected.Length);
            count += expected.Length;
            return true;
        } else {
            return false;
        }
    }

    public ReadOnlySpan<char> Forward(int length, ref ReadOnlySpan<char> spanValue, ref int count) {
        var result = spanValue[0..length];
        spanValue=spanValue.Slice(length);
        count += length;
        return result;
    }
}