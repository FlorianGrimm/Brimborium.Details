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
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="MarkdownLanguageParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.12.0")]
[System.CLSCompliant(false)]
public interface IMarkdownLanguageListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="MarkdownLanguageParser.markdownLanguage"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMarkdownLanguage([NotNull] MarkdownLanguageParser.MarkdownLanguageContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="MarkdownLanguageParser.markdownLanguage"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMarkdownLanguage([NotNull] MarkdownLanguageParser.MarkdownLanguageContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="MarkdownLanguageParser.markdownLine"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMarkdownLine([NotNull] MarkdownLanguageParser.MarkdownLineContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="MarkdownLanguageParser.markdownLine"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMarkdownLine([NotNull] MarkdownLanguageParser.MarkdownLineContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="MarkdownLanguageParser.mdAnchor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMdAnchor([NotNull] MarkdownLanguageParser.MdAnchorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="MarkdownLanguageParser.mdAnchor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMdAnchor([NotNull] MarkdownLanguageParser.MdAnchorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="MarkdownLanguageParser.mdReference"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMdReference([NotNull] MarkdownLanguageParser.MdReferenceContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="MarkdownLanguageParser.mdReference"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMdReference([NotNull] MarkdownLanguageParser.MdReferenceContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="MarkdownLanguageParser.mdCommand"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMdCommand([NotNull] MarkdownLanguageParser.MdCommandContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="MarkdownLanguageParser.mdCommand"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMdCommand([NotNull] MarkdownLanguageParser.MdCommandContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="MarkdownLanguageParser.paragraphAnchor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParagraphAnchor([NotNull] MarkdownLanguageParser.ParagraphAnchorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="MarkdownLanguageParser.paragraphAnchor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParagraphAnchor([NotNull] MarkdownLanguageParser.ParagraphAnchorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="MarkdownLanguageParser.paragraphReference"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParagraphReference([NotNull] MarkdownLanguageParser.ParagraphReferenceContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="MarkdownLanguageParser.paragraphReference"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParagraphReference([NotNull] MarkdownLanguageParser.ParagraphReferenceContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="MarkdownLanguageParser.paragraphCommand"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParagraphCommand([NotNull] MarkdownLanguageParser.ParagraphCommandContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="MarkdownLanguageParser.paragraphCommand"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParagraphCommand([NotNull] MarkdownLanguageParser.ParagraphCommandContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="MarkdownLanguageParser.anchor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAnchor([NotNull] MarkdownLanguageParser.AnchorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="MarkdownLanguageParser.anchor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAnchor([NotNull] MarkdownLanguageParser.AnchorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="MarkdownLanguageParser.reference"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterReference([NotNull] MarkdownLanguageParser.ReferenceContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="MarkdownLanguageParser.reference"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitReference([NotNull] MarkdownLanguageParser.ReferenceContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="MarkdownLanguageParser.contentPath"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterContentPath([NotNull] MarkdownLanguageParser.ContentPathContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="MarkdownLanguageParser.contentPath"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitContentPath([NotNull] MarkdownLanguageParser.ContentPathContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="MarkdownLanguageParser.filePath"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFilePath([NotNull] MarkdownLanguageParser.FilePathContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="MarkdownLanguageParser.filePath"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFilePath([NotNull] MarkdownLanguageParser.FilePathContext context);
}
