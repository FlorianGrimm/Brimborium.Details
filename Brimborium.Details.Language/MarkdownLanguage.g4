grammar MarkdownLanguage;		
import DetailsLanguage;

markdownLanguage: (markdownLine NewLine)* (markdownLine NewLine?)? EOF;

markdownLine: 
    paragraphAnchor 
    | paragraphReference
    | paragraphCommand
    ;

mdAnchor: Paragraph  anchor  Paragraph?;
mdReference: Paragraph  anchor  Paragraph?;
mdCommand: ParagraphGreater CommandName  anchor?  Paragraph?;
WS: [ \t]+ -> skip;

/*| mdHeadings */
/*mdHeadings: MdHeaderSt NotNewLine; */ 
/* MdHeader16: '#'|'##'|'###'|'####'|'#####'|'######'; */
/*MdOther: NotParagraph NotNewLine;*/