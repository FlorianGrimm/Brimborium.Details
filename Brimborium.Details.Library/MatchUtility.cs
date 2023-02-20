namespace Brimborium.Details;

public static class MatchUtility {
    //private System.Text.RegularExpressions.Regex _Regex = new System.Text.RegularExpressions.Regex(@"§(>?)\s*([^\s]+)\s*§\s*([^\s]+)\s*§\s*([^\s]+)\s*§");

    private static char[] arrCharParagraph = new char[] { '§' };

    public static MatchInfo? parseMatchIfMatches(string value) {
        if (!value.Contains('§')) { return null; }
        string normalized = value.Trim();
        if (!normalized.StartsWith("§")) {
            return null;
        }
        return parseMatch(value);
    }
    
    public static MatchInfo parseMatch(string value) {
        // § Syntax Marker
        var arr = value.Split(arrCharParagraph);
        arr = arr.Select(
            item => item
                .Trim())
                .Where((item, idx)=>(idx==0)?(!string.IsNullOrEmpty(item)):true)
                .ToArray();        
        bool isCommand = false;        
        if (arr.Length > 0) {
            if (arr[0].StartsWith(">")) {
                arr[0] = arr[0].Substring(1).Trim();
                isCommand = true;
            }
        }
        var result = new MatchInfo(value, isCommand, arr);
        return result;
    }
}