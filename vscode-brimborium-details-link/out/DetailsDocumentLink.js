"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.DetailsDocumentLink = void 0;
const vscode = require("vscode");
class DetailsDocumentLink extends vscode.DocumentLink {
    constructor(range, target, tooltip, filename) {
        super(range);
        this.filename = filename;
        this.target = target;
        this.tooltip = tooltip;
    }
}
exports.DetailsDocumentLink = DetailsDocumentLink;
//# sourceMappingURL=DetailsDocumentLink.js.map