namespace Brimborium.Details;

public class StringSpliceStringBuilderTests {
    [Fact]
    public void T001BuildReplacement() {
        var sut = new StringSpliceStringBuilder("Hello World");
        var part = sut.CreatePart(1, 1);
        if (part is null) { throw new Exception("part is null"); }
        part.GetReplacementBuilder().Append("a");
        Assert.Equal("Hallo World", sut.BuildReplacement());
        var part2 = sut.CreatePart(sut.Length, 0);
        if (part2 is null) { throw new Exception("part2 is null"); }
        part2.GetReplacementBuilder().Append("!");
        Assert.Equal("Hallo World!", sut.BuildReplacement());
    }
}