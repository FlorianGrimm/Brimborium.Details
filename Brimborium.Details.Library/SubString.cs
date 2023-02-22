﻿namespace Brimborium.Details;

public struct SubString {
    private readonly string _Text = String.Empty;
    private readonly Range _Range;

    public SubString(
       string text
     ) {
        this._Text = text;
        this._Range = new Range(0, text.Length);
    }
    public SubString(
        string text,
        Range range) {
        this._Text = text;
        if (range.Start.IsFromEnd || range.End.IsFromEnd) {
            var (offset, length) = range.GetOffsetAndLength(text.Length);
            range = new Range(offset, offset + length);
        } else {
            if (text.Length < range.Start.Value) { throw new ArgumentOutOfRangeException(nameof(range)); }
            if (text.Length < range.End.Value) { throw new ArgumentOutOfRangeException(nameof(range)); }
        }
        this._Range = range;
    }

    public SubString GetSubString(int start, int length) {
        if (start < 0) { throw new ArgumentOutOfRangeException(nameof(start)); }
        if (length < 0) { throw new ArgumentOutOfRangeException(nameof(length)); }
        var nextRange = new Range(start, start + length);
        return new SubString(
            this._Text,
            nextRange
            );
    }

    public SubString GetSubString(Range range) {
        var (thisOffset, thisLength) = this.Range.GetOffsetAndLength(this._Text.Length);
        var (rangeOffset, rangeLength) = range.GetOffsetAndLength(thisLength);

        var nextRange = new Range(thisOffset + rangeOffset, thisOffset + rangeOffset + rangeLength);
        if (nextRange.Start.Value > nextRange.End.Value) { throw new ArgumentOutOfRangeException(nameof(range)); }
        if (thisLength < nextRange.Start.Value) { throw new ArgumentOutOfRangeException(nameof(range)); }
        if (thisLength < nextRange.End.Value) { throw new ArgumentOutOfRangeException(nameof(range)); }

        return new SubString(
            this._Text,
            nextRange
            );
    }

    public string Text => this.ToString();
    public Range Range => _Range;

    public int Start => this.Range.Start.Value;
    
    public int Length {
        get {
            var (_, length) = this.Range.GetOffsetAndLength(this._Text.Length);
            return length;
        }
    }

    public int End => this.Range.End.Value;
    
    override public string ToString()
            => this._Text[this.Range];

    public ReadOnlySpan<char> AsSpan() 
        => this._Text.AsSpan()[this.Range];
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

    public bool IsRangeValid(
        StringSpliceBase stringSplice,
        int start,
        int length
        ) {
        if (start < 0) { return false; }
        if (stringSplice.Length < (start + length)) { return false; }
        return true;
    }

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