namespace Brimborium.Details;
public class StringSpliceTests {
    [Fact]
    public void T001CreatePartUp() {
        var sut = new StringSplice("Hello World");

        var part22 = sut.CreatePart(2, 2);
        Assert.Equal("ll", part22?.GetText());

        var part81 = sut.CreatePart(8, 1);
        Assert.Equal("r", part81?.GetText());

        Assert.Equal("2..4;8..9", string.Join(";",
            sut.GetArrayPart()?.Select(item => item.Range.ToString())
            ?? throw new Exception("sut.GetArrayPart() is null")));
    }
    [Fact]
    public void T002CreatePartDown() {
        var sut = new StringSplice("Hello World");

        var part81 = sut.CreatePart(8, 1);
        Assert.Equal("r", part81?.GetText());

        var part22 = sut.CreatePart(2, 2);
        Assert.Equal("ll", part22?.GetText());

        Assert.Equal("2..4;8..9", string.Join(";",
            sut.GetArrayPart()?.Select(item => item.Range.ToString())
            ?? throw new Exception("sut.GetArrayPart() is null")));
    }

    [Fact]
    public void T003CreatePartUp() {
        var sut = new StringSplice("Hello World");
        var act = new List<StringSplice>();
        foreach (int start in new int[] { 0, 8, 6, 4 }) {
            var p = sut.CreatePart(start, 2);
            if (p is null) throw new Exception("p is null");
            act.Add(p);
        }
        Assert.Equal("0..2;4..6;6..8;8..10", string.Join(";", sut.GetArrayPart()?.Select(item => item.Range.ToString())
            ?? throw new Exception("sut.GetArrayPart() is null")));
        Assert.Equal("Heo Worl", string.Join("", sut.GetArrayPart()?.Select(item => item.GetText())
            ?? throw new Exception("sut.GetArrayPart() is null")));
    }

    [Fact]
    public void T004CreatePartOverlapUp() {
        var sut = new StringSplice("Hello World");

        Assert.NotNull(sut.CreatePart(4, 4));
        Assert.Null(sut.CreatePart(4, 4));
        Assert.Null(sut.CreatePart(2, 4));
    }

    [Fact]
    public void T005CreatePartLength() {
        var sut = new StringSplice("Hello World");
        Assert.Null(sut.CreatePart(4, 10));
    }

    [Fact]
    public void T006GetOrCreatePart() {
        var sut = new StringSplice("Hello World");
        var a = sut.GetOrCreatePart(4, 4);
        Assert.NotNull(a);
        Assert.Equal(4, a.Range.Start);
        Assert.Equal(8, a.Range.End);
        Assert.Equal(4, a.Length);

        var b = sut.GetOrCreatePart(4, 4);
        Assert.NotNull(b);

        Assert.Same(a, b);

        var c = sut.GetOrCreatePart(4, 40);
        Assert.Null(c);

    }



    [Fact]
    public void T011BuildReplacement() {
        var sut = new StringSplice("Hello World");
        var part = sut.CreatePart(1, 1);
        if (part is null) { throw new Exception("part is null"); }
        part.GetReplacementBuilder().Append("a");
        Assert.Equal("Hallo World", sut.BuildReplacement());
        var part2 = sut.CreatePart(sut.Length, 0);
        if (part2 is null) { throw new Exception("part2 is null"); }
        part2.GetReplacementBuilder().Append("!");
        Assert.Equal("Hallo World!", sut.BuildReplacement());
    }

    [Fact]
    public void T012BuildReplacement() {
        var sut = new StringSplice("123BC");
        //var sutRange = new Range(1, new Index(1, true),
        var part1 = sut.CreatePart(1..^1);
        if (part1 is null) { throw new Exception("part is null"); }
        Assert.Equal("23B", part1.GetText());

        var part2 = part1.CreatePart(1, 1);
        if (part2 is null) { throw new Exception("part2 is null"); }
        part2.SetReplacementText("A");

        Assert.Equal("2AB", part1.BuildReplacement());
        Assert.Equal("12ABC", sut.BuildReplacement());
    }

    [Fact]
    public void T013BuildReplacement() {
        var sut = new StringSplice("123BC");

        var part = new StringSplice(sut.AsSubString(), 1..^1);
        if (part is null) { throw new Exception("part is null"); }
        Assert.Equal("23B", part.GetText());

        var part2 = part.CreatePart(1, 1);
        if (part2 is null) { throw new Exception("part2 is null"); }
        part2.SetReplacementText("A");

        Assert.Equal("2AB", part.BuildReplacement());

        Assert.Equal("123BC", sut.BuildReplacement());
    }

    [Fact]
    public void T014BuildReplacement() {
        var sut = new StringSplice("15");

        sut.CreatePart(1, 0)?.SetReplacementText("2");
        sut.CreatePart(1, 0)?.SetReplacementText("3");
        sut.CreatePart(1, 0)?.SetReplacementText("4");

        Assert.Equal("12345", sut.BuildReplacement());
    }
}
