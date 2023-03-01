namespace Brimborium.Details.Library.Tests;

public class PathInfoTests {
    [Fact]
    public void PathInfo_Parse() {
        {
            var act = PathInfo.Parse("a/b/c.md");
            Assert.Equal("a/b/c.md", act.LogicalPath);
            Assert.Equal("a/b/c.md", act.FilePath);
            Assert.Equal("", act.ContentPath);
            Assert.Equal(0, act.ContentLevel);
        }

        {
            var act = PathInfo.Parse("a/b/c.md#/d/e/f");
            Assert.Equal("a/b/c.md#/d/e/f", act.LogicalPath);
            Assert.Equal("a/b/c.md", act.FilePath);
            Assert.Equal("/d/e/f", act.ContentPath);
            Assert.Equal(3, act.ContentLevel);
        }

        {
            var act = PathInfo.Parse("a/b.md#/c/d/e/f");
            Assert.Equal("a/b.md#/c/d/e/f", act.LogicalPath);
            Assert.Equal("a/b.md", act.FilePath);
            Assert.Equal("/c/d/e/f", act.ContentPath);
            Assert.Equal(4, act.ContentLevel);
        }

        {
            var act = PathInfo.Parse("a/b/c.md#d/e/f");
            Assert.Equal("a/b/c.md#/d/e/f", act.LogicalPath);
            Assert.Equal("a/b/c.md", act.FilePath);
            Assert.Equal("/d/e/f", act.ContentPath);
        }

        {
            var act = PathInfo.Parse("");
            Assert.Equal("", act.LogicalPath);
            Assert.Equal("", act.FilePath);
            Assert.Equal("", act.ContentPath);
            Assert.Equal(0, act.ContentLevel);
        }
    }
}
