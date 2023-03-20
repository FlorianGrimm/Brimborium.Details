namespace Brimborium.Details.Parse;

[Transient]
public class MarkdownService {
    private readonly IFileSystem _FileSystem;
    private readonly IServiceProvider _ServiceProvider;
    private readonly MarkdownPipeline _Pipeline;
    private readonly List<IMatchCommand> _LstMatchCommand;

    public MarkdownService(
        IFileSystem fileSystem,
        IServiceProvider serviceProvider
        ) {
        this._FileSystem = fileSystem;
        this._ServiceProvider = serviceProvider;
        this._Pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .EnableTrackTrivia()
            .Build();
        this._LstMatchCommand = serviceProvider.GetServices<IMatchCommand>().ToList();
    }
    public Task<MarkdownContext?> PrepareSolutionDetail(
        IParserSinkContext parserSinkContext,
        CancellationToken cancellationToken) {
        var projectInfo = parserSinkContext.GetOrAddDetailsProject(null);
        return Task.FromResult<MarkdownContext?>(new MarkdownContext(projectInfo));
    }

    public async Task ParseDetail(
        IParserSinkContext parserSinkContext,
        MarkdownContext markdownContext,
        CancellationToken cancellationToken) {
        // System.Console.Out.WriteLine($"DetailsFolder: {SolutionInfo.DetailsFolder}");
        var solutionInfo = parserSinkContext.SolutionData;
        var lstMarkdownFile = this._FileSystem.EnumerateFiles(
                solutionInfo.DetailsFolder,
                "*.md",
                SearchOption.AllDirectories);
        if (lstMarkdownFile is null || !lstMarkdownFile.Any()) {
            Console.Out.WriteLine($"DetailsFolder: {solutionInfo.DetailsFolder} contains no *.md files");
            return;
        }
        var listDocumentInfo = new List<MarkdownDocumentInfo>();
        await ParallelUtility.ForEachAsync(
            lstMarkdownFile,
            cancellationToken,
            async (markdownFile, cancellationToken) => {
                var documentInfo = await this.ParseMarkdownFile(
                    parserSinkContext, markdownFile, cancellationToken);
                
                if (documentInfo is not null) {
                    lock (listDocumentInfo) {
                        listDocumentInfo.Add(documentInfo);
                    }
                }
            }            
        );
        parserSinkContext.SetListProjectDocumentInfo(markdownContext.MarkdownProjectInfo, listDocumentInfo);
    }

    public async Task<MarkdownDocumentInfo> ParseMarkdownFile(
        IParserSinkContext parserSinkContext,
        FileName markdownFile,
        CancellationToken cancellationToken) {
        Console.Out.WriteLine($"markdownFile: {markdownFile}");

        var markdownContent = await this._FileSystem.ReadAllTextAsync(markdownFile, Encoding.UTF8, cancellationToken);
        var document = Markdown.Parse(markdownContent, this._Pipeline);
        var documentInfo = MarkdownDocumentInfo.Create(markdownFile, parserSinkContext.SolutionData.DetailsFolder);
        IReplacementFinder? replacementFinder = null;

        var lstCurrentHeadings = new List<string>();
        string? currentHeading = default;
        for (var idx = 0; idx < document.Count; idx++) {
            var block = document[idx];

            if (block is HeadingBlock headingBlock) {
                if (headingBlock.Inline is null) {
                    throw new NotImplementedException("headingBlock.Inline");
                }
                while (lstCurrentHeadings.Count >= headingBlock.Level) {
                    lstCurrentHeadings.RemoveAt(lstCurrentHeadings.Count - 1);
                }
                lstCurrentHeadings.Add(GetText(headingBlock.Inline));

                var heading = BuildHeading(lstCurrentHeadings);
                var anchor = PathData.Create(documentInfo.FileName.RelativePath!, headingBlock.Inline.Line, heading);
                documentInfo.GetLstProvides().Add(
                    new SourceCodeData(
                        documentInfo.FileName,
                        new DetailData(
                            Kind: MatchInfoKind.Anchor,
                            MatchPath: anchor,
                            MatchRange: new Range(headingBlock.Inline.Span.Start, headingBlock.Inline.Span.End),
                            Path: PathData.Empty,
                            Command: string.Empty,
                            Comment: string.Empty,
                            Line: headingBlock.Inline.Line
                            )
                        )
                    );
                documentInfo.LstHeading.Add(heading);
                if (replacementFinder is not null) {
                    replacementFinder.VisitNotFound();
                    replacementFinder = null;
                }
                currentHeading = heading;
                // System.Console.Out.WriteLine($"headingBlock - {headingBlock.Level} - {heading}");
                continue;
            } else if (block is ParagraphBlock paragraphBlock) {
                if (paragraphBlock.Inline is not null) {
                    // System.Console.Out.WriteLine("ParagraphBlock");
                    for (var inline = paragraphBlock.Inline.FirstChild; inline is not null; inline = inline.NextSibling) {

                        if (inline is LiteralInline literalInline) {
                            var ownMatchPath = PathData.Create(documentInfo.FileName.RelativePath ?? string.Empty, literalInline.Line, currentHeading ?? string.Empty);
                            var matchInfo = MatchUtility.ParseMatch(literalInline.Content.ToString(), ownMatchPath, null, inline.Line, inline.Span.Start);

                            if (matchInfo is not null) {
                                var heading = BuildHeading(lstCurrentHeadings);
                                // TODO: was anchor
                                //if (!matchInfo.Path.IsEmpty()) {
                                //    throw new InvalidOperationException("matchInfo.Path anchor was used");
                                //}
                                //matchInfo = matchInfo with {
                                //    Path = PathData.Create(documentInfo.FileName.RelativePath ?? string.Empty, literalInline.Line, heading)
                                //};

                                var sourceCodeMatch = new SourceCodeData(
                                    documentInfo.FileName,
                                    matchInfo,
                                    null
                                    );
                                (matchInfo.IsCommand
                                    ? documentInfo.GetLstConsumes()
                                    : documentInfo.GetLstProvides()
                                    ).Add(sourceCodeMatch);
                                var command = this.GetMatchCommand(matchInfo);
                                if (command is not null) {
                                    if (replacementFinder is not null) {
                                        replacementFinder.VisitNotFound();
                                        replacementFinder = null;
                                    }
                                    replacementFinder = command.GetReplacementFinder(documentInfo, sourceCodeMatch);
                                }
                            }
                        } else if (inline is LinkInline linkInline) {
                            Console.Out.WriteLine($"  linkInline - {linkInline.Url} - {linkInline.Title} - {GetText(linkInline)}");
                        } else if (inline is LineBreakInline) {
                        } else {
                            // throw new NotImplementedException($"TODO {inline.GetType().FullName}");
                            Console.Out.WriteLine($"  TODO {inline.GetType().FullName}");
                        }
                    }
                }
                continue;
            } else if (block is FencedCodeBlock fencedCodeBlock) {
                if (replacementFinder is not null && replacementFinder.VisitBlock(fencedCodeBlock)) {
                    replacementFinder = null;
                }
            } else if (block is CodeBlock codeBlock) {
                if (replacementFinder is not null && replacementFinder.VisitBlock(codeBlock)) {
                    replacementFinder = null;
                }

            } else if (block is ListBlock listBlock) {
                if (replacementFinder is not null && replacementFinder.VisitBlock(listBlock)) {
                    replacementFinder = null;
                }
            } else {
                if (replacementFinder is not null && replacementFinder.VisitBlock(block)) {
                    replacementFinder = null;
                }
            }
            Console.Out.WriteLine("block - " + block.GetType().FullName);
            /*
            if (block is LeafBlock leafBlock) {
                var inline = leafBlock.Inline;
                if (inline is not null) {
                    foreach (var x in inline) {
                        System.Console.Out.WriteLine(" inline - " + x.GetType().FullName);
                    }
                }
            }
            */
        }
        return documentInfo;
    }

    public static string BuildHeading(List<string> lstCurrentHeadings) {
        var sb = new StringBuilder();
        for (var idx = 0; idx < lstCurrentHeadings.Count; idx++) {
            if (idx == 0) {
                sb.Append("#/");
            } else {
                sb.Append("/");
            }
            sb.Append(lstCurrentHeadings[idx]);
        }
        return sb.ToString();
    }

    public static string GetText(ContainerInline inline) {
        StringBuilder? sb = default;
        for (var item = inline.FirstChild; item is not null; item = item.NextSibling) {
            if (item is LiteralInline literalInline) {
                if (item.NextSibling is null) {
                    return literalInline.Content.ToString();
                } else {
                    if (sb is null) {
                        sb = new StringBuilder();
                    }
                    sb.Append(literalInline.Content);
                    continue;
                }
            }
            throw new NotImplementedException($"TODO {item.GetType().FullName}");
        }
        return sb?.ToString() ?? "";
    }

    public async Task WriteMarkdownDetail(
        WriterContext writerContext,
        CancellationToken cancellationToken) {
        // § todo.md
        //Console.WriteLine($"DetailPath {writerContext.SolutionData.DetailsFolder}");
        var lstMarkdownDocumentInfo = writerContext.GetAllMarkdownDocumentInfo();
        foreach (var markdownDocumentInfo in lstMarkdownDocumentInfo) {
            MarkdownDocumentWriter? markdownDocumentWriter = default;
            if (markdownDocumentInfo.ListConsumes is null) {
                continue;
            }
            var lstReplacementFinder = markdownDocumentInfo.LstReplacementFinder ?? new();
            foreach (var sourceCodeMatch in markdownDocumentInfo.ListConsumes) {
                var matchInfo = sourceCodeMatch.DetailData;
                if (!matchInfo.IsCommand) { continue; }

                var replacementFinder = lstReplacementFinder.FirstOrDefault(
                    replacementFinder => ReferenceEquals(replacementFinder.SourceCodeMatch, sourceCodeMatch));
                if (replacementFinder is not null) {
                    if (markdownDocumentWriter is null) {
                        markdownDocumentWriter = await EnsureMarkdownDocumentWriter(writerContext, markdownDocumentInfo, markdownDocumentWriter, cancellationToken);
                    }

                    await replacementFinder.Command.ExecuteAsync(sourceCodeMatch, markdownDocumentWriter, replacementFinder, cancellationToken);
                    continue;
                }

                var command = this.GetMatchCommand(matchInfo);
                if (command is null) { continue; }

                if (markdownDocumentWriter is null) {
                    markdownDocumentWriter = await EnsureMarkdownDocumentWriter(writerContext, markdownDocumentInfo, markdownDocumentWriter, cancellationToken);
                }
                await command.ExecuteAsync(sourceCodeMatch, markdownDocumentWriter, null, cancellationToken);
            }
            if (markdownDocumentWriter is not null) {
                await markdownDocumentWriter.WriteAsync(cancellationToken);
            }
        }
        await Task.CompletedTask;

        async Task<MarkdownDocumentWriter> EnsureMarkdownDocumentWriter(WriterContext writerContext, MarkdownDocumentInfo markdownDocumentInfo, MarkdownDocumentWriter? markdownDocumentWriter, CancellationToken cancellationToken) {
            if (markdownDocumentWriter is null) {
                var markdownContent = await this._FileSystem.ReadAllTextAsync(markdownDocumentInfo.FileName, Encoding.UTF8, cancellationToken);
                var document = Markdown.Parse(markdownContent, this._Pipeline);
                markdownDocumentWriter = new MarkdownDocumentWriter(writerContext, markdownDocumentInfo, markdownContent, document, this._FileSystem);
            }

            return markdownDocumentWriter;
        }
    }


    private IMatchCommand? GetMatchCommand(DetailData match) {
        if (!match.IsCommand) { return null; }
        var command = this._LstMatchCommand.FirstOrDefault(c => c.IsMatching(match));
        return command;
    }
}
public record MarkdownContext(
    ProjectData MarkdownProjectInfo
    );