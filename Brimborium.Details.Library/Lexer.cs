namespace Brimborium.Details;

/*
 § a#a § coment §
 §> command paragraph §
 */
public readonly ref struct Lexer {
    public readonly ReadOnlySpan<char> Paragraph = "§".AsSpan();
    public readonly ReadOnlySpan<char> ParagraphGreater = "§>".AsSpan();
    public readonly ReadOnlySpan<char> ParagraphHash = "§#".AsSpan();
    public readonly ReadOnlySpan<char> Hash = "#".AsSpan();
    public readonly ReadOnlySpan<char> ParagraphHashNewLine = "§#\r\n".AsSpan();
    public readonly ReadOnlySpan<char> Whitespace = " \t".AsSpan();
    public readonly ReadOnlySpan<char> WhitespaceNewLine = " \t\r\n".AsSpan();
    public readonly ReadOnlySpan<char> ParagraphNewLine = "§\r\n".AsSpan();
    public readonly ReadOnlySpan<char> NewLine = "\r\n".AsSpan();
    public readonly ReadOnlySpan<char> TabSpace = " \t".AsSpan();
    public readonly ReadOnlySpan<char> SlashSlash = "//".AsSpan();
    public readonly ReadOnlySpan<char> DetailsProtocol = "details://".AsSpan();
    public readonly ReadOnlySpan<char> DetailsCodeProtocol = "details-code://".AsSpan();
    public readonly ReadOnlySpan<char> OpenSquareBrackets = "[".AsSpan();
    public readonly ReadOnlySpan<char> CloseSquareBrackets = "]".AsSpan();
    public readonly ReadOnlySpan<char> OpenRoundBrackets = "(".AsSpan();
    public readonly ReadOnlySpan<char> CloseRoundBrackets = ")".AsSpan();

    public Lexer() { }

#if false
    public bool IsParagraphCommandStart(ref ReadOnlySpan<char> spanValue, ref int count, ref bool eof) {
        if (spanValue.StartsWith(this.ParagraphGreater)) {
            if ((spanValue.Length > 3)
                && char.IsWhiteSpace(spanValue[2])) {
                spanValue = spanValue.Slice(3);
                count += 3;
                this.EatWhile(this.Whitespace, ref spanValue, ref count, ref eof);
                return true;
            }
        }
        return false;
    }
    public bool IsParagraphHashStart(ref ReadOnlySpan<char> spanValue, ref int count) {
        if (spanValue.StartsWith(this.ParagraphHash)) {
            if ((spanValue.Length > 3)
                && char.IsWhiteSpace(spanValue[2])) {
                spanValue = spanValue.Slice(3);
                count += 3;
                this.EatWhile(this.Whitespace, ref spanValue, ref count, ref eof);
                return true;
            }
        }
        return false;
    }
#endif

    public int IndexOfWhitespace(ref ReadOnlySpan<char> spanValue) {
        return spanValue.IndexOfAny(WhitespaceNewLine);
    }

    public int IndexOfParagraph(ref ReadOnlySpan<char> spanValue) {
        return spanValue.IndexOfAny(ParagraphNewLine);
    }

    public int IndexOfParagraphOrHash(ref ReadOnlySpan<char> spanValue) {
        return spanValue.IndexOfAny(ParagraphHashNewLine);
    }

    public int IndexOfNewLine(ref ReadOnlySpan<char> spanValue) {
        int idx = spanValue.IndexOfAny(NewLine);
        if (idx >= 0) { return idx; }
        return spanValue.Length;
    }

#if false
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
            spanValue = spanValue.Slice(lastFound + 1);
            count += (lastFound + 1);
        }
    }
#endif
    public bool EatWord(ReadOnlySpan<char> expected, ref ReadOnlySpan<char> spanValue, ref int count) {
        if (spanValue.StartsWith(expected)) {
            spanValue = spanValue.Slice(expected.Length);
            count += expected.Length;
            return true;
        } else {
            return false;
        }
    }

    public ReadOnlySpan<char> EatWhile(ReadOnlySpan<char> search, ref ReadOnlySpan<char> spanValue, ref int count, ref bool eof) {
        for (int pos = 0; pos < spanValue.Length; pos++) {
            if ((spanValue[pos] == '\r') || (spanValue[pos] == '\n')) {
                eof = true;
                var result = spanValue[0..pos];
                spanValue = spanValue.Slice(pos);
                return result;
            }
            bool found = false;
            for (int idx = search.Length - 1; idx >= 0; idx--) {
                if (search[idx] == spanValue[pos]) {
                    found = true;
                    break;
                }
            }
            if (found) {
                continue;
            } else {
                var result = spanValue[0..pos];
                spanValue = spanValue.Slice(pos);
                return result;
            }
        }
        {
            eof = true;
            var result = spanValue;
            spanValue = string.Empty.AsSpan();
            return result;
        }
    }

    public ReadOnlySpan<char> EatUntil(ReadOnlySpan<char> search, ref ReadOnlySpan<char> spanValue, ref int count, ref bool eof) {
        for (int pos = 0; pos < spanValue.Length; pos++) {
            if ((spanValue[pos] == '\r') || (spanValue[pos] == '\n')) {
                eof = true;
                var result = spanValue[0..pos];
                spanValue = spanValue.Slice(pos);
                return result;
            }
            bool found = false;
            for (int idx = search.Length - 1; idx >= 0; idx--) {
                if (search[idx] == spanValue[pos]) {
                    found = true;
                    break;
                }
            }
            if (found) {
                var result = spanValue[0..pos];
                spanValue = spanValue.Slice(pos);
                return result;
            } else {
                continue;
            }
        }
        {
            eof = true;
            var result = spanValue;
            spanValue = string.Empty.AsSpan();
            return result;
        }
    }

    public ReadOnlySpan<char> EatLength(int length, ref ReadOnlySpan<char> spanValue, ref int count) {
        var result = spanValue[0..length];
        spanValue = spanValue.Slice(length);
        count += length;
        return result;
    }
}