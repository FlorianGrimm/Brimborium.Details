lexer grammar DetailsLexer;

CommandName: Letters Minus Letters;
Letters: [A-Za-z]+;
Minus : '-' ;
FileName : [-.0-9A-Za-z_]+ ;
Slash : '/' ;
SlashSlash : '//' ;
DirectorySeperator : [\\/] ;
MarkdownExtension : '.md' ;
CSharpExtension : '.cs' ;
HTMLExtension : '.html' ;
Hash : [#] ;
Paragraph : [§] ;
NotParagraph: ~[§] ;
ParagraphGreater : '§>' ;
NewLine : [\r\n]+;
WhiteSpace : [ \t]+;

mode COMMENT;
Comment: ~[\r\n§]+;

fragment InputCharacter:       ~[\r\n\u0085\u2028\u2029];
