import * as vscode from "vscode";
import { CacheFileWatcher, CacheItem } from "./CacheFileWatcher";
import { DetailsFileWatcher } from "./DetailsFileWatcher";
import type { StateCommunication } from "./DetailsExtensionState";
import { fileUtilities } from "./FileUtilties";
import { DetailsJSON } from "./DetailsJSON";

export class WorkspaceState {
    private detailsFileWatcher: DetailsFileWatcher | undefined;
    private details: DetailsJSON | undefined;

    constructor(
        private readonly stateCommunication: StateCommunication,
        private readonly logger: vscode.LogOutputChannel,
        private readonly workspaceFolder: vscode.WorkspaceFolder,
        public readonly id: string
    ) {
    }

    log(message: string) {
        this.logger.appendLine(message);
        console.log(message);
    }

    public async initialize() {
        this.log(`WorkspaceState.initialize id: ${this.id}, workspaceFolderUri: ${this.workspaceFolder.uri.toString()}`);
        if (await fileUtilities.exists(vscode.Uri.joinPath(this.workspaceFolder.uri, "details.json"), vscode.FileType.File)) {
            this.log(`WorkspaceState.initialize id: ${this.id}, workspaceFolder.uri: ${this.workspaceFolder.uri.toString()}, found details.json`);
            const detailsFileWatcher = new DetailsFileWatcher(new WorkspaceStateCommunication(this), this.logger, this.workspaceFolder);
            this.detailsFileWatcher = detailsFileWatcher;
            await detailsFileWatcher.initialize();
            // TODO: this.stateCommunication.detailsFileFound(this.workspaceFolderUri);
        }
    }

    public dispose() {
        if (this.detailsFileWatcher){
            this.detailsFileWatcher.dispose();
        }
    }

    toString() {
        return `WorkspaceState: ${this.id} - ${JSON.stringify(this.details??{})}`;
    }

    setDetails(details: DetailsJSON | undefined){
        this.details = details;
        // this.stateCommunication.setDetails(details);
    }
    hasDetails(){
        return this.details !== undefined;
    }
    getDetails(){
        return this.details;
    }

    async getDetailsPath(targetPath: string, token: vscode.CancellationToken): Promise<vscode.Uri> {
        throw new Error("Method not implemented.");
    }

    async getDetailsRoot(token: vscode.CancellationToken): Promise<vscode.Uri> {
        throw new Error("Method not implemented.");
    }

    async getCodePath(targetPath: string, token: vscode.CancellationToken): Promise<vscode.Uri> {
        throw new Error("Method not implemented.");
    }

}

export class WorkspaceStateCommunication {
    constructor(
        private readonly workspaceState: WorkspaceState
    ) {
    }
    setDetails(details: DetailsJSON | undefined){
        this.workspaceState.setDetails(details);
    }
}