grammar CSharpLanguage;
import DetailsLanguage;

csharpLanguage: (csharpSingleLineComment NewLine)* (csharpSingleLineComment NewLine?)? EOF;

csharpContent: 
    csharpSingleLineComment NewLine
    ;
csharpSingleLineComment
    : SlashSlash paragraphAnchor
    | SlashSlash paragraphReference
    | SlashSlash paragraphCommand
    ;
