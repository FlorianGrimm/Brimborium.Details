namespace Brimborium.Details;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class StringSplice {
    private readonly SubString _Text;
    private readonly Range _Range;
    private List<StringSplice>? _LstPart;
    private StringBuilder? _ReplacementBuilder;
    private string? _ReplacementText;

    public StringSplice(string text) {
        this._Text = new SubString(text);
        this._Range = new Range(0, text.Length);
    }

    public StringSplice(SubString text) {
        this._Text = text;
        this._Range = new Range(0, text.Length);
    }

    public StringSplice(
        SubString text,
        int start,
        int length) {
        this._Text = text;
        var range = new Range(start, start + length);
        if (range.Start.Value < 0 || range.Start.Value > text.Length) { throw new ArgumentOutOfRangeException(nameof(start)); }
        if (range.End.Value < 0 || range.End.Value > text.Length) { throw new ArgumentOutOfRangeException(nameof(length)); }
        this._Range = range;
    }

    public StringSplice(
        SubString text,
        Range range) {
        this._Text = text;
        if (range.Start.IsFromEnd || range.End.IsFromEnd) {

            var (rangeOffset, rangeLength) = range.GetOffsetAndLength(text.Length);

            var rangeEnd = rangeOffset + rangeLength;

            range = new Range(rangeOffset, rangeEnd);
        }
        if (range.Start.Value < 0 || range.Start.Value > text.Length) { throw new ArgumentOutOfRangeException(nameof(range)); }
        if (range.End.Value < 0 || range.End.Value > text.Length) { throw new ArgumentOutOfRangeException(nameof(range)); }
        if (range.End.Value < range.Start.Value) { throw new ArgumentOutOfRangeException(nameof(range)); }
        this._Range = range;
    }

    public SubString AsSubString() => this._Text.GetSubString(this._Range);
    public string GetText() => this.AsSubString().ToString();

    public Range Range => _Range;
    public int Length { 
        get {
            var (_, length) = this.Range.GetOffsetAndLength(this._Text.Length);
            return length;
        }
    }

    public string? GetReplacementText() { return this._ReplacementText; }

    public void SetReplacementText(string? value) {
        if (this._ReplacementBuilder is not null) {
            throw new InvalidOperationException("Use only one of ReplacmentText and ReplacmentBuilder.");
        }
        if (this._LstPart is not null) {
            throw new InvalidOperationException("Use only one of ReplacmentText and Parts.");
        }
        this._ReplacementText = value;
    }

    public void SetReplacementBuilder(StringBuilder? value) {
        if (this._ReplacementText is not null) {
            throw new InvalidOperationException("Use only one of ReplacmentText and ReplacmentBuilder.");
        }
        if (this._LstPart is not null) {
            throw new InvalidOperationException("Use only one of ReplacmentBuilder and Parts.");
        }

        this._ReplacementBuilder = value;
    }

    public StringBuilder GetReplacementBuilder() {
        if (this._ReplacementText is not null) {
            throw new InvalidOperationException("Use only one of ReplacmentText and ReplacmentBuilder.");
        }
        if (this._LstPart is not null) {
            throw new InvalidOperationException("Use only one of ReplacmentBuilder and Parts.");
        }

        return this._ReplacementBuilder ??= new StringBuilder();
    }

    public StringSplice[]? GetArrayPart() => this._LstPart?.ToArray();

    public bool IsRangeValid(
         int start,
         int length
    ) {
        if (start < 0) { return false; }
        if (length < 0) { return false; }

        // skip the length is positive
        // if (this.Length < start) { return false; }

        if (this.Length < (start + length)) { return false; }
        return true;
    }

    public StringSplice? CreatePart(int start, int length) {
        if (!this.IsRangeValid(start, length)) {
            return null;
        }
        if (this._LstPart is null) {
            if (this._ReplacementText is not null) {
                throw new InvalidOperationException("Use only one of ReplacmentText and Parts.");
            }
            if (this._ReplacementBuilder is not null) {
                throw new InvalidOperationException("Use only one of ReplacmentBuilder and Parts.");
            }
            this._LstPart = new List<StringSplice>();
        }

        for (int idx = 0; idx < this._LstPart.Count; idx++) {
            var item = this._LstPart[idx];
            if (item.Range.Start.Value < start) {
                continue;
            }
            if (item.Range.Start.Value == start) {
                // special case for length==0 add behind the with this start.
                if (length == 0) {
                    while ((idx + 1) < this._LstPart.Count) {
                        if (item.Range.Start.Value == start) {
                            idx++;
                            continue;
                        } else {
                            break;
                        }
                    }
                    {
                        var result = this.Factory(start, length);
                        if (result is not null) {
                            this._LstPart.Insert(idx+1, result);
                        }
                        return result;
                    }
                }
                return null;
            }
            {
                // within the span?
                if (item.Range.Start.Value < (start + length)) {
                    return null;
                }
                var result = this.Factory(start, length);
                if (result is not null) {
                    this._LstPart.Insert(idx, result);
                }
                return result;
            }
        }
        {
            var result = this.Factory(start, length);
            if (result is not null) {
                this._LstPart.Add(result);
            }
            return result;
        }
    }

    public StringSplice? CreatePart(Range range) {
        var (offset, length) = range.GetOffsetAndLength(this._Text.Length);
        return this.CreatePart(offset, length);
    }

    public StringSplice? GetOrCreatePart(int start, int length) {
        if (!this.IsRangeValid(start, length)) { return null; }

        if (this._LstPart is null) {
            if (this._ReplacementText is not null) {
                throw new InvalidOperationException("Use only one of ReplacmentText and Parts.");
            }
            if (this._ReplacementBuilder is not null) {
                throw new InvalidOperationException("Use only one of ReplacmentBuilder and Parts.");
            }
            this._LstPart = new List<StringSplice>();
        }

        for (int idx = 0; idx < this._LstPart.Count; idx++) {
            var item = this._LstPart[idx];
            if (item.Range.Start.Value < start) {
                continue;
            }
            if (item.Range.Start.Value == start) {
                if (item.Length == length) {
                    return item;
                }
                return null;
            }
            {
                if (item.Range.Start.Value < (start + length)) {
                    return null;
                }
                var result = this.Factory(start, length);
                if (result is not null) {
                    this._LstPart.Insert(idx, result);
                }
                return result;
            }
        }
        {
            var result = this.Factory(start, length);
            if (result is not null) {
                this._LstPart.Add(result);
            }
            return result;
        }
    }


    public IEnumerable<StringSplice> GetLstPartInRange(int start, int length) {
        if (!this.IsRangeValid(start, length)) {
            yield break;
        } else if (this._LstPart is null) {
            yield break;
        } else {
            var end = start + length;
            for (int idx = 0; idx < this._LstPart.Count; idx++) {
                var item = this._LstPart[idx];
                if (start <= item.Range.Start.Value && item.Range.Start.Value <= end) {
                    yield return item;
                    continue;
                }
                if (start <= item.Range.End.Value && item.Range.End.Value <= end) {
                    yield return item;
                    continue;
                }
                yield break;
            }
        }
    }


    protected virtual StringSplice Factory(int start, int length) {
        return new StringSplice(this.AsSubString(), start, length);
    }

    public string BuildReplacement() {
        if (this._LstPart is not null && this._LstPart.Count > 0) {
            var result = new StringBuilder();
            this.BuildReplacementStringBuilder(result);
            return result.ToString();
        } else {
            return this.GetText();
        }
    }

    public void BuildReplacementStringBuilder(StringBuilder result) {
        if (this._LstPart is null) {
            return;
        } else {
            int posEnd = 0;
            for (int idx = 0; idx < this._LstPart.Count; idx++) {
                var item = this._LstPart[idx];
                if (posEnd < item.Range.Start.Value) {
                    var span = this._Text.GetSubString(this.Range)
                        .AsSpan()[new Range(posEnd, item.Range.Start.Value)];
                    result.Append(span);
                }

                if (item._ReplacementText is not null) {
                    result.Append(item._ReplacementText!);
                } else if (item._ReplacementBuilder is not null) {
                    result.Append(item._ReplacementBuilder!);
                } else if (item._LstPart is not null) {
                    item.BuildReplacementStringBuilder(result);
                }

                posEnd = item.Range.End.Value;
            }

            // add the tail
            if (posEnd < this.Length) {
                var span = this._Text.GetSubString(this.Range).AsSpan();
                if (posEnd == 0) {
                    result.Append(span);
                } else {
                    span = span[posEnd..^0];
                    result.Append(span);
                }
            }
        }
    }

    public override string ToString() {
        return this.BuildReplacement();
    }

    private string GetDebuggerDisplay() {
        return $"{this._Text}; Range:{this.Range}; #Part:{this._LstPart?.Count};";
    }
}
#if false
public abstract class StringSpliceBase {
    protected StringSpliceBase(string text) {
        this.Text = text;
    }

    public string Text { get; }

    public int Length => this.Text.Length;

}

public abstract class StringSpliceBase<TStringSplicePart>
    : StringSpliceBase
    where TStringSplicePart : StringSplicePart {
    private readonly List<TStringSplicePart> _Part;

    protected StringSpliceBase(string text)
        : base(text) {
        this._Part = new List<TStringSplicePart>();
    }

    protected abstract TStringSplicePart? Factory(
        StringSpliceBase stringSplice,
        int start,
        int length);

 

    public TStringSplicePart? CreatePart(int start, int length) {
        if (!this.IsRangeValid(this, start, length)) {
            return null;
        }
        for (int idx = 0; idx < this._Part.Count; idx++) {
            var item = this._Part[idx];
            if (item.Start < start) {
                continue;
            }
            if (item.Start == start) {
                return null;
            }
            {
                if (item.Start < (start + length)) {
                    return null;
                }
                var result = this.Factory(this, start, length);
                if (result is not null) {
                    this._Part.Insert(idx, result);
                }
                return result;
            }
        }
        {
            var result = this.Factory(this, start, length);
            if (result is not null) {
                this._Part.Add(result);
            }
            return result;
        }
    }

    public TStringSplicePart? GetOrCreatePart(int start, int length) {
        if (!this.IsRangeValid(this, start, length)) {
            return null;
        }

        for (int idx = 0; idx < this._Part.Count; idx++) {
            var item = this._Part[idx];
            if (item.Start < start) {
                continue;
            }
            if (item.Start == start) {
                if (item.Length == length) {
                    return item;
                }
                return null;
            }
            {
                if (item.Start < (start + length)) {
                    return null;
                }
                var result = this.Factory(this, start, length);
                if (result is not null) {
                    this._Part.Insert(idx, result);
                }
                return result;
            }
        }
        {
            var result = this.Factory(this, start, length);
            if (result is not null) {
                this._Part.Add(result);
            }
            return result;
        }
    }

    public TStringSplicePart? GetPartExact(int start) {
        for (int idx = 0; idx < this._Part.Count; idx++) {
            var item = this._Part[idx];
            if (item.Start < start) {
                continue;
            }
            if (item.Start == start) {
                return item;
            }
            return null;
        }
        return null;
    }
    public IEnumerable<TStringSplicePart> GetPartsStartInRange(int start, int length) {
        int end = start + length;
        for (int idx = 0; idx < this._Part.Count; idx++) {
            var item = this._Part[idx];
            if (item.Start < start) {
                continue;
            }
            if (start <= item.Start && item.Start < end) {
                yield return item;
                continue;
            }
            yield break;
        }
    }

    public TStringSplicePart[] GetParts() => this._Part.ToArray();

    public string BuildReplacement() {
        if (this._Part.Count == 0) {
            return this.Text;
        }
        var result = new StringBuilder();
        int posEnd = 0;
        for (int idx = 0; idx < this._Part.Count; idx++) {
            var item = this._Part[idx];
            if (posEnd < item.Start) {
                result.Append(this.Text.Substring(posEnd, item.Start-posEnd));
            }
            var replacement = item.GetReplacement();
            if (replacement is not null) {
                result.Append(replacement);
            } else {
                result.Append(item.Text);
            }
            posEnd=item.Start + item.Length;
        }
        if (posEnd < this.Length) {
            result.Append(this.Text.Substring(posEnd));
        }
        return result.ToString();
    }

}

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public abstract class StringSplicePart {
    protected StringSplicePart(
        StringSpliceBase stringSplice,
        int start,
        int length) {
        this.StringSplice = stringSplice;
        this.Start = start;
        this.Length = length;
    }

    public StringSpliceBase StringSplice { get; }

    public int Start { get; }

    public int Length { get; }

    private string? _Text;
    public string Text => this._Text ??= this.StringSplice.Text.Substring(this.Start, this.Length);

    internal protected abstract string? GetReplacement();

    public override string ToString() => this.Text;

    private string GetDebuggerDisplay()
        => $"{this.Start}-{this.Length}";
}

public sealed class StringSpliceFactory<TStringSplicePart>
    : StringSpliceBase<TStringSplicePart> 
    where TStringSplicePart : StringSplicePart {
    private readonly Func<StringSpliceBase, int, int, TStringSplicePart?> _Factory;

    public StringSpliceFactory(
        string text,
        Func<StringSpliceBase /*stringSplice*/, int /*start*/, int /*length*/, TStringSplicePart ?> factory
        ) : base(text) {
    this._Factory = factory;
}

protected override TStringSplicePart? Factory(StringSpliceBase stringSplice, int start, int length) {
    return this._Factory(this, start, length);
}
}

public sealed class StringSpliceStringBuilder
    : StringSpliceBase<StringSplicePartStringBuilder> {
    public StringSpliceStringBuilder(string text) : base(text) {
    }
    protected override StringSplicePartStringBuilder? Factory(StringSpliceBase stringSplice, int start, int length) {
        return new StringSplicePartStringBuilder(stringSplice, start, length);
    }
}

public class StringSplicePartStringBuilder : StringSplicePart {
    public StringSplicePartStringBuilder(
        StringSpliceBase stringSplice,
        int start,
        int length
        ) : base(
            stringSplice,
            start,
            length
            ) {
    }
    public StringBuilder? Replacement { get; set; }

    public StringBuilder GetReplacementBuilder() {
        return this.Replacement ??= new StringBuilder();
    }

    protected internal override string? GetReplacement() {
        return (this.Replacement is not null)
            ? this.Replacement.ToString()
            : null;
    }
}
#endif
// public class StringSlice {
//    public StringSlice(string text, int start, int length) {
//        this.Text = text;
//        this.Start = start;
//        this.Length = length;
//    }
//}
