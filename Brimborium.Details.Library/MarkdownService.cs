namespace Brimborium.Details;

[Brimborium.Registrator.Singleton]
public class MarkdownService {
    public readonly SolutionInfo SolutionInfo;
    private readonly MarkdownPipeline _Pipeline;

    public MarkdownService(SolutionInfo solutionInfo) {
        this.SolutionInfo = solutionInfo;

        this._Pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .EnableTrackTrivia()
            .Build();
    }

    public async Task ParseDetail(CancellationToken cancellationToken) {
        // System.Console.Out.WriteLine($"DetailsFolder: {SolutionInfo.DetailsFolder}");

        var lstMarkdownFile = Directory.EnumerateFiles(this.SolutionInfo.DetailsFolder, "*.md", SearchOption.AllDirectories);
        if (lstMarkdownFile is null || !lstMarkdownFile.Any()){
            System.Console.Out.WriteLine($"DetailsFolder: {SolutionInfo.DetailsFolder} contains no *.md files");
            return;
        }
        foreach (var markdownFile in lstMarkdownFile) {
            await this.ParseMarkdownFile(markdownFile);
        }
    }

    public async Task ParseMarkdownFile(string markdownFile) {
        System.Console.Out.WriteLine($"markdownFile: {markdownFile}");
        var markdownContent = await File.ReadAllTextAsync(markdownFile);
        //MarkdownDocument document = MarkdownParser.Parse(markdownContent);
        MarkdownDocument document = Markdown.Parse(markdownContent, this._Pipeline);
        List<string> listHeadings = new List<string>();
        for (int idx = 0; idx < document.Count; idx++) {
            var block = document[idx];

            //ContainerInline? inline = default;
            //if (block is LeafBlock leafBlock) {
            //    inline = leafBlock.Inline;
            //}

            if (block is HeadingBlock headingBlock) {
                if (headingBlock.Inline is null) {
                    throw new NotImplementedException("headingBlock.Inline");
                }
                // System.Console.Out.WriteLine($"headingBlock - {headingBlock.Level} - {GetText(headingBlock.Inline)}");                
                while (listHeadings.Count >= headingBlock.Level) {
                    listHeadings.RemoveAt(listHeadings.Count - 1);
                }
                listHeadings.Add(GetText(headingBlock.Inline));
                var heading = string.Join(" / ", listHeadings);
                System.Console.Out.WriteLine($"headingBlock - {headingBlock.Level} - {heading}");
                continue;
            } else if (block is ParagraphBlock paragraphBlock) {
                if (paragraphBlock.Inline is not null) {
                    System.Console.Out.WriteLine("ParagraphBlock");
                    for (var inline = paragraphBlock.Inline.FirstChild; inline is not null; inline = inline.NextSibling) {
                        if (inline is LiteralInline literalInline) {
                            var match = MatchUtility.parseMatchIfMatches(literalInline.Content.ToString());
                            if (match is not null) {
                                System.Console.Out.WriteLine($"  literalInline - {literalInline.Content} - {match}");
                            } else {
                                System.Console.Out.WriteLine($"  literalInline - {literalInline.Content}");
                            }
                            if (match is not null && match.IsCommand) {
                                // var command = this.GetMatchCommand(match);
                                // command
                            }
                        } else if (inline is LinkInline linkInline) {
                            System.Console.Out.WriteLine($"  linkInline - {linkInline.Url} - {linkInline.Title} - {GetText(linkInline)}");
                        } else if (inline is LineBreakInline) {
                        } else {
                            throw new NotImplementedException($"TODO {inline.GetType().FullName}");
                        }
                    }
                }
                continue;
            } else if (block is FencedCodeBlock fencedCodeBlock) {
            } else if (block is CodeBlock codeBlock) {
            }
            System.Console.Out.WriteLine("block - " + block.GetType().FullName);
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
    }

    private object GetMatchCommand(MatchInfo match) {
        throw new NotImplementedException();
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

    public async Task WriteDetail(CancellationToken cancellationToken) {
        Console.WriteLine($"DetailPath {SolutionInfo.DetailsFolder}");
        await Task.CompletedTask;
    }
}