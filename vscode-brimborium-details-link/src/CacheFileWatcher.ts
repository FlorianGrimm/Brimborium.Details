import * as vscode from "vscode";

export type CacheItem = {
  name: string;
  path: string;
  line?: number | undefined;
};

export class CacheFileWatcher {
  private watcher: vscode.Disposable | undefined;
  private currentCacheUri: vscode.Uri | undefined;
  constructor(
    private workspaceFolder: vscode.WorkspaceFolder,
    private setCache: (cache: CacheItem[]) => void
    ) {}

  private isStarted: Promise<void> | undefined = undefined;

  async start(uri: vscode.Uri) {
    if (
      this.currentCacheUri &&
      this.currentCacheUri.fsPath === uri.fsPath &&
      this.isStarted !== undefined
    ) {
      await this.isStarted;
    }
    if (this.isStarted === undefined) {
      this.dispose();
      this.isStarted = new Promise(async (resolve, reject) => {
        try {
          this.currentCacheUri = uri;
          const pattern=new vscode.RelativePattern(
            this.workspaceFolder, 
            vscode.workspace.asRelativePath(uri));
          this.watcher = vscode.workspace
            .createFileSystemWatcher(pattern)
            .onDidChange((e) => {
              if (e.toString() === uri.toString()) {
                this.readCacheJson(e);
              }
            });
          await this.readCacheJson(uri);
          resolve();
        } catch (e) {
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

  async readCacheJson(e: vscode.Uri) {
    var data = await vscode.workspace.fs.readFile(e);
    const content = Buffer.from(data).toString("utf8");
    const cacheRead = JSON.parse(content) as CacheItem[];
    const cacheNext: CacheItem[] = [];
    for (const item of cacheRead) {
      if (
        typeof item.name === "string" &&
        item.name &&
        typeof item.path === "string" &&
        item.path &&
        (typeof item.line === "number" || item.line === undefined)
      ) {
        cacheNext.push(item);
      }
    }
    this.setCache(cacheNext);
  }
}
