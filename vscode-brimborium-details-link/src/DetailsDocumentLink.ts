import * as vscode from "vscode";

export class DetailsDocumentLink extends vscode.DocumentLink{
  constructor(
    range: vscode.Range, 
    target: vscode.Uri| undefined, 
    tooltip: string | undefined,
    public readonly filename: string | undefined) {
    super(range);
    this.target = target;
    this.tooltip = tooltip;
  }
}