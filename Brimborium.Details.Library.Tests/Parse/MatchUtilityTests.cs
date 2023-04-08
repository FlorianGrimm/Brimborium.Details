namespace Brimborium.Details.Parse;

public class MatchUtilityTests {
    [Fact]
    public void T0001MatchUtilityparseMatchParagraph() {
        var location = PathData.Create("other.md", 42, "#/definition");
        {
            var sut = MatchUtility.ParseMatch(
            "// § todo.md", location, new string[]{"//"}, 0, 0);

            Assert.NotNull(sut);
            Assert.Equal(MatchInfoKind.Paragraph, sut.Kind);
            Assert.Equal("other.md", sut.MatchPath.FilePath.ToString());
            Assert.Equal("/definition", sut.MatchPath.ContentPath.ToString());
            Assert.Equal("todo.md", sut.Path.FilePath.ToString());
            Assert.Equal("", sut.Path.ContentPath.ToString());
            Assert.Equal("", sut.Command);
            Assert.Equal("", sut.Comment);

        }
        {
            var sut = MatchUtility.ParseMatch(
                "§ Syntax-Marker.md#Syntax-Marker", location, new string[]{"//"}, 0, 0);

            Assert.NotNull(sut);
            Assert.Equal(MatchInfoKind.Paragraph, sut.Kind);
            //Assert.Equal("Syntax-Marker.md # / Syntax Marker", sut.Path.ToString());
            Assert.Equal("other.md", sut.MatchPath.FilePath.ToString());
            Assert.Equal("/definition", sut.MatchPath.ContentPath.ToString());
            Assert.Equal("Syntax-Marker.md", sut.Path.FilePath.ToString());
            Assert.Equal("/Syntax-Marker", sut.Path.ContentPath.ToString());
            Assert.Equal("", sut.Command);
            Assert.Equal("", sut.Comment);
        }

        {
            var sut = MatchUtility.ParseMatch(
                "§ Syntax-Marker.md#/Syntax-Marker § Comment", location, new string[]{"//"}, 0, 0);

            Assert.NotNull(sut);
            Assert.Equal(MatchInfoKind.Paragraph, sut.Kind);
            Assert.Equal("other.md", sut.MatchPath.FilePath.ToString());
            Assert.Equal("/definition", sut.MatchPath.ContentPath.ToString());
            Assert.Equal("Syntax-Marker.md", sut.Path.FilePath.ToString());
            Assert.Equal("/Syntax-Marker", sut.Path.ContentPath.ToString());
            Assert.Equal("", sut.Command);
            Assert.Equal("Comment", sut.Comment);
        }

        {
            var sut = MatchUtility.ParseMatch(
            "§ Syntax-Marker.md#/Syntax-Marker § Comment §", location, new string[]{"//"}, 0, 0);

            Assert.NotNull(sut);
            Assert.Equal(MatchInfoKind.Paragraph, sut.Kind);
            Assert.Equal("other.md", sut.MatchPath.FilePath.ToString());
            Assert.Equal("/definition", sut.MatchPath.ContentPath.ToString());
            Assert.Equal("Syntax-Marker.md", sut.Path.FilePath.ToString());
            Assert.Equal("/Syntax-Marker", sut.Path.ContentPath.ToString());
            Assert.Equal("", sut.Command);
            Assert.Equal("Comment §", sut.Comment);
        }

        //{

        //    var sut = MatchUtility.parseMatch(
        //        "§ Syntax-Marker.md#/Syntax-Marker # 5", location, new string[]{"//"}, 0, 0);
        //    Assert.NotNull(sut);
        //    Assert.Equal(MatchInfoKind.Paragraph, sut.Kind);
        //    Assert.Equal("other.md", sut.MatchPath.FilePath.ToString());
        //    Assert.Equal("/definition", sut.MatchPath.ContentPath.ToString());
        //    Assert.Equal("Syntax-Marker.md", sut.Path.FilePath.ToString());
        //    Assert.Equal("/Syntax-Marker", sut.Path.ContentPath.ToString());
        //    Assert.Equal("", sut.Command);
        //    Assert.Equal("", sut.Comment);
        //}

        //{
        //    var sut = MatchUtility.parseMatch(
        //    "§ Syntax-Marker.md#Syntax-Marker # 10 §", location, new string[]{"//"}, 0, 0);
        //    Assert.NotNull(sut);
        //    Assert.Equal(MatchInfoKind.Paragraph, sut.Kind);
        //    Assert.Equal("other.md", sut.MatchPath.FilePath.ToString());
        //    Assert.Equal("/definition", sut.MatchPath.ContentPath.ToString());
        //    Assert.Equal("Syntax-Marker.md", sut.Path.FilePath.ToString());
        //    Assert.Equal("/Syntax-Marker", sut.Path.ContentPath.ToString());
        //    Assert.Equal("", sut.Command);
        //    Assert.Equal("Comment", sut.Comment);
        //}

        //{
        //    var sut = MatchUtility.parseMatch(
        //        "§ Syntax-Marker.md#Syntax-Marker # 5 § Comment", location, new string[]{"//"}, 0, 0);
        //    Assert.NotNull(sut);
        //    Assert.Equal(MatchInfoKind.Paragraph, sut.Kind);
        //    Assert.Equal("other.md", sut.MatchPath.FilePath.ToString());
        //    Assert.Equal("/definition", sut.MatchPath.ContentPath.ToString());
        //    Assert.Equal("Syntax-Marker.md", sut.Path.FilePath.ToString());
        //    Assert.Equal("/Syntax-Marker", sut.Path.ContentPath.ToString());
        //    Assert.Equal("", sut.Command);
        //    Assert.Equal("", sut.Comment);
        //}

        //{
        //    var sut = MatchUtility.parseMatch(
        //        "§ Syntax-Marker.md#Syntax-Marker # 10 § Comment §", location, new string[]{"//"}, 0, 0);
        //    Assert.NotNull(sut);
        //    Assert.Equal(MatchInfoKind.Paragraph, sut.Kind);
        //    Assert.Equal("other.md", sut.MatchPath.FilePath.ToString());
        //    Assert.Equal("/definition", sut.MatchPath.ContentPath.ToString());
        //    Assert.Equal("Syntax-Marker.md", sut.Path.FilePath.ToString());
        //    Assert.Equal("/Syntax-Marker", sut.Path.ContentPath.ToString());
        //    Assert.Equal("", sut.Command);
        //    Assert.Equal("Comment", sut.Comment);
        //}
    }
    
    [Fact]
    public void T0002MatchUtilityparseMatchCommand() {
        var location = PathData.Create("other.md", 42, "#/definition");

        {
            var sut = MatchUtility.ParseMatch(
               "// §> Call-Command \r\n", location, new string[]{"//"}, 0, 0);

            Assert.NotNull(sut);
            Assert.Equal(MatchInfoKind.ParagraphCommand, sut.Kind);
            Assert.Equal("other.md", sut.MatchPath.FilePath.ToString());
            Assert.Equal("/definition", sut.MatchPath.ContentPath.ToString());
            
            Assert.Equal("", sut.Path.FilePath.ToString());
            Assert.Equal("", sut.Path.ContentPath.ToString());
            
            Assert.Equal("Call-Command", sut.Command);
            Assert.Equal("", sut.Comment);
        }

        {
            var sut = MatchUtility.ParseMatch(
               "§> Call-Command Syntax-Marker.md/Syntax-Marker", location, new string[]{"//"}, 0, 0);

            Assert.NotNull(sut);
            Assert.Equal(MatchInfoKind.ParagraphCommand, sut.Kind);
            Assert.Equal("other.md", sut.MatchPath.FilePath.ToString());
            Assert.Equal("/definition", sut.MatchPath.ContentPath.ToString());
            
            Assert.Equal("Syntax-Marker.md/Syntax-Marker", sut.Path.FilePath.ToString());
            Assert.Equal("", sut.Path.ContentPath.ToString());
            
            Assert.Equal("Call-Command", sut.Command);
            Assert.Equal("", sut.Comment);
        }

        {
            var sut = MatchUtility.ParseMatch(
               "§> Show-List todo.md", location, new string[]{"//"}, 0, 0);

            Assert.NotNull(sut);
            Assert.Equal("Show-List", sut.Command);
            Assert.Equal("todo.md##", sut.Path.ToString());
            Assert.Equal("", sut.Comment);
            // TODO Assert.Equal("Syntax-Marker.md / Syntax Marker", c2.Path);
        }

    }
    
    [Fact]
    public void T0003MatchUtilityparseMatchAnchor() {
        var location = PathData.Create("other.md", 42, "#/definition");
        {
            // § details/Syntax-Marker.md#Parse/Anchor test // §# todo.md
            var sut = MatchUtility.ParseMatch(
               "// §# todo.md", location, new string[]{"//"}, 0, 0);

            Assert.NotNull(sut);
            Assert.Equal(MatchInfoKind.Anchor, sut.Kind);
            Assert.Equal("other.md", sut.MatchPath.FilePath.ToString());
            Assert.Equal("/definition", sut.MatchPath.ContentPath.ToString());
            
            Assert.Equal("todo.md", sut.Path.FilePath.ToString());
            Assert.Equal("", sut.Path.ContentPath.ToString());
            
            Assert.Equal("", sut.Command);
            Assert.Equal("", sut.Comment);
        }
        {
            // § details/Syntax-Marker.md#Parse/Anchor test // §# todo.md comment
            var sut = MatchUtility.ParseMatch(
               "// §# todo.md comment", location, new string[]{"//"}, 0, 0);

            Assert.NotNull(sut);
            Assert.Equal(MatchInfoKind.Anchor, sut.Kind);
            Assert.Equal("other.md", sut.MatchPath.FilePath.ToString());
            Assert.Equal("/definition", sut.MatchPath.ContentPath.ToString());
            Assert.Equal("todo.md", sut.Path.FilePath.ToString());
            Assert.Equal("", sut.Path.ContentPath.ToString());
            Assert.Equal("", sut.Command);
            Assert.Equal("comment", sut.Comment);
        }
    }
}