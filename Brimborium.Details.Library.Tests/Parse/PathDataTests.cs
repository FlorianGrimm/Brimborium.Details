namespace Brimborium.Details.Parse;

public class PathDataTests {
    [Fact]
    public void PathInfo_ParseFilePath() {
        {
            var act = PathData.Parse("a/b/c.md");
            Assert.Equal("a/b/c.md##", act.LogicalPath);
            Assert.Equal("a/b/c.md", act.FilePath);
            Assert.Equal("", act.ContentPath);
            Assert.Equal(0, act.ContentLevel);
            Assert.Equal(0, act.Line);
        }
    }

    [Fact]
    public void PathInfo_ParseFilePathContentPath() {
        {
            var act = PathData.Parse("a/b/c.md#/d/e/f");
            Assert.Equal("a/b/c.md##/d/e/f", act.LogicalPath);
            Assert.Equal("a/b/c.md", act.FilePath);
            Assert.Equal("/d/e/f", act.ContentPath);
            Assert.Equal(3, act.ContentLevel);
            Assert.Equal(0, act.Line);
        }

        {
            var act = PathData.Parse("a/b.md#/c/d/e/f");
            Assert.Equal("a/b.md##/c/d/e/f", act.LogicalPath);
            Assert.Equal("a/b.md", act.FilePath);
            Assert.Equal("/c/d/e/f", act.ContentPath);
            Assert.Equal(4, act.ContentLevel);
            Assert.Equal(0, act.Line);
        }

        {
            var act = PathData.Parse("a/b/c.md#d/e/f");
            Assert.Equal("a/b/c.md##/d/e/f", act.LogicalPath);
            Assert.Equal("a/b/c.md", act.FilePath);
            Assert.Equal("/d/e/f", act.ContentPath);
            Assert.Equal(0, act.Line);
        }
    }

    [Fact]
    public void PathInfo_ParseFilePathLineContentPath() {
        {
            var act = PathData.Parse("a/b/c.md#0#d/e/f");
            Assert.Equal("a/b/c.md##/d/e/f", act.LogicalPath);
            Assert.Equal("a/b/c.md", act.FilePath);
            Assert.Equal("/d/e/f", act.ContentPath);
            Assert.Equal(0, act.Line);
        }

        {
            var act = PathData.Parse("a/b/c.md#42#d/e/f");
            Assert.Equal("a/b/c.md#42#/d/e/f", act.LogicalPath);
            Assert.Equal("a/b/c.md", act.FilePath);
            Assert.Equal("/d/e/f", act.ContentPath);
            Assert.Equal(42, act.Line);
        }
    }

    [Fact]
    public void PathInfo_ParseEmpty() {
        {
            var act = PathData.Parse("");
            Assert.Equal("", act.LogicalPath);
            Assert.Equal("", act.FilePath);
            Assert.Equal("", act.ContentPath);
            Assert.Equal(0, act.ContentLevel);
        }
    }
}
