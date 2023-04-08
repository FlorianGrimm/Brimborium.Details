import * as vscode from "vscode";
import { DetailsDocumentLink } from "./DetailsDocumentLink";
import type { DetailsExtensionState } from "./DetailsExtensionState";

/**
 * A {@link DocumentLinkProvider document link provider} that detects links to details files.
 * details://<detailsfile>
 * DetailsCodeLinkDefinitionProvider
 *
 */
export class DetailsLinkDefinitionProvider
  implements vscode.DocumentLinkProvider<vscode.DocumentLink>
{
  regexpDetailsGlobal: RegExp;
  regexpDetailsLocal: RegExp;

  constructor(
    private readonly commentPrefix: string,
    private state: DetailsExtensionState
  ) {
    commentPrefix = `(${commentPrefix}[ \t]*)`;
    const e = commentPrefix + "§[ \t]*([^#§\\r\\n]+)";
    this.regexpDetailsGlobal = new RegExp(e, "g");
    this.regexpDetailsLocal = new RegExp(e);
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
    while ((match = this.regexpDetailsGlobal.exec(text))) {
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

    const match = link.tooltip.match(this.regexpDetailsLocal);
    if (match === null) {
      this.state.log(`resolveDocumentLink: ${link.tooltip}: tooltip does not match`);
      return undefined;
    }

    let targetPath = match[2];
    if (!targetPath) {
      this.state.log(`resolveDocumentLink: ${link.tooltip}: tooltip match is falsy`);
      return undefined;
    }

    if (token.isCancellationRequested) {
      return undefined;
    }
    const targetUri = await workspaceStateDetails.getDetailsFilePath(workspaceStateFile, targetPath, token);
    if (targetUri === undefined) {
      this.state.log(`resolveDocumentLink: ${link.tooltip}: targetUri === undefined`);
      return undefined;
    }

    link.target = targetUri;
    this.state.log(`resolveDocumentLink: ${targetUri}`);
    return link;
  }
}
