"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.deactivate = exports.activate = void 0;
// The module 'vscode' contains the VS Code extensibility API
// Import the module and reference it with the alias vscode in your code below
const vscode = require("vscode");
const DetailsCodeLinkDefinitionProvider_1 = require("./DetailsCodeLinkDefinitionProvider");
const DetailsDocuLinkDefinitionProvider_1 = require("./DetailsDocuLinkDefinitionProvider");
const DetailsExtensionState_1 = require("./DetailsExtensionState");
const DetailsLinkDefinitionProvider_1 = require("./DetailsLinkDefinitionProvider");
//import { DetailsDocuLinkCompletionItemProvider } from "./DetailsDocuLinkCompletionItemProvider";
let state = new DetailsExtensionState_1.DetailsExtensionState();
// This method is called when your extension is activated
// Your extension is activated the very first time the command is executed
function activate(context) {
    console.log("vscode-brimborium-details-link activate");
    // Use the console to output diagnostic information (console.log) and errors (console.error)
    // This line of code will only be executed once when your extension is activated
    //   console.log(
    //     'Congratulations, your extension "vscode-brimborium-details-link" is now active!'
    //   );
    context.subscriptions.push(state);
    context.subscriptions.push(vscode.workspace.onDidChangeWorkspaceFolders((event) => {
        event.removed.forEach((folder) => {
            state.removeWorkspaceFolder(folder);
        });
        event.added.forEach((folder) => {
            state.addWorkspaceFolder(folder);
        });
    }));
    vscode.workspace.workspaceFolders?.forEach((folder) => {
        state.addWorkspaceFolder(folder);
    });
    context.subscriptions.push(vscode.commands.registerCommand("vscode-brimborium-details-link.showDetails", () => {
        // The code you place here will be executed every time your command is executed
        // Display a message box to the user
        showDetails();
    }));
    context.subscriptions.push(vscode.languages.registerDocumentLinkProvider({ language: "markdown" }, new DetailsCodeLinkDefinitionProvider_1.DetailsCodeLinkDefinitionProvider(state)));
    context.subscriptions.push(vscode.languages.registerDocumentLinkProvider({ language: "markdown" }, new DetailsDocuLinkDefinitionProvider_1.DetailsDocuLinkDefinitionProvider(state)));
    context.subscriptions.push(vscode.languages.registerDocumentLinkProvider({ language: "csharp" }, new DetailsDocuLinkDefinitionProvider_1.DetailsDocuLinkDefinitionProvider(state)));
    context.subscriptions.push(vscode.languages.registerDocumentLinkProvider({ language: "csharp" }, new DetailsLinkDefinitionProvider_1.DetailsLinkDefinitionProvider("//", state)));
    /*
    context.subscriptions.push(
      vscode.languages.registerCompletionItemProvider(
          { language: "csharp" },
          new DetailsDocuLinkCompletionItemProvider(state),
      ));
      */
}
exports.activate = activate;
async function showDetails() {
    vscode.window.showInformationMessage(JSON.stringify(state));
    //const rootFolder = vscode.workspace.asRelativePath("details.json");
    //const detailsJson = vscode.workspace.asRelativePath("details.json");
    // vscode.workspace.fs.stat(vscode.Uri.file(detailsJson)).then((stat) => {
    // 	vscode.window.showInformationMessage(detailsJson);
    // });
    // vscode.workspace.findFiles('details.json').then((uris) => {
    // 	var uriAsString = uris.map((uri) => uri.toString()).join("; ");
    // 	vscode.window.showInformationMessage(detailsJson);
    // 	if (uris.length === 1){
    // 		vscode.workspace.fs.readFile(uris[0]).then((data) => {
    // 			});
    // 	}
    // });
    /*
      if (detailsJson) {
          vscode.window.showInformationMessage(detailsJson);
      } else {
          vscode.window.showErrorMessage('No details.json found.');
      }
      */
}
// This method is called when your extension is deactivated
function deactivate() { }
exports.deactivate = deactivate;
//# sourceMappingURL=extension.js.map