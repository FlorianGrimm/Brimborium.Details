//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.12.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from MarkdownLanguage.g4 by ANTLR 4.12.0

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="MarkdownLanguageParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.12.0")]
[System.CLSCompliant(false)]
public interface IMarkdownLanguageVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="MarkdownLanguageParser.markdownLanguage"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMarkdownLanguage([NotNull] MarkdownLanguageParser.MarkdownLanguageContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MarkdownLanguageParser.markdownLine"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMarkdownLine([NotNull] MarkdownLanguageParser.MarkdownLineContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MarkdownLanguageParser.mdAnchor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMdAnchor([NotNull] MarkdownLanguageParser.MdAnchorContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MarkdownLanguageParser.mdReference"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMdReference([NotNull] MarkdownLanguageParser.MdReferenceContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MarkdownLanguageParser.mdCommand"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMdCommand([NotNull] MarkdownLanguageParser.MdCommandContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MarkdownLanguageParser.paragraphAnchor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitParagraphAnchor([NotNull] MarkdownLanguageParser.ParagraphAnchorContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MarkdownLanguageParser.paragraphReference"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitParagraphReference([NotNull] MarkdownLanguageParser.ParagraphReferenceContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MarkdownLanguageParser.paragraphCommand"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitParagraphCommand([NotNull] MarkdownLanguageParser.ParagraphCommandContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MarkdownLanguageParser.anchor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAnchor([NotNull] MarkdownLanguageParser.AnchorContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MarkdownLanguageParser.reference"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitReference([NotNull] MarkdownLanguageParser.ReferenceContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MarkdownLanguageParser.contentPath"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitContentPath([NotNull] MarkdownLanguageParser.ContentPathContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MarkdownLanguageParser.filePath"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFilePath([NotNull] MarkdownLanguageParser.FilePathContext context);
}