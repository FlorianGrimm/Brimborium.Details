grammar DetailsLanguage;	

import DetailsLexer;

/* § #abc § */
paragraphAnchor: Paragraph anchor Paragraph?;
/* § abc/def#/ghi § */
paragraphReference: Paragraph reference paragraphComment? Paragraph?;
paragraphComment: Paragraph -> pushMode(Comment) ;
/*comment */
/*comment: */
paragraphCommand: ParagraphGreater CommandName  anchor?  Paragraph?;

/* #/ghi */
anchor: contentPath;

/* abc/def#/ghi */
reference: filePath contentPath?;

/* #/ghi */
contentPath: Hash Slash? (FileName Slash)* FileName;

/* abc/def */
filePath: (FileName DirectorySeperator)* FileName;
