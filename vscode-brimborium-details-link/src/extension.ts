// The module 'vscode' contains the VS Code extensibility API
// Import the module and reference it with the alias vscode in your code below
import * as vscode from "vscode";
import { CacheFileWatcher } from "./CacheFileWatcher";
import { DetailsCodeLinkDefinitionProvider } from "./DetailsCodeLinkDefinitionProvider";
import { DetailsDocuLinkDefinitionProvider } from "./DetailsDocuLinkDefinitionProvider";
import { DetailsExtensionState } from "./DetailsExtensionState";
import { DetailsFileWatcher } from "./DetailsFileWatcher";
import { DetailsLinkDefinitionProvider } from "./DetailsLinkDefinitionProvider";
//import { DetailsDocuLinkCompletionItemProvider } from "./DetailsDocuLinkCompletionItemProvider";

let state = new DetailsExtensionState();

// This method is called when your extension is activated
// Your extension is activated the very first time the command is executed
export function activate(context: vscode.ExtensionContext) {
  console.log("vscode-brimborium-details-link activate");
  // Use the console to output diagnostic information (console.log) and errors (console.error)
  // This line of code will only be executed once when your extension is activated
  //   console.log(
  //     'Congratulations, your extension "vscode-brimborium-details-link" is now active!'
  //   );
  context.subscriptions.push(
    state
  );

  context.subscriptions.push(
    vscode.workspace.onDidChangeWorkspaceFolders((event) => {
      event.removed.forEach((folder) => {
        state.removeWorkspaceFolder(folder);
      });
      event.added.forEach((folder) => {
        state.addWorkspaceFolder(folder);
      });
    })
  );
  vscode.workspace.workspaceFolders?.forEach((folder) => {
    state.addWorkspaceFolder(folder);
  });

  context.subscriptions.push(
    vscode.commands.registerCommand(
      "vscode-brimborium-details-link.showDetails",
      () => {
        // The code you place here will be executed every time your command is executed
        // Display a message box to the user
        showDetails();
      }
    )
  );

  context.subscriptions.push(
    vscode.languages.registerDocumentLinkProvider(
      { language: "markdown" },
      new DetailsCodeLinkDefinitionProvider(state)
    )
  );
  context.subscriptions.push(
    vscode.languages.registerDocumentLinkProvider(
      { language: "markdown" },
      new DetailsDocuLinkDefinitionProvider(state)
    )
  );

  context.subscriptions.push(
    vscode.languages.registerDocumentLinkProvider(
      { language: "csharp" },
      new DetailsDocuLinkDefinitionProvider(state)
    )
  );

  context.subscriptions.push(
    vscode.languages.registerDocumentLinkProvider(
      { language: "csharp" },
      new DetailsLinkDefinitionProvider("//", state)
    )
  );

  /*
  context.subscriptions.push(
	vscode.languages.registerCompletionItemProvider(
		{ language: "csharp" },
		new DetailsDocuLinkCompletionItemProvider(state),
	));
	*/
}

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
export function deactivate() {}
