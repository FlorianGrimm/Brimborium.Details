"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.WorkspaceState = exports.DetailsExtensionState = void 0;
const vscode = require("vscode");
const CacheFileWatcher_1 = require("./CacheFileWatcher");
const DetailsFileWatcher_1 = require("./DetailsFileWatcher");
/*eslint-enable */
class DetailsExtensionState {
    constructor() {
        this.workspaceStates = new Map();
    }
    getWorkspaceStateIdByFileName(fileName) {
        const workspaceFolder = vscode.workspace.getWorkspaceFolder(vscode.Uri.parse(fileName, true));
        return workspaceFolder === undefined ? "#" : workspaceFolder.uri.toString();
    }
    getWorkspaceStateIdByWorkspaceFolder(workspaceFolder) {
        return workspaceFolder === undefined ? "#" : workspaceFolder.uri.toString();
    }
    getWorkspaceStateByFileName(fileName) {
        const workspaceFolder = vscode.workspace.getWorkspaceFolder(vscode.Uri.file(fileName));
        const workspaceFolderId = workspaceFolder === undefined ? "#" : workspaceFolder.uri.toString();
        let workspaceState = this.workspaceStates.get(workspaceFolderId);
        if (workspaceState !== undefined) {
            if (workspaceState.ensureNotDirty()) {
                workspaceState = undefined;
            }
        }
        if (workspaceState === undefined) {
            workspaceState = new WorkspaceState(workspaceFolder, workspaceFolderId);
            this.workspaceStates.set(workspaceFolderId, workspaceState);
        }
        return workspaceState;
    }
    getWorkspaceStateById(workspaceFolderId, workspaceFolder = undefined) {
        let workspaceState = this.workspaceStates.get(workspaceFolderId);
        if (workspaceState !== undefined) {
            if (workspaceState.ensureNotDirty()) {
                workspaceState = undefined;
            }
        }
        if (workspaceState === undefined) {
            if (workspaceFolder !== undefined) {
                workspaceState = new WorkspaceState(workspaceFolder, workspaceFolderId);
                this.workspaceStates.set(workspaceFolderId, workspaceState);
            }
        }
        return workspaceState;
    }
    addWorkspaceFolder(workspaceFolder) {
        return this.getWorkspaceStateById(this.getWorkspaceStateIdByWorkspaceFolder(workspaceFolder), workspaceFolder);
    }
    removeWorkspaceFolder(workspaceFolder) {
        const workspaceFolderId = this.getWorkspaceStateIdByWorkspaceFolder(workspaceFolder);
        let workspaceState = this.workspaceStates.get(workspaceFolderId);
        if (workspaceState !== undefined) {
            workspaceState.dispose();
        }
        this.workspaceStates.delete(workspaceFolderId);
    }
    dispose() {
        this.workspaceStates.forEach((workspaceState) => {
            workspaceState.dispose();
        });
        this.workspaceStates.clear();
    }
}
exports.DetailsExtensionState = DetailsExtensionState;
class WorkspaceState {
    constructor(workspaceFolder, id) {
        this.workspaceFolder = workspaceFolder;
        this.id = id;
        this.isStarted = undefined;
        this.cacheCode = [];
        this.cacheDetails = [];
        this.workspaceFolderUri = this.workspaceFolder?.uri;
    }
    ensureNotDirty(workspaceFolder = undefined) {
        if (this.workspaceFolder === undefined && workspaceFolder === undefined) {
            return false;
        }
        if (this.workspaceFolder === undefined && workspaceFolder !== undefined) {
            return true;
        }
        const uri = this.workspaceFolder?.uri;
        if (this.workspaceFolderUri === uri) {
            // OK
        }
        else if (this.workspaceFolderUri === undefined && uri === undefined) {
            // OK
        }
        else {
            this.workspaceFolderUri = uri;
            this.dispose();
            this.start();
        }
        return false;
    }
    async start() {
        if (this.workspaceFolder === undefined) {
            return false;
        }
        else {
            if (this.isStarted === undefined) {
                this.isStarted = new Promise(async (resolve, reject) => {
                    try {
                        if (this.workspaceFolder === undefined) {
                            resolve();
                            return;
                        }
                        if (this.detailsFileWatcher === undefined) {
                            const found = await vscode.workspace.findFiles(new vscode.RelativePattern(this.workspaceFolderUri, "**/details.json"), "**/node_modules/**", 2);
                            if (found.length > 0) {
                                this.detailsFileWatcher = new DetailsFileWatcher_1.DetailsFileWatcher(this.workspaceFolder, found[0], (details) => {
                                    this.setDetailsJson(details);
                                });
                            }
                            else {
                                this.detailsFileWatcher = new DetailsFileWatcher_1.DetailsFileWatcher(this.workspaceFolder, vscode.Uri.joinPath(this.workspaceFolder.uri, "details.json"), (details) => {
                                    this.setDetailsJson(details);
                                });
                            }
                            this.detailsFileWatcher.start();
                        }
                        if (this.cacheWatcherDetails === undefined) {
                            this.cacheWatcherDetails = new CacheFileWatcher_1.CacheFileWatcher(this.workspaceFolder, (cache) => {
                                this.setCacheDetails(cache);
                            });
                        }
                        if (this.cacheWatcherCode === undefined) {
                            this.cacheWatcherCode = new CacheFileWatcher_1.CacheFileWatcher(this.workspaceFolder, (cache) => {
                                this.setCacheCode(cache);
                            });
                        }
                        if (this.detailsFileWatcher !== undefined) {
                            await this.detailsFileWatcher.start();
                        }
                        resolve();
                    }
                    catch (e) {
                        reject(e);
                    }
                });
            }
        }
        await this.isStarted;
        return true;
    }
    setDetailsJson(details) {
        this.detailsConfiguration = details?.DetailsConfiguration;
        this.detailsConfigurationUri = details?.detailsConfigurationUri;
        this.detailsRoot = details?.DetailsRoot;
        this.detailsRootUri = details?.detailsRootUri;
        this.solutionFile = details?.SolutionFile;
        this.detailsFolder = details?.DetailsFolder;
        this.detailsFolderUri = details?.detailsFolderUri;
        if (this.onStateChanged) {
            this.onStateChanged();
        }
        if (this.detailsFolderUri !== undefined) {
            if (this.cacheWatcherDetails !== undefined) {
                this.cacheWatcherDetails.start(vscode.Uri.joinPath(this.detailsFolderUri, "cache-details-links.json"));
            }
            if (this.cacheWatcherCode !== undefined) {
                this.cacheWatcherCode.start(vscode.Uri.joinPath(this.detailsFolderUri, "cache-code-links.json"));
            }
        }
    }
    setCacheCode(cache) {
        this.cacheCode = cache || [];
    }
    setCacheDetails(cache) {
        this.cacheDetails = cache || [];
    }
    dispose() {
        if (this.detailsFileWatcher) {
            this.detailsFileWatcher.dispose();
            this.detailsFileWatcher = undefined;
        }
        if (this.cacheWatcherCode) {
            this.cacheWatcherCode.dispose();
            this.cacheWatcherCode = undefined;
        }
        if (this.cacheWatcherDetails) {
            this.cacheWatcherDetails.dispose();
            this.cacheWatcherDetails = undefined;
        }
        this.isStarted = undefined;
    }
    async getDetailsRoot(token) {
        if (await this.start()) {
            return this.detailsRootUri;
        }
        return undefined;
    }
    async getDetailsFolder(token) {
        if (await this.start()) {
            return this.detailsFolderUri;
        }
        return undefined;
    }
    async getDetailsPath(targetPath, token) {
        const detailsRoot = await this.getDetailsFolder(token);
        if (detailsRoot === undefined) {
            return undefined;
        }
        // TODO: lookup in cache or service
        // hack for now
        const pos = targetPath.indexOf(".md");
        if (pos > 0) {
            targetPath = targetPath.substring(0, pos + 3);
        }
        const targetUri = vscode.Uri.joinPath(detailsRoot, targetPath);
        return targetUri;
    }
    async getCodePath(targetPath, token) {
        const detailsRoot = await this.getDetailsRoot(token);
        if (detailsRoot === undefined) {
            return undefined;
        }
        // TODO: lookup in cache or service
        // hack for now
        const match = targetPath.match(/([-._A-Z0-9/\\]+)(\.cs)|(\.ts)|(\.html)/i);
        if (match === null) {
            return undefined;
        }
        const found = await vscode.workspace.findFiles(new vscode.RelativePattern(this.workspaceFolderUri, "**/" + match[0]));
        if (found.length > 0) {
            return found[0];
        }
        return undefined;
        // const targetUri = vscode.Uri.joinPath(detailsRoot, targetPath);
        // return targetUri;
    }
}
exports.WorkspaceState = WorkspaceState;
//# sourceMappingURL=DetailsExtensionState.js.map