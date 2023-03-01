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
using IErrorNode = Antlr4.Runtime.Tree.IErrorNode;
using ITerminalNode = Antlr4.Runtime.Tree.ITerminalNode;
using IToken = Antlr4.Runtime.IToken;
using ParserRuleContext = Antlr4.Runtime.ParserRuleContext;

/// <summary>
/// This class provides an empty implementation of <see cref="IMarkdownLanguageListener"/>,
/// which can be extended to create a listener which only needs to handle a subset
/// of the available methods.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.12.0")]
[System.Diagnostics.DebuggerNonUserCode]
[System.CLSCompliant(false)]
public partial class MarkdownLanguageBaseListener : IMarkdownLanguageListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="MarkdownLanguageParser.markdownLanguage"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMarkdownLanguage([NotNull] MarkdownLanguageParser.MarkdownLanguageContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MarkdownLanguageParser.markdownLanguage"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMarkdownLanguage([NotNull] MarkdownLanguageParser.MarkdownLanguageContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MarkdownLanguageParser.markdownLine"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMarkdownLine([NotNull] MarkdownLanguageParser.MarkdownLineContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MarkdownLanguageParser.markdownLine"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMarkdownLine([NotNull] MarkdownLanguageParser.MarkdownLineContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MarkdownLanguageParser.mdAnchor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMdAnchor([NotNull] MarkdownLanguageParser.MdAnchorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MarkdownLanguageParser.mdAnchor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMdAnchor([NotNull] MarkdownLanguageParser.MdAnchorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MarkdownLanguageParser.mdReference"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMdReference([NotNull] MarkdownLanguageParser.MdReferenceContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MarkdownLanguageParser.mdReference"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMdReference([NotNull] MarkdownLanguageParser.MdReferenceContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MarkdownLanguageParser.mdCommand"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMdCommand([NotNull] MarkdownLanguageParser.MdCommandContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MarkdownLanguageParser.mdCommand"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMdCommand([NotNull] MarkdownLanguageParser.MdCommandContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MarkdownLanguageParser.paragraphAnchor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterParagraphAnchor([NotNull] MarkdownLanguageParser.ParagraphAnchorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MarkdownLanguageParser.paragraphAnchor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitParagraphAnchor([NotNull] MarkdownLanguageParser.ParagraphAnchorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MarkdownLanguageParser.paragraphReference"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterParagraphReference([NotNull] MarkdownLanguageParser.ParagraphReferenceContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MarkdownLanguageParser.paragraphReference"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitParagraphReference([NotNull] MarkdownLanguageParser.ParagraphReferenceContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MarkdownLanguageParser.paragraphCommand"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterParagraphCommand([NotNull] MarkdownLanguageParser.ParagraphCommandContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MarkdownLanguageParser.paragraphCommand"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitParagraphCommand([NotNull] MarkdownLanguageParser.ParagraphCommandContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MarkdownLanguageParser.anchor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAnchor([NotNull] MarkdownLanguageParser.AnchorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MarkdownLanguageParser.anchor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAnchor([NotNull] MarkdownLanguageParser.AnchorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MarkdownLanguageParser.reference"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterReference([NotNull] MarkdownLanguageParser.ReferenceContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MarkdownLanguageParser.reference"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitReference([NotNull] MarkdownLanguageParser.ReferenceContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MarkdownLanguageParser.contentPath"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterContentPath([NotNull] MarkdownLanguageParser.ContentPathContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MarkdownLanguageParser.contentPath"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitContentPath([NotNull] MarkdownLanguageParser.ContentPathContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MarkdownLanguageParser.filePath"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFilePath([NotNull] MarkdownLanguageParser.FilePathContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MarkdownLanguageParser.filePath"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFilePath([NotNull] MarkdownLanguageParser.FilePathContext context) { }

	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void EnterEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void ExitEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitTerminal([NotNull] ITerminalNode node) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitErrorNode([NotNull] IErrorNode node) { }
}