/*

%USERPROFILE%\AppData\Roaming\Python\Python39\Scripts/antlr4.exe
%USERPROFILE%\AppData\Roaming\Python\Python39\Scripts/antlr4.exe DetailsLanguage.g4 -o Generated -Dlanguage=CSharp
%USERPROFILE%\AppData\Roaming\Python\Python39\Scripts/antlr4.exe MarkdownLanguage.g4 -o Generated -Dlanguage=CSharp
%USERPROFILE%\AppData\Roaming\Python\Python39\Scripts/antlr4.exe CSharpLanguage.g4 -o Generated -Dlanguage=CSharp

%USERPROFILE%\AppData\Roaming\Python\Python39\Scripts/antlr4-parse MarkdownLanguage.g4 markdownLanguage -tree
§ Hello.md/World
antlr4-parse Expr.g4 prog -tokens -trace
*/
namespace Brimborium.Details.Language;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
public class Program
{
    public static void Main(string[] args)
    {
/*
            §#World
            § Hello.md#World
            §> Hello.md#World

*/        
        var input = new AntlrInputStream(
            @"
            §#World            
            "
            );
        var lexer = new MarkdownLanguageLexer(input);
        var tokens = new CommonTokenStream(lexer);
        var parser = new MarkdownLanguageParser(tokens);
        var tree = parser.markdownLanguage();
        //var tree = parser.mdAnchor();
        Console.WriteLine(tree.ToStringTree(parser));
    }
}