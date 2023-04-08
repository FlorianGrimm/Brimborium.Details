import * as vscode from "vscode";
import { DetailsDocumentLink } from "./DetailsDocumentLink";
import type { DetailsExtensionState } from "./DetailsExtensionState";

/**
 * A {@link DocumentLinkProvider document link provider} that detects links to details files.
 * details://<detailsfile>
 * DetailsCodeLinkDefinitionProvider
 *
 */
export class DetailsDocuLinkDefinitionProvider
  implements vscode.DocumentLinkProvider<DetailsDocumentLink>
{
  regexpDetailsGlobal: RegExp;
  regexpDetailsLocal: RegExp;

  constructor(private state: DetailsExtensionState) {
    // TODO: c# and this is different
    const e = "details://([^)#]*)([#][1-9]+)?([§].*)?";
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
  ): vscode.ProviderResult<DetailsDocumentLink[]> {
    const text = document.getText();
    if (token.isCancellationRequested) {
      return undefined;
    }
    const workspaceState = this.state.getWorkspaceStateByFileName(
      document.fileName
    );

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
  ) /*: vscode.ProviderResult<DetailsDocumentLink>*/ {
    if (link.filename === undefined) {
      console.log("resolveDocumentLink: link.filename === undefined");
      return undefined;
    }
    const workspaceState = this.state.getWorkspaceStateByFileName(
      link.filename
    );
    if (workspaceState === undefined) {
      console.log("resolveDocumentLink: %s: workspaceState === undefined",link.filename);
      return undefined;
    }
    const detailsRoot = await workspaceState.getDetailsRoot(token);
    if (detailsRoot === undefined) {
      console.log("resolveDocumentLink: %s: detailsRoot === undefined",link.filename);
      return undefined;
    }

    if (!link.tooltip) {
      console.log("resolveDocumentLink: %s: tooltip === undefined",link.filename);
      return undefined;
    }

    const match = link.tooltip.match(this.regexpDetailsLocal);
    if (match === null) {
      console.log("resolveDocumentLink: %s: tooltip does not match",link.tooltip);
      return undefined;
    }

    let targetPath = match[1];
    if (!targetPath) {
      console.log("resolveDocumentLink: %s: tooltip match is falsy",link.tooltip);
      return undefined;
    }

    if (token.isCancellationRequested) {
      return undefined;
    }
    const targetUri = await workspaceState.getDetailsPath(targetPath, token);
    if (targetUri === undefined) {
      console.log("resolveDocumentLink: %s: targetUri === undefined",link.tooltip);
      return undefined;
    }

    link.target = targetUri;
    console.log("resolveDocumentLink: %s", targetUri.toString());
    return link;
  }
}
