"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.DetailsLinkDefinitionProvider = void 0;
const vscode = require("vscode");
const DetailsDocumentLink_1 = require("./DetailsDocumentLink");
/**
 * A {@link DocumentLinkProvider document link provider} that detects links to details files.
 * details://<detailsfile>
 * DetailsCodeLinkDefinitionProvider
 *
 */
class DetailsLinkDefinitionProvider {
    constructor(commentPrefix, state) {
        this.commentPrefix = commentPrefix;
        this.state = state;
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
    provideDocumentLinks(document, token) {
        const text = document.getText();
        if (token.isCancellationRequested) {
            return undefined;
        }
        const links = [];
        let match;
        while ((match = this.regexpDetailsGlobal.exec(text))) {
            const start = document.positionAt(match.index);
            const end = document.positionAt(match.index + match[0].length);
            const range = new vscode.Range(start, end);
            links.push(new DetailsDocumentLink_1.DetailsDocumentLink(range, undefined, match[0], document.fileName));
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
    async resolveDocumentLink(link, token) {
        if (link.filename === undefined) {
            return undefined;
        }
        const workspaceState = this.state.getWorkspaceStateByFileName(link.filename);
        if (workspaceState === undefined) {
            return undefined;
        }
        const detailsRoot = await workspaceState.getDetailsRoot(token);
        if (detailsRoot === undefined) {
            return undefined;
        }
        if (!link.tooltip) {
            return undefined;
        }
        const match = link.tooltip.match(this.regexpDetailsLocal);
        if (match === null) {
            return undefined;
        }
        let targetPath = match[2];
        if (!targetPath) {
            return undefined;
        }
        if (token.isCancellationRequested) {
            return undefined;
        }
        const targetUri = await workspaceState.getDetailsPath(targetPath, token);
        if (targetUri === undefined) {
            return undefined;
        }
        link.target = targetUri;
        console.log("resolveDocumentLink: %s", targetUri.toString());
        return link;
    }
}
exports.DetailsLinkDefinitionProvider = DetailsLinkDefinitionProvider;
//# sourceMappingURL=DetailsLinkDefinitionProvider.js.map