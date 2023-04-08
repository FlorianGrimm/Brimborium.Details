import * as vscode from "vscode";
import { DetailsDocumentLink } from "./DetailsDocumentLink";
import type { DetailsExtensionState } from "./DetailsExtensionState";



/**
 * A {@link DocumentLinkProvider document link provider} that detects links to details files.
 * detailscode://path/to/code.ext#linenumber§marker
 */
export class DetailsCodeLinkDefinitionProvider
  implements vscode.DocumentLinkProvider<vscode.DocumentLink>
{
  // TODO: c# and this is different
  regExpDetailscodeGlobal = new RegExp(
    "detailscode://([^ #]*)([#][0-9]+)?([§].*)?",
    "g"
  );
  regExpDetailscodeLocal = new RegExp(
    "detailscode://([^ #]*)([#][0-9]+)?([§].*)?",
  );
  constructor(private state: DetailsExtensionState) {

  }
  /**
   * Provide links for the given document. Note that the editor ships with a default provider that detects
   * `http(s)` and `file` links.
   *
   * @param document The document in which the command was invoked.
   * @param token A cancellation token.
   * @return An array of {@link DocumentLink document links} or a thenable that resolves to such. The lack of a result
   * can be signaled by returning `undefined`, `null`, or an empty array.
   */
  provideDocumentLinks(
    document: vscode.TextDocument,
    token: vscode.CancellationToken
  ): vscode.ProviderResult<vscode.DocumentLink[]> {
    const text = document.getText();
    if (token.isCancellationRequested) {
      return undefined;
    }

    const links: DetailsDocumentLink[] = [];
    let match: RegExpExecArray | null;
    while ((match = this.regExpDetailscodeGlobal.exec(text))) {
      const start = document.positionAt(match.index);
      const end = document.positionAt(match.index + match[0].length);
      const range = new vscode.Range(start, end);
      links.push(
        new DetailsDocumentLink(range, undefined, match[0], document.fileName)
      );
      if (token.isCancellationRequested) {
        return undefined;
      }
    }
    return links;
  }

  /**
   * Given a link fill in its {@link DocumentLink.target target}. This method is called when an incomplete
   * link is selected in the UI. Providers can implement this method and return incomplete links
   * (without target) from the {@linkcode DocumentLinkProvider.provideDocumentLinks provideDocumentLinks} method which
   * often helps to improve performance.
   *
   * @param link The link that is to be resolved.
   * @param token A cancellation token.
   */
  async resolveDocumentLink(
    link: DetailsDocumentLink,
    token: vscode.CancellationToken
  ) /*: vscode.ProviderResult<vscode.DocumentLink>*/ {
    if (link.filename === undefined) {
      this.state.log("resolveDocumentLink: link.filename === undefined");
      return undefined;
    }
    const workspaceStateDetails = this.state.getWorkspaceStateWithDetails();
    if (workspaceStateDetails === undefined) {
      this.state.log(`resolveDocumentLink: ${link.filename}: workspaceStateDetails === undefined"`);
      return undefined;
    }
    const workspaceStateFile = this.state.getWorkspaceStateByFileName(
      link.filename
    );
    if (workspaceStateFile === undefined) {
      this.state.log(`resolveDocumentLink: ${link.filename}: workspaceStateFile === undefined"`);
      return undefined;
    }
    if (!link.tooltip) {
      this.state.log(`resolveDocumentLink: ${link.filename}: tooltip === undefined`);
      return undefined;
    }

    const match = link.tooltip.match(this.regExpDetailscodeLocal);
    if (match === null) {
      this.state.log(`resolveDocumentLink: ${link.tooltip}: tooltip does not match`);
      return undefined;
    }

    let targetPath = match[1];
    if (!targetPath) {
      this.state.log(`resolveDocumentLink: ${link.tooltip}: tooltip match is falsy`);
      return undefined;
    }

    let lineNumberText = match[2];
    let lineNumber = 0;
    if (lineNumberText) {
      lineNumberText = lineNumberText.substring(1);
      lineNumber = parseInt(lineNumberText);
    }

    if (token.isCancellationRequested) {
      return undefined;
    }

    const targetUri =await this.state.getCodeFilePath(workspaceStateDetails, workspaceStateFile, targetPath, token);
    if (targetUri === undefined) {
      return undefined;
    }

    link.target = targetUri.with({ fragment: lineNumberText });
    this.state.log(`resolveDocumentLink: ${link.target}`);
    // if (lineNumber <= 0) {
    //   this.state.log(`resolveDocumentLink: ${targetUri}`);
    // } else {
    //   //link.range = new vscode.Range(new vscode.Position(lineNumber-1, 0), new vscode.Position(lineNumber-1, 0));
    //   this.state.log(`resolveDocumentLink: ${targetUri} ${lineNumber}`);
    // }
    return link;
  }
}
