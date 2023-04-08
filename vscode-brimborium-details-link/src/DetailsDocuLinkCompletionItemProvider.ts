import * as vscode from "vscode";
import type { DetailsExtensionState } from "./DetailsExtensionState";

// TODO: c# and this is different
const regExpDetailscode = new RegExp(
  "detailscode://([^ #]*)([#][1-9]+)?([§].*)?",
  "g"
);

export class DetailsDocuLinkCompletionItemProvider
  implements vscode.CompletionItemProvider<vscode.CompletionItem>
{
  constructor(private state: DetailsExtensionState) {}

  /**
   * Provide completion items for the given position and document.
   *
   * @param document The document in which the command was invoked.
   * @param position The position at which the command was invoked.
   * @param token A cancellation token.
   * @param context How the completion was triggered.
   *
   * @return An array of completions, a {@link CompletionList completion list}, or a thenable that resolves to either.
   * The lack of a result can be signaled by returning `undefined`, `null`, or an empty array.
   */
  provideCompletionItems(
    document: vscode.TextDocument,
    position: vscode.Position,
    token: vscode.CancellationToken,
    context: vscode.CompletionContext
  ): vscode.ProviderResult<vscode.CompletionItem[] | vscode.CompletionList<vscode.CompletionItem>> {
    //new vscode.Range(position-, position)
    //const documentText = document.getText();
    const line = document.lineAt(position.line);
    const lineText = line.text;
    let result: vscode.CompletionItem[] = [];
    const matches = lineText.matchAll(regExpDetailscode);
    let foundMatch: RegExpMatchArray | undefined = undefined;
    for (const match of matches) {
      if (typeof match.index === "number") {
        if (match.index < position.character) {
          foundMatch = match;
        } else {
          break;
        }
        /*
            const start = document.positionAt(match.index);
            const end = document.positionAt(match.index + match[0].length);
            const range = new vscode.Range(start, end);
            const item = new vscode.CompletionItem(match[0], vscode.CompletionItemKind.Reference);
            item.range = range;
            item.detail = "detailscode";
            item.documentation = "detailscode";
            result.push(item);
            */
      }
    }
    if (foundMatch!==undefined) {
        if (foundMatch.length>1)    {
            const prefix = foundMatch[1];
            /*
            position.character =
            foundMatch.index!
            const item = new vscode.CompletionItem(prefix+"test", vscode.CompletionItemKind.Reference);
            

            const start = document.positionAt(match.index);
            const end = document.positionAt(match.index + match[0].length);
            const range = new vscode.Range(start, end);
            
            item.range = range;
            item.detail = "detailscode";
            item.documentation = "detailscode";
            result.push(item);
            */
        }
    }
    // let match: RegExpExecArray | null;
    // while ((match = regExpDetailscode.exec(lineText))) {
    //     match.index
    // }
    //.substring(0, position.character)
    // document.offsetAt(position);

    return result;
  }

  /**
   * Given a completion item fill in more data, like {@link CompletionItem.documentation doc-comment}
   * or {@link CompletionItem.detail details}.
   *
   * The editor will only resolve a completion item once.
   *
   * *Note* that this function is called when completion items are already showing in the UI or when an item has been
   * selected for insertion. Because of that, no property that changes the presentation (label, sorting, filtering etc)
   * or the (primary) insert behaviour ({@link CompletionItem.insertText insertText}) can be changed.
   *
   * This function may fill in {@link CompletionItem.additionalTextEdits additionalTextEdits}. However, that means an item might be
   * inserted *before* resolving is done and in that case the editor will do a best effort to still apply those additional
   * text edits.
   *
   * @param item A completion item currently active in the UI.
   * @param token A cancellation token.
   * @return The resolved completion item or a thenable that resolves to of such. It is OK to return the given
   * `item`. When no result is returned, the given `item` will be used.
   */
  resolveCompletionItem(
    item: vscode.CompletionItem,
    token: vscode.CancellationToken
  ): vscode.ProviderResult<vscode.CompletionItem> {
    return undefined;
  }
}
