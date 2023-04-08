import * as vscode from "vscode";
vscode.Breakpoint;
import { DetailsJSON } from "./DetailsJSON";
import type { WorkspaceStateCommunication } from "./WorkspaceState";
import { fileUtilities } from "./FileUtilties";

//import { isEqual, isEqualOrParent, normalize } from 'vs/base/common/paths';
//isEqualOrParent()
export class DetailsFileWatcher {
  private watcher: vscode.Disposable | undefined;
  private currentDetails: DetailsJSON | undefined;

  constructor(
    private readonly workspaceStateCommunication: WorkspaceStateCommunication,
    private readonly logger: vscode.LogOutputChannel,
    private readonly workspaceFolder: vscode.WorkspaceFolder
  ) {
  }

  log(message: string) {
    this.logger.appendLine(message);
    console.log(message);
  }

  async initialize() {
    const uri = vscode.Uri.joinPath(this.workspaceFolder.uri, "details.json");
    if (await fileUtilities.exists(uri, vscode.FileType.File)) {
      await this.readDetailsJson(uri);
    }
    this.watcher = vscode.workspace
      .createFileSystemWatcher(
        // new vscode.RelativePattern(this.workspaceFolder, this.detailsJsonUri)
        uri.fsPath
      )
      .onDidChange((uri) => {
        this.log(`DetailsFileWatcher.initialize.onDidChange uri: ${uri.toString()}`);
        this.readDetailsJson(uri);
      })
      ;
  }

  dispose() {
    if (this.watcher) {
      this.watcher.dispose();
    }
  }

  async readDetailsJson(uri: vscode.Uri) {
    const folderUri = fileUtilities.parentUri(uri);
    this.log(`DetailsFileWatcher.readDetailsJson uri: ${uri.toString()}`);
    if (await fileUtilities.exists(uri, vscode.FileType.File)) {
      var detailsData = await vscode.workspace.fs.readFile(uri);
      const detailsContent = Buffer.from(detailsData).toString("utf8");
      const detailsRead = JSON.parse(detailsContent) as DetailsJSON;
      const detailsNext: DetailsJSON = {
        /*eslint-disable */
        detailsConfigurationUri: undefined,
        detailsRootUri: undefined,
        detailsFolderUri: undefined,

        DetailsConfiguration: "",
        DetailsRoot: "",
        SolutionFile: "",
        DetailsFolder: "",
        /*eslint-enable */
      };

      if (detailsRead.DetailsConfiguration === "") {
        detailsRead.DetailsConfiguration = undefined;
      }
      if (detailsRead.DetailsRoot === ".") {
        detailsRead.DetailsRoot = "";
      }
      if (detailsRead.DetailsFolder === "") {
        detailsRead.DetailsFolder = "details";
      }

      if (
        typeof detailsRead.DetailsRoot === "string" &&
        detailsRead.DetailsRoot
      ) {
        
        let detailsRootUri = vscode.Uri.joinPath(folderUri, detailsRead.DetailsRoot);
        detailsNext.detailsRootUri = detailsRootUri;
        detailsNext.DetailsRoot = detailsRootUri.fsPath;
      } else {
        let detailsRootUri = vscode.Uri.joinPath(folderUri);
        detailsNext.detailsRootUri = detailsRootUri;
        detailsNext.DetailsRoot = detailsRootUri.fsPath;
      }

      if (
        typeof detailsRead.DetailsConfiguration === "string" &&
        detailsRead.DetailsConfiguration
      ) {
        const detailsConfigurationUri = vscode.Uri.joinPath(
          folderUri,
          detailsRead.DetailsConfiguration
        );
        this.log(`DetailsFileWatcher.readDetailsJson detailsConfigurationUri: ${detailsConfigurationUri.toString()}`);
        detailsNext.detailsConfigurationUri = detailsConfigurationUri;
        detailsNext.DetailsConfiguration = detailsConfigurationUri.fsPath;
      }

      if (detailsRead.DetailsFolder) {
        if (detailsNext.detailsRootUri !== undefined) {
          detailsNext.detailsFolderUri = vscode.Uri.joinPath(detailsNext.detailsRootUri, detailsRead.DetailsFolder);

          detailsNext.DetailsFolder = detailsNext.detailsFolderUri.fsPath;
        }
      }

      if (
        this.currentDetails &&
        this.currentDetails.DetailsConfiguration ===
        detailsNext.DetailsConfiguration &&
        this.currentDetails.DetailsRoot === detailsNext.DetailsRoot &&
        this.currentDetails.SolutionFile === detailsNext.SolutionFile &&
        this.currentDetails.DetailsFolder === detailsNext.DetailsFolder
      ) {
        // No change
        return;
      }

      let currentDetails: DetailsJSON | undefined;
      if (
        detailsNext.detailsConfigurationUri &&
        detailsNext.detailsConfigurationUri !== this.currentDetails?.detailsConfigurationUri
      ) {
        currentDetails = detailsNext;
        // this.chainedWatcher = new DetailsFileWatcher2(
        //   this.workspaceFolder,
        //   detailsNext.detailsConfigurationUri,
        //   this.setDetails
        // );
      } else {
        // if (this.chainedWatcher !== undefined) {
        //   this.chainedWatcher.dispose();
        //   this.chainedWatcher = undefined;
        // }
        /*eslint-disable */
        currentDetails = {
          ...detailsNext,
          detailsConfigurationUri: uri,
          DetailsConfiguration: uri.toString(),
        };
        /*eslint-enable */
        // this.setDetails({
        //   ...detailsNext,
        //   detailsConfigurationUri: uri,
        //   DetailsConfiguration: uri.toString(),
        // });

      }
      // this.isStarted = Promise.resolve();
      this.setDetails(currentDetails);
    } else {
      this.setDetails(undefined);
    }
  }
  
  setDetails(details: DetailsJSON | undefined) {
    this.currentDetails = details;
    this.workspaceStateCommunication.setDetails(details);
  }
}

export class DetailsFileWatcher2 {
  private watcher: vscode.Disposable | undefined;
  private currentDetails: DetailsJSON | undefined;
  private chainedWatcher: DetailsFileWatcher2 | undefined;

  constructor(
    private workspaceFolder: vscode.WorkspaceFolder,
    private detailsJsonUri: vscode.Uri,
    private setDetails: (details: DetailsJSON | undefined) => void
  ) {
    this.watcher = vscode.workspace
      .createFileSystemWatcher(
        // new vscode.RelativePattern(this.workspaceFolder, this.detailsJsonUri)
        this.detailsJsonUri.fsPath
      )
      .onDidChange((e) => {
        this.readDetailsJson(e);
      });
  }

  private isStarted: Promise<void> | undefined = undefined;

  async start() {
    if (this.isStarted === undefined) {
      this.isStarted = new Promise(async (resolve, reject) => {
        try {

          var detailsUris = await vscode.workspace.findFiles(
            new vscode.RelativePattern(
              this.workspaceFolder,
              vscode.workspace.asRelativePath(this.detailsJsonUri))
          );
          if (detailsUris.length === 1) {
            await this.readDetailsJson(detailsUris[0]);
          }
          resolve();
        } catch (e) {
          reject(e);
        }
      });
    }
    await this.isStarted;
  }

  async doesFileExist(uri: vscode.Uri) {
    try {
      await vscode.workspace.fs.stat(uri);
      return true;
    } catch {
      return false;
    }
  }

  async readDetailsJson(e: vscode.Uri) {
    if (!this.doesFileExist(e)) {
      return;
    }
    var detailsData = await vscode.workspace.fs.readFile(e);
    const detailsContent = Buffer.from(detailsData).toString("utf8");
    const detailsRead = JSON.parse(detailsContent) as DetailsJSON;
    const detailsNext: DetailsJSON = {
      /*eslint-disable */
      detailsConfigurationUri: undefined,
      detailsRootUri: undefined,
      detailsFolderUri: undefined,

      DetailsConfiguration: "",
      DetailsRoot: "",
      SolutionFile: "",
      DetailsFolder: "",
      /*eslint-enable */
    };


    if (detailsRead.DetailsConfiguration === "") {
      detailsRead.DetailsConfiguration = undefined;
    }
    if (detailsRead.DetailsRoot === ".") {
      detailsRead.DetailsRoot = "";
    }
    if (detailsRead.DetailsFolder === "") {
      detailsRead.DetailsFolder = "details";
    }

    if (
      typeof detailsRead.DetailsRoot === "string" &&
      detailsRead.DetailsRoot
    ) {
      let detailsRootUri = vscode.Uri.joinPath(e, "..", detailsRead.DetailsRoot);
      detailsNext.detailsRootUri = detailsRootUri;
      detailsNext.DetailsRoot = detailsRootUri.fsPath;
    } else {
      let detailsRootUri = vscode.Uri.joinPath(e, "..");
      detailsNext.detailsRootUri = detailsRootUri;
      detailsNext.DetailsRoot = detailsRootUri.fsPath;
    }

    if (
      typeof detailsRead.DetailsConfiguration === "string" &&
      detailsRead.DetailsConfiguration
    ) {
      detailsNext.detailsConfigurationUri = vscode.Uri.joinPath(
        e,
        "..",
        detailsRead.DetailsConfiguration
      );
      detailsNext.DetailsConfiguration = detailsNext.detailsConfigurationUri.fsPath;
    }

    if (detailsRead.DetailsFolder) {
      if (detailsNext.detailsRootUri !== undefined) {
        detailsNext.detailsFolderUri = vscode.Uri.joinPath(detailsNext.detailsRootUri, detailsRead.DetailsFolder);
        detailsNext.DetailsFolder = detailsNext.detailsFolderUri.fsPath;
      }
    }

    if (
      this.currentDetails &&
      this.currentDetails.DetailsConfiguration ===
      detailsNext.DetailsConfiguration &&
      this.currentDetails.DetailsRoot === detailsNext.DetailsRoot &&
      this.currentDetails.SolutionFile === detailsNext.SolutionFile &&
      this.currentDetails.DetailsFolder === detailsNext.DetailsFolder
    ) {
      // No change
      return;
    }

    if (
      detailsNext.detailsConfigurationUri &&
      detailsNext.detailsConfigurationUri !==
      this.currentDetails?.detailsConfigurationUri
    ) {
      this.currentDetails = detailsNext;
      this.chainedWatcher = new DetailsFileWatcher2(
        this.workspaceFolder,
        detailsNext.detailsConfigurationUri,
        this.setDetails
      );
    } else {
      if (this.chainedWatcher !== undefined) {
        this.chainedWatcher.dispose();
        this.chainedWatcher = undefined;
      }
      this.currentDetails = detailsNext;
      /*eslint-disable */
      this.setDetails({
        ...detailsNext,
        detailsConfigurationUri: e,
        DetailsConfiguration: e.toString(),
      });

      /*eslint-enable */
    }
    this.isStarted = Promise.resolve();
  }

  dispose(): void {
    this.watcher?.dispose();
    this.watcher = undefined;
  }
}
