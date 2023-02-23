namespace Brimborium.Details.Tests;

public class MatchUtilityTests {
    [Fact]
    public void T0001MatchUtility() {
        {
            var m1 = MatchUtility.parseMatch(
                "§ Syntax-Marker.md / Syntax Marker");

            Assert.NotNull(m1);
            Assert.Equal("Syntax-Marker.md / Syntax Marker", m1.Path);
            Assert.Equal("", m1.Command);
            Assert.Equal("", m1.Comment);
        }

        {
            var m2 = MatchUtility.parseMatch(
                "§ Syntax-Marker.md / Syntax Marker § Comment");
        }

        {
            var m3 = MatchUtility.parseMatch(
            "§ Syntax-Marker.md / Syntax Marker § Comment §");
        }

        {

            var m4 = MatchUtility.parseMatch(
                "§ Syntax-Marker.md / Syntax Marker # 5");
        }

        {
            var m5 = MatchUtility.parseMatch(
            "§ Syntax-Marker.md / Syntax Marker # 10 §");
        }

        {
            var m6 = MatchUtility.parseMatch(
                "§ Syntax-Marker.md / Syntax Marker # 5 § Comment");
        }

        {
            var m7 = MatchUtility.parseMatch(
                "§ Syntax-Marker.md / Syntax Marker # 10 § Comment §");
        }

        {
            var c1 = MatchUtility.parseMatch(
               "§> Call-Command");

            Assert.NotNull(c1);
            Assert.Equal("Call-Command", c1.Command);
            Assert.Equal("", c1.Path);
        }

        {
            var c2 = MatchUtility.parseMatch(
               "§> Call-Command Syntax-Marker.md / Syntax Marker");

            Assert.NotNull(c2);
            Assert.Equal("Call-Command", c2.Command);
            // TODO Assert.Equal("Syntax-Marker.md / Syntax Marker", c2.Path);
        }
    }
}