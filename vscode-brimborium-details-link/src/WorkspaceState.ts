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
        if (this.detailsFileWatcher) {
            this.detailsFileWatcher.dispose();
        }
    }

    toString() {
        return `WorkspaceState: ${this.id} - ${JSON.stringify(this.details ?? {})}`;
    }

    setDetails(details: DetailsJSON | undefined) {
        this.details = details;
        // this.stateCommunication.setDetails(details);
    }
    hasDetails() {
        return this.details !== undefined;
    }
    getDetails() {
        return this.details;
    }

    getDetailsRoot(token: vscode.CancellationToken): vscode.Uri | undefined {
        return this.details?.detailsRootUri;
    }

    async getDetailsFilePath(
        workspaceFile: WorkspaceState,
        targetPath: string,
        token: vscode.CancellationToken): Promise<vscode.Uri | undefined> {
        if (this.details === undefined) {
            return undefined;
        }
        if (this.details.detailsFolderUri === undefined) {
            return undefined;
        }
        {
            const resultPath = vscode.Uri.joinPath(this.details.detailsFolderUri, targetPath);
            if (await fileUtilities.exists(resultPath, vscode.FileType.File)) {
                return resultPath;
            }
        }
        {
            if (token.isCancellationRequested) { return undefined; }
        }
        {
            const resultPath = vscode.Uri.joinPath(workspaceFile.workspaceFolder.uri, targetPath);
            if (await fileUtilities.exists(resultPath, vscode.FileType.File)) {
                return resultPath;
            }
        }

        return undefined;
    }

    async getCodeFilePath(
        workspaceFile: WorkspaceState,
        targetPath: string,
        token: vscode.CancellationToken
    ): Promise<vscode.Uri | undefined> {
        if (this.details === undefined) {
            return undefined;
        }
        if (this.details.detailsRootUri === undefined) {
            return undefined;
        }
        {
            const resultPath = vscode.Uri.joinPath(this.details.detailsRootUri, targetPath);
            if (await fileUtilities.exists(resultPath, vscode.FileType.File)) {
                return resultPath;
            }
        }
        {
            if (token.isCancellationRequested) { return undefined; }
        }
        {
            const resultPath = vscode.Uri.joinPath(workspaceFile.workspaceFolder.uri, targetPath);
            if (await fileUtilities.exists(resultPath, vscode.FileType.File)) {
                return resultPath;
            }
        }
        return undefined;
    }
}

export class WorkspaceStateCommunication {
    constructor(
        private readonly workspaceState: WorkspaceState
    ) {
    }
    setDetails(details: DetailsJSON | undefined) {
        this.workspaceState.setDetails(details);
    }
}