namespace Brimborium.Details;

public class SubStringTests {
    [Fact]
    public void T001SubString() {
        var sut = new SubString("Hello World");
        Assert.Equal(new Index(0), sut.Range.Start);
        Assert.Equal(new Index(11), sut.Range.End);
        Assert.Equal("Hello World", sut.ToString());
    }

    [Fact]
    public void T002GetString() {
        var sut = new SubString("Hello World");
        Assert.Equal(new Index(0), sut.Range.Start);
        Assert.Equal(new Index(11), sut.Range.End);
        Assert.Equal("Hello World", sut.ToString());

        var sut22 = sut.GetSubString(2, 2);
        Assert.Equal("ll", sut22.ToString());

        
        var sut1_1 = sut.GetSubString(1, sut.Length - 2);
        Assert.Equal("ello Worl", sut1_1.ToString());

        var sut1_1_22 = sut1_1.GetSubString(2, 2);
        Assert.Equal("ll", sut1_1_22.ToString());
    }
}
