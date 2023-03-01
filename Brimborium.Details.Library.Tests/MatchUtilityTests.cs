namespace Brimborium.Details.Tests;

public class MatchUtilityTests {
    [Fact]
    public void T0001MatchUtility() {
        var location = PathInfo.Create("other.md", "#/definition");
        {
            var comment1 = MatchUtility.parseMatch(
            "// § todo.md", location, 0, 0);

            Assert.NotNull(comment1);
            Assert.Equal("todo.md", comment1.Path.ToString());
            Assert.Equal("", comment1.Command);
            Assert.Equal("", comment1.Comment);

        }
        {
            var m1 = MatchUtility.parseMatch(
                "§ Syntax-Marker.md / Syntax Marker", location, 0, 0);

            Assert.NotNull(m1);
            Assert.Equal("Syntax-Marker.md / Syntax Marker", m1.Path.ToString());
            Assert.Equal("", m1.Command);
            Assert.Equal("", m1.Comment);
        }

        {
            var m2 = MatchUtility.parseMatch(
                "§ Syntax-Marker.md / Syntax Marker § Comment", location, 0, 0);
        }

        {
            var m3 = MatchUtility.parseMatch(
            "§ Syntax-Marker.md / Syntax Marker § Comment §", location, 0, 0);
        }

        {

            var m4 = MatchUtility.parseMatch(
                "§ Syntax-Marker.md / Syntax Marker # 5", location, 0, 0);
        }

        {
            var m5 = MatchUtility.parseMatch(
            "§ Syntax-Marker.md / Syntax Marker # 10 §", location, 0, 0);
        }

        {
            var m6 = MatchUtility.parseMatch(
                "§ Syntax-Marker.md / Syntax Marker # 5 § Comment", location, 0, 0);
        }

        {
            var m7 = MatchUtility.parseMatch(
                "§ Syntax-Marker.md / Syntax Marker # 10 § Comment §", location, 0, 0);
        }

        {
            var c1 = MatchUtility.parseMatch(
               "§> Call-Command", location, 0, 0);

            Assert.NotNull(c1);
            Assert.Equal("Call-Command", c1.Command);
            Assert.Equal("", c1.Path.ToString());
        }

        {
            var c2 = MatchUtility.parseMatch(
               "§> Call-Command Syntax-Marker.md / Syntax Marker", location, 0, 0);

            Assert.NotNull(c2);
            Assert.Equal("Call-Command", c2.Command);
            // TODO Assert.Equal("Syntax-Marker.md / Syntax Marker", c2.Path);
        }

        {
            var c3 = MatchUtility.parseMatch(
               "§> Show-List todo.md", location, 0, 0);

            Assert.NotNull(c3);
            Assert.Equal("Show-List", c3.Command);
            Assert.Equal("todo.md", c3.Path.ToString());
            // TODO Assert.Equal("Syntax-Marker.md / Syntax Marker", c2.Path);
        }

        // 

        {
            var s1 = MatchUtility.parseMatch(
               "// § todo.md", location, 0, 0);

            Assert.NotNull(s1);
            Assert.Equal(MatchInfoKind.Paragraph, s1.Kind);
        }
    }
}