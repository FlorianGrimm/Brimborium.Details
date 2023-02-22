namespace Brimborium.Details;
public class StringSpliceTests {
    [Fact]
    public void T001CreatePartUp() {
        var sut = new StringSplice("Hello World");

        var part22 = sut.CreatePart(2, 2);
        Assert.Equal("ll", part22?.GetText());

        var part81 = sut.CreatePart(8, 1);
        Assert.Equal("r", part81?.GetText());

        Assert.Equal("2..4;8..9", string.Join(";", sut.GetArrayPart().Select(item => item.Range.ToString())));
    }
     [Fact]
    public void T002CreatePartDown() {
        var sut = new StringSplice("Hello World");

        var part81 = sut.CreatePart(8, 1);
        Assert.Equal("r", part81?.GetText());

        var part22 = sut.CreatePart(2, 2);
        Assert.Equal("ll", part22?.GetText());

        Assert.Equal("2..4;8..9", string.Join(";", sut.GetArrayPart().Select(item => item.Range.ToString())));
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
        Assert.Equal("0..2;4..6;6..8;8..10", string.Join(";", sut.GetArrayPart().Select(item => item.Range.ToString())));
        Assert.Equal("Heo Worl", string.Join("", sut.GetArrayPart().Select(item => item.GetText())));
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
    public void T001BuildReplacement() {
        var sut = new StringSplice("Hello World");
        var part = sut.CreatePart(1, 1);
        if (part is null) { throw new Exception("part is null"); }
        part.GetReplacementuilder().Append("a");
        Assert.Equal("Hallo World", sut.BuildReplacement());
        var part2 = sut.CreatePart(sut.Length, 0);
        if (part2 is null) { throw new Exception("part2 is null"); }
        part2.GetReplacementBuilder().Append("!");
        Assert.Equal("Hallo World!", sut.BuildReplacement());
    }

}
