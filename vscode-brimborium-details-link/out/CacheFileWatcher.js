"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.CacheFileWatcher = void 0;
const vscode = require("vscode");
class CacheFileWatcher {
    constructor(workspaceFolder, setCache) {
        this.workspaceFolder = workspaceFolder;
        this.setCache = setCache;
        this.isStarted = undefined;
    }
    async start(uri) {
        if (this.currentCacheUri &&
            this.currentCacheUri.fsPath === uri.fsPath &&
            this.isStarted !== undefined) {
            await this.isStarted;
        }
        if (this.isStarted === undefined) {
            this.dispose();
            this.isStarted = new Promise(async (resolve, reject) => {
                try {
                    this.currentCacheUri = uri;
                    const pattern = new vscode.RelativePattern(this.workspaceFolder, vscode.workspace.asRelativePath(uri));
                    this.watcher = vscode.workspace
                        .createFileSystemWatcher(pattern)
                        .onDidChange((e) => {
                        if (e.toString() === uri.toString()) {
                            this.readCacheJson(e);
                        }
                    });
                    await this.readCacheJson(uri);
                    resolve();
                }
                catch (e) {
                    reject(e);
                }
            });
        }
        await this.isStarted;
    }
    dispose() {
        if (this.watcher) {
            this.watcher.dispose();
            this.watcher = undefined;
        }
        this.isStarted = undefined;
    }
    async readCacheJson(e) {
        var data = await vscode.workspace.fs.readFile(e);
        const content = Buffer.from(data).toString("utf8");
        const cacheRead = JSON.parse(content);
        const cacheNext = [];
        for (const item of cacheRead) {
            if (typeof item.name === "string" &&
                item.name &&
                typeof item.path === "string" &&
                item.path &&
                (typeof item.line === "number" || item.line === undefined)) {
                cacheNext.push(item);
            }
        }
        this.setCache(cacheNext);
    }
}
exports.CacheFileWatcher = CacheFileWatcher;
//# sourceMappingURL=CacheFileWatcher.js.map