//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.12.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from DetailsLanguage.g4 by ANTLR 4.12.0

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.12.0")]
[System.CLSCompliant(false)]
public partial class DetailsLanguageParser : Parser {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		CommandName=1, Letters=2, Minus=3, FileName=4, Slash=5, SlashSlash=6, 
		DirectorySeperator=7, MarkdownExtension=8, CSharpExtension=9, HTMLExtension=10, 
		Hash=11, Paragraph=12, NotParagraph=13, ParagraphGreater=14, NewLine=15, 
		WhiteSpace=16;
	public const int
		RULE_paragraphAnchor = 0, RULE_paragraphReference = 1, RULE_paragraphCommand = 2, 
		RULE_anchor = 3, RULE_reference = 4, RULE_contentPath = 5, RULE_filePath = 6;
	public static readonly string[] ruleNames = {
		"paragraphAnchor", "paragraphReference", "paragraphCommand", "anchor", 
		"reference", "contentPath", "filePath"
	};

	private static readonly string[] _LiteralNames = {
		null, null, null, "'-'", null, "'/'", "'//'", null, "'.md'", "'.cs'", 
		"'.html'", null, null, null, "'\\u00A7>'"
	};
	private static readonly string[] _SymbolicNames = {
		null, "CommandName", "Letters", "Minus", "FileName", "Slash", "SlashSlash", 
		"DirectorySeperator", "MarkdownExtension", "CSharpExtension", "HTMLExtension", 
		"Hash", "Paragraph", "NotParagraph", "ParagraphGreater", "NewLine", "WhiteSpace"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "DetailsLanguage.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override int[] SerializedAtn { get { return _serializedATN; } }

	static DetailsLanguageParser() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}

		public DetailsLanguageParser(ITokenStream input) : this(input, Console.Out, Console.Error) { }

		public DetailsLanguageParser(ITokenStream input, TextWriter output, TextWriter errorOutput)
		: base(input, output, errorOutput)
	{
		Interpreter = new ParserATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	public partial class ParagraphAnchorContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode[] Paragraph() { return GetTokens(DetailsLanguageParser.Paragraph); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode Paragraph(int i) {
			return GetToken(DetailsLanguageParser.Paragraph, i);
		}
		[System.Diagnostics.DebuggerNonUserCode] public AnchorContext anchor() {
			return GetRuleContext<AnchorContext>(0);
		}
		public ParagraphAnchorContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_paragraphAnchor; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IDetailsLanguageListener typedListener = listener as IDetailsLanguageListener;
			if (typedListener != null) typedListener.EnterParagraphAnchor(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IDetailsLanguageListener typedListener = listener as IDetailsLanguageListener;
			if (typedListener != null) typedListener.ExitParagraphAnchor(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IDetailsLanguageVisitor<TResult> typedVisitor = visitor as IDetailsLanguageVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitParagraphAnchor(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public ParagraphAnchorContext paragraphAnchor() {
		ParagraphAnchorContext _localctx = new ParagraphAnchorContext(Context, State);
		EnterRule(_localctx, 0, RULE_paragraphAnchor);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 14;
			Match(Paragraph);
			State = 15;
			anchor();
			State = 17;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			if (_la==Paragraph) {
				{
				State = 16;
				Match(Paragraph);
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class ParagraphReferenceContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode[] Paragraph() { return GetTokens(DetailsLanguageParser.Paragraph); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode Paragraph(int i) {
			return GetToken(DetailsLanguageParser.Paragraph, i);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ReferenceContext reference() {
			return GetRuleContext<ReferenceContext>(0);
		}
		public ParagraphReferenceContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_paragraphReference; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IDetailsLanguageListener typedListener = listener as IDetailsLanguageListener;
			if (typedListener != null) typedListener.EnterParagraphReference(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IDetailsLanguageListener typedListener = listener as IDetailsLanguageListener;
			if (typedListener != null) typedListener.ExitParagraphReference(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IDetailsLanguageVisitor<TResult> typedVisitor = visitor as IDetailsLanguageVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitParagraphReference(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public ParagraphReferenceContext paragraphReference() {
		ParagraphReferenceContext _localctx = new ParagraphReferenceContext(Context, State);
		EnterRule(_localctx, 2, RULE_paragraphReference);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 19;
			Match(Paragraph);
			State = 20;
			reference();
			State = 22;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			if (_la==Paragraph) {
				{
				State = 21;
				Match(Paragraph);
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class ParagraphCommandContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode ParagraphGreater() { return GetToken(DetailsLanguageParser.ParagraphGreater, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode CommandName() { return GetToken(DetailsLanguageParser.CommandName, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public AnchorContext anchor() {
			return GetRuleContext<AnchorContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode Paragraph() { return GetToken(DetailsLanguageParser.Paragraph, 0); }
		public ParagraphCommandContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_paragraphCommand; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IDetailsLanguageListener typedListener = listener as IDetailsLanguageListener;
			if (typedListener != null) typedListener.EnterParagraphCommand(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IDetailsLanguageListener typedListener = listener as IDetailsLanguageListener;
			if (typedListener != null) typedListener.ExitParagraphCommand(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IDetailsLanguageVisitor<TResult> typedVisitor = visitor as IDetailsLanguageVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitParagraphCommand(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public ParagraphCommandContext paragraphCommand() {
		ParagraphCommandContext _localctx = new ParagraphCommandContext(Context, State);
		EnterRule(_localctx, 4, RULE_paragraphCommand);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 24;
			Match(ParagraphGreater);
			State = 25;
			Match(CommandName);
			State = 27;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			if (_la==Hash) {
				{
				State = 26;
				anchor();
				}
			}

			State = 30;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			if (_la==Paragraph) {
				{
				State = 29;
				Match(Paragraph);
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class AnchorContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ContentPathContext contentPath() {
			return GetRuleContext<ContentPathContext>(0);
		}
		public AnchorContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_anchor; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IDetailsLanguageListener typedListener = listener as IDetailsLanguageListener;
			if (typedListener != null) typedListener.EnterAnchor(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IDetailsLanguageListener typedListener = listener as IDetailsLanguageListener;
			if (typedListener != null) typedListener.ExitAnchor(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IDetailsLanguageVisitor<TResult> typedVisitor = visitor as IDetailsLanguageVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitAnchor(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public AnchorContext anchor() {
		AnchorContext _localctx = new AnchorContext(Context, State);
		EnterRule(_localctx, 6, RULE_anchor);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 32;
			contentPath();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class ReferenceContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public FilePathContext filePath() {
			return GetRuleContext<FilePathContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ContentPathContext contentPath() {
			return GetRuleContext<ContentPathContext>(0);
		}
		public ReferenceContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_reference; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IDetailsLanguageListener typedListener = listener as IDetailsLanguageListener;
			if (typedListener != null) typedListener.EnterReference(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IDetailsLanguageListener typedListener = listener as IDetailsLanguageListener;
			if (typedListener != null) typedListener.ExitReference(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IDetailsLanguageVisitor<TResult> typedVisitor = visitor as IDetailsLanguageVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitReference(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public ReferenceContext reference() {
		ReferenceContext _localctx = new ReferenceContext(Context, State);
		EnterRule(_localctx, 8, RULE_reference);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 34;
			filePath();
			State = 36;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			if (_la==Hash) {
				{
				State = 35;
				contentPath();
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class ContentPathContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode Hash() { return GetToken(DetailsLanguageParser.Hash, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode[] FileName() { return GetTokens(DetailsLanguageParser.FileName); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode FileName(int i) {
			return GetToken(DetailsLanguageParser.FileName, i);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode[] Slash() { return GetTokens(DetailsLanguageParser.Slash); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode Slash(int i) {
			return GetToken(DetailsLanguageParser.Slash, i);
		}
		public ContentPathContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_contentPath; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IDetailsLanguageListener typedListener = listener as IDetailsLanguageListener;
			if (typedListener != null) typedListener.EnterContentPath(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IDetailsLanguageListener typedListener = listener as IDetailsLanguageListener;
			if (typedListener != null) typedListener.ExitContentPath(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IDetailsLanguageVisitor<TResult> typedVisitor = visitor as IDetailsLanguageVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitContentPath(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public ContentPathContext contentPath() {
		ContentPathContext _localctx = new ContentPathContext(Context, State);
		EnterRule(_localctx, 10, RULE_contentPath);
		int _la;
		try {
			int _alt;
			EnterOuterAlt(_localctx, 1);
			{
			State = 38;
			Match(Hash);
			State = 40;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			if (_la==Slash) {
				{
				State = 39;
				Match(Slash);
				}
			}

			State = 46;
			ErrorHandler.Sync(this);
			_alt = Interpreter.AdaptivePredict(TokenStream,6,Context);
			while ( _alt!=2 && _alt!=global::Antlr4.Runtime.Atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					State = 42;
					Match(FileName);
					State = 43;
					Match(Slash);
					}
					} 
				}
				State = 48;
				ErrorHandler.Sync(this);
				_alt = Interpreter.AdaptivePredict(TokenStream,6,Context);
			}
			State = 49;
			Match(FileName);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class FilePathContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode[] FileName() { return GetTokens(DetailsLanguageParser.FileName); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode FileName(int i) {
			return GetToken(DetailsLanguageParser.FileName, i);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode[] DirectorySeperator() { return GetTokens(DetailsLanguageParser.DirectorySeperator); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode DirectorySeperator(int i) {
			return GetToken(DetailsLanguageParser.DirectorySeperator, i);
		}
		public FilePathContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_filePath; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IDetailsLanguageListener typedListener = listener as IDetailsLanguageListener;
			if (typedListener != null) typedListener.EnterFilePath(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IDetailsLanguageListener typedListener = listener as IDetailsLanguageListener;
			if (typedListener != null) typedListener.ExitFilePath(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IDetailsLanguageVisitor<TResult> typedVisitor = visitor as IDetailsLanguageVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitFilePath(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public FilePathContext filePath() {
		FilePathContext _localctx = new FilePathContext(Context, State);
		EnterRule(_localctx, 12, RULE_filePath);
		try {
			int _alt;
			EnterOuterAlt(_localctx, 1);
			{
			State = 55;
			ErrorHandler.Sync(this);
			_alt = Interpreter.AdaptivePredict(TokenStream,7,Context);
			while ( _alt!=2 && _alt!=global::Antlr4.Runtime.Atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					State = 51;
					Match(FileName);
					State = 52;
					Match(DirectorySeperator);
					}
					} 
				}
				State = 57;
				ErrorHandler.Sync(this);
				_alt = Interpreter.AdaptivePredict(TokenStream,7,Context);
			}
			State = 58;
			Match(FileName);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	private static int[] _serializedATN = {
		4,1,16,61,2,0,7,0,2,1,7,1,2,2,7,2,2,3,7,3,2,4,7,4,2,5,7,5,2,6,7,6,1,0,
		1,0,1,0,3,0,18,8,0,1,1,1,1,1,1,3,1,23,8,1,1,2,1,2,1,2,3,2,28,8,2,1,2,3,
		2,31,8,2,1,3,1,3,1,4,1,4,3,4,37,8,4,1,5,1,5,3,5,41,8,5,1,5,1,5,5,5,45,
		8,5,10,5,12,5,48,9,5,1,5,1,5,1,6,1,6,5,6,54,8,6,10,6,12,6,57,9,6,1,6,1,
		6,1,6,0,0,7,0,2,4,6,8,10,12,0,0,61,0,14,1,0,0,0,2,19,1,0,0,0,4,24,1,0,
		0,0,6,32,1,0,0,0,8,34,1,0,0,0,10,38,1,0,0,0,12,55,1,0,0,0,14,15,5,12,0,
		0,15,17,3,6,3,0,16,18,5,12,0,0,17,16,1,0,0,0,17,18,1,0,0,0,18,1,1,0,0,
		0,19,20,5,12,0,0,20,22,3,8,4,0,21,23,5,12,0,0,22,21,1,0,0,0,22,23,1,0,
		0,0,23,3,1,0,0,0,24,25,5,14,0,0,25,27,5,1,0,0,26,28,3,6,3,0,27,26,1,0,
		0,0,27,28,1,0,0,0,28,30,1,0,0,0,29,31,5,12,0,0,30,29,1,0,0,0,30,31,1,0,
		0,0,31,5,1,0,0,0,32,33,3,10,5,0,33,7,1,0,0,0,34,36,3,12,6,0,35,37,3,10,
		5,0,36,35,1,0,0,0,36,37,1,0,0,0,37,9,1,0,0,0,38,40,5,11,0,0,39,41,5,5,
		0,0,40,39,1,0,0,0,40,41,1,0,0,0,41,46,1,0,0,0,42,43,5,4,0,0,43,45,5,5,
		0,0,44,42,1,0,0,0,45,48,1,0,0,0,46,44,1,0,0,0,46,47,1,0,0,0,47,49,1,0,
		0,0,48,46,1,0,0,0,49,50,5,4,0,0,50,11,1,0,0,0,51,52,5,4,0,0,52,54,5,7,
		0,0,53,51,1,0,0,0,54,57,1,0,0,0,55,53,1,0,0,0,55,56,1,0,0,0,56,58,1,0,
		0,0,57,55,1,0,0,0,58,59,5,4,0,0,59,13,1,0,0,0,8,17,22,27,30,36,40,46,55
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
