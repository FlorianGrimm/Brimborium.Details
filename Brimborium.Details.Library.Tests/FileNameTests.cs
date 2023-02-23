namespace Brimborium.Details.Library.Tests;

public class FileNameTests {
    [Fact]
    public void T001FileNameCtor() {
        var rootFolder = new FileName() {
            AbsolutePath = @"C:\root",
        };

        var test1 = new FileName() {
            RootFolder = rootFolder,
            RelativePath = @"abc\def",
        };
        Assert.Equal(@"abc/def", test1.RelativePath);
        Assert.Equal(@"C:\root\abc\def", test1.AbsolutePath);

        var test2 = new FileName() {
            AbsolutePath = @"C:\root\abc\def",
            RootFolder = rootFolder
        };
        Assert.Equal(@"abc/def", test2.RelativePath);
        Assert.Equal(@"C:\root\abc\def", test2.AbsolutePath);
    }

    [Fact]
    public void T002FileNameRebase() {

        var rootFolder1 = new FileName() {
            AbsolutePath = @"C:\root",
        };

        var test1 = new FileName() {
            RootFolder = rootFolder1,
            RelativePath = @"abc\def",
        };
        Assert.Equal(@"abc/def", test1.RelativePath);
        Assert.Equal(@"C:\root\abc\def", test1.AbsolutePath);

        var test2 = new FileName() {
            AbsolutePath = @"C:\root\abc\def",
            RootFolder = rootFolder1
        };

        Assert.Equal(@"abc/def", test2.RelativePath);
        Assert.Equal(@"C:\root\abc\def", test2.AbsolutePath);

        var rootFolder2 = new FileName() {
            AbsolutePath = @"C:\fourtwo",
        };
        var test1R = test1.Rebase(rootFolder2);
        Assert.NotNull(test1R);
        Assert.Equal(@"../root/abc/def", test1R.RelativePath);
        Assert.Equal(@"C:\root\abc\def", test1R.AbsolutePath);

        var test2R = test2.Rebase(rootFolder2);
        Assert.NotNull(test2R);
        Assert.Equal(@"../root/abc/def", test2R.RelativePath);
        Assert.Equal(@"C:\root\abc\def", test2R.AbsolutePath);

        Assert.True(test1R.Equals(test2R));

        Assert.False(test1.Equals(test1R));
    }

    [Fact]
    public void T003FileNameGetHashCode() {
        Assert.Equal(new FileName(
        ) {
            AbsolutePath = @"C:\root\abc\def",
        }.GetHashCode(), new FileName() {
            AbsolutePath = @"C:\root\abc\def",
        }.GetHashCode());

        Assert.NotEqual(new FileName(
        ) {
            AbsolutePath = @"C:\root\abc\defg",
        }.GetHashCode(), new FileName() {
            AbsolutePath = @"C:\root\abc\def",
        }.GetHashCode());
    }
}

