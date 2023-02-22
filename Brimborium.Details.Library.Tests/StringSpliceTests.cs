using System.Linq;

namespace Brimborium.Details;

public class StringSpliceTests {
    [Fact]
    public void T001CreatePartUp() {
        var sut = new StringSpliceStringBuilder("Hello World");

        var part22 = sut.CreatePart(2, 2);
        Assert.Equal("ll", part22?.Text);

        var part81 = sut.CreatePart(8, 1);
        Assert.Equal("r", part81?.Text);

        Assert.Equal("2,2;8,1", string.Join(";", sut.GetParts().Select(item => $"{item.Start},{item.Length}")));
    }

    [Fact]
    public void T002CreatePartDown() {
        var sut = new StringSpliceStringBuilder("Hello World");

        var part81 = sut.CreatePart(8, 1);
        Assert.Equal("r", part81?.Text);

        var part22 = sut.CreatePart(2, 2);
        Assert.Equal("ll", part22?.Text);

        Assert.Equal("2,2;8,1", string.Join(";", sut.GetParts().Select(item => $"{item.Start},{item.Length}")));
    }

    [Fact]
    public void T003CreatePartUp() {
        var sut = new StringSpliceStringBuilder("Hello World");
        var act = new List<StringSplicePart>();
        foreach (int start in new int[] { 0, 8, 6, 4 }) {
            var p = sut.CreatePart(start, 2);
            if (p is null) throw new Exception("p is null");
            act.Add(p);
        }
        Assert.Equal("0,2;4,2;6,2;8,2", string.Join(";", sut.GetParts().Select(item => $"{item.Start},{item.Length}")));
        Assert.Equal("Heo Worl", string.Join("", sut.GetParts().Select(item => item.Text)));
    }

    [Fact]
    public void T004CreatePartOverlapUp() {
        var sut = new StringSpliceStringBuilder("Hello World");

        Assert.NotNull(sut.CreatePart(4, 4));
        Assert.Null(sut.CreatePart(4, 4));
        Assert.Null(sut.CreatePart(2, 4));
    }

    [Fact]
    public void T005CreatePartLength() {
        var sut = new StringSpliceStringBuilder("Hello World");
        Assert.Null(sut.CreatePart(4, 10));
    }
}
