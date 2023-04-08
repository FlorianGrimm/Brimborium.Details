//import { env, ExtensionContext, workspace, window, Disposable, commands, Uri, version as vscodeVersion, WorkspaceFolder, LogOutputChannel, l10n, LogLevel } from 'vscode';
import * as vscode from "vscode";
// import { CacheFileWatcher, CacheItem } from "./CacheFileWatcher";
// import { DetailsFileWatcher } from "./DetailsFileWatcher";
import { WorkspaceState } from "./WorkspaceState";
import type { DetailsJSON } from "./DetailsJSON";

export class DetailsExtensionState {
  private workspaceStates: Map<string, WorkspaceState> = new Map<
    string,
    WorkspaceState
  >();
  private disposables: vscode.Disposable[] = [];
  logger: vscode.LogOutputChannel;
  constructor(
    private context: vscode.ExtensionContext
  ) {

    //const logger = vscode.window.createOutputChannel('Log', { log: true });
    const logger = vscode.window.createOutputChannel('Details', { log: true });
    this.disposables.push(logger);
  
    const onDidChangeLogLevel = (logLevel: vscode.LogLevel) => {
      logger.appendLine(`Log level: ${vscode.LogLevel[logLevel]}`, );
    };
    this.disposables.push(logger.onDidChangeLogLevel(onDidChangeLogLevel));
    onDidChangeLogLevel(logger.logLevel);
    this.logger=logger;
  }
  log(message:string){
    this.logger.appendLine(message);
    console.log(message);
  }
  async initialize() {
    this.log("DetailsExtensionState.initialize");
    
    this.context.subscriptions.push(
      vscode.workspace.onDidChangeWorkspaceFolders((event) => {
        event.removed.forEach((folder) => {
          this.removeWorkspaceFolder(folder);
        });
        event.added.forEach((folder) => {
          this.addWorkspaceFolder(folder);
        });
      })
    );
    vscode.workspace.workspaceFolders?.forEach((folder) => {
      this.addWorkspaceFolder(folder);
    });

  }

  getWorkspaceStateIdByFileName(fileName: string) {
    const workspaceFolder = vscode.workspace.getWorkspaceFolder(
      //vscode.Uri.parse(fileName, true)
      vscode.Uri.file(fileName)
    );
    return workspaceFolder === undefined ? "#" : workspaceFolder.uri.toString();
  }

  getWorkspaceStateIdByWorkspaceFolder(
    workspaceFolder: vscode.WorkspaceFolder | undefined
  ) {
    return workspaceFolder === undefined ? "#" : workspaceFolder.uri.toString();
  }

   getWorkspaceStateByFileName(fileName: string) {
    const workspaceStateId = this.getWorkspaceStateIdByFileName(fileName);
    if (this.workspaceStates.has(workspaceStateId)) {
      const workspaceState = this.workspaceStates.get(workspaceStateId);
      return workspaceState;
    }
    return undefined;
  //   const workspaceFolderId =
  //     workspaceFolder === undefined ? "#" : workspaceFolder.uri.toString();
  //   let workspaceState = this.workspaceStates.get(workspaceFolderId);
  //   if (workspaceState !== undefined) {
  //     if (workspaceState.ensureNotDirty()) {
  //       workspaceState = undefined;
  //     }
  //   }
  //   if (workspaceState === undefined) {
  //     workspaceState = new WorkspaceState(workspaceFolder, workspaceFolderId);
  //     this.workspaceStates.set(workspaceFolderId, workspaceState);
  //   }
  //   return workspaceState;
  }

  // getWorkspaceStateById(
  //   workspaceFolderId: string,
  //   workspaceFolder: vscode.WorkspaceFolder | undefined = undefined
  // ) {
  //   let workspaceState = this.workspaceStates.get(workspaceFolderId);
  //   if (workspaceState !== undefined) {
  //     if (workspaceState.ensureNotDirty()) {
  //       workspaceState = undefined;
  //     }
  //   }
  //   if (workspaceState === undefined) {
  //     if (workspaceFolder !== undefined) {
  //       workspaceState = new WorkspaceState(workspaceFolder, workspaceFolderId);
  //       this.workspaceStates.set(workspaceFolderId, workspaceState);
  //     }
  //   }
  //   return workspaceState;
  // }

  async addWorkspaceFolder(workspaceFolder: vscode.WorkspaceFolder) {
    const workspaceStateId = this.getWorkspaceStateIdByWorkspaceFolder(workspaceFolder);
        if (this.workspaceStates.has(workspaceStateId)) {
      const oldWorkspaceState = this.workspaceStates.get(workspaceStateId);
      if (oldWorkspaceState !== undefined){
        oldWorkspaceState.dispose();
      }
    }
    {
      const nextWorkspaceState = new WorkspaceState(new StateCommunication(this, workspaceStateId), this.logger, workspaceFolder, workspaceStateId);
      this.workspaceStates.set(workspaceStateId, nextWorkspaceState);
      await nextWorkspaceState.initialize();
      return nextWorkspaceState;
    }    
  }

  removeWorkspaceFolder(workspaceFolder: vscode.WorkspaceFolder) {
    const workspaceStateId = this.getWorkspaceStateIdByWorkspaceFolder(workspaceFolder);
    if (this.workspaceStates.has(workspaceStateId)) {
      let workspaceState = this.workspaceStates.get(workspaceStateId);
      if (workspaceState !== undefined) {
        workspaceState.dispose();
      }
    }
    this.workspaceStates.delete(workspaceStateId);
  }

  dispose() {
    this.workspaceStates.forEach((workspaceState) => {
      workspaceState.dispose();
    });
    this.workspaceStates.clear();
    this.disposables.forEach((disposable) => {
      disposable.dispose();
    });
    this.disposables = [];
  }

  toString() {
    let result = "";
    this.workspaceStates.forEach((workspaceState) => {
      result += workspaceState.toString()+";";
    });
    return result;
  }

  // setDetails(workspaceStateId:string, details: DetailsJSON | undefined){
  //   TODO: x
  // }
  
  getDetails():DetailsJSON[] {
    const result:DetailsJSON[]=[];
    for (const workspaceState of this.workspaceStates.values()) {
      const details = workspaceState.getDetails();
      if (details !== undefined){
        result.push(details);
      }
    }
    return result;
  }
  
  getListWorkspaceStateWithDetails():WorkspaceState[]{
    const result:WorkspaceState[]=[];
    for (const workspaceState of this.workspaceStates.values()) {
      const details = workspaceState.getDetails();
      if (details !== undefined){
        result.push(workspaceState);
      }
    }
    return result;
  }

  getWorkspaceStateWithDetails():WorkspaceState | undefined{
    const result:WorkspaceState[]=[];
    for (const workspaceState of this.workspaceStates.values()) {
      const details = workspaceState.getDetails();
      if (details !== undefined){
        result.push(workspaceState);
      }
    }
    if (result.length === 1){
      return result[0];
    }
    // TODO: think about it
    return undefined;
  }

  async getDetailsFilePath(
    workspaceStateDetails: WorkspaceState,
    workspaceStateFile: WorkspaceState,
    targetPath:string, 
    token: vscode.CancellationToken): Promise<vscode.Uri | undefined> {
    return await workspaceStateDetails.getDetailsFilePath(workspaceStateFile, targetPath, token);
  }
  async getCodeFilePath(
    workspaceStateDetails: WorkspaceState,
    workspaceStateFile: WorkspaceState,
    targetPath:string, 
    token: vscode.CancellationToken): Promise<vscode.Uri | undefined> {
    return await workspaceStateDetails.getCodeFilePath(workspaceStateFile, targetPath, token);
  }
  
}

export class StateCommunication {
  constructor(
    private readonly detailsExtensionState: DetailsExtensionState,
    private readonly workspaceStateId: string
  ) {
  }
  // setDetails(details: DetailsJSON | undefined){
  //   this.detailsExtensionState.setDetails(this.workspaceStateId, details);
  // }
}

// export class WorkspaceState2 {
//   private detailsConfiguration: string | undefined;
//   private detailsRoot: string | undefined;
//   private solutionFile: string | undefined;
//   private detailsFolder: string | undefined;

//   private detailsRootUri: vscode.Uri | undefined;
//   private detailsConfigurationUri: vscode.Uri | undefined;
//   private detailsFolderUri: vscode.Uri | undefined;

//   onStateChanged: (() => void) | undefined;
//   private workspaceFolderUri: vscode.Uri | undefined;

//   constructor(
//     private readonly workspaceFolder: vscode.WorkspaceFolder | undefined,
//     public readonly id: string
//   ) {
//     this.workspaceFolderUri = this.workspaceFolder?.uri;
//   }

//   // getFolderUri() {
//   //   if (this.workspaceFolder === undefined) {
//   //     return undefined;
//   //   } else {
//   //     return this.workspaceFolder.uri;
//   //   }
//   // }
//   private detailsFileWatcher: DetailsFileWatcher | undefined;
//   private cacheWatcherDetails: CacheFileWatcher | undefined;
//   private cacheWatcherCode: CacheFileWatcher | undefined;

//   ensureNotDirty(
//     workspaceFolder: vscode.WorkspaceFolder | undefined = undefined
//   ) {
//     if (this.workspaceFolder === undefined && workspaceFolder === undefined) {
//       return false;
//     }
//     if (this.workspaceFolder === undefined && workspaceFolder !== undefined) {
//       return true;
//     }
//     const uri = this.workspaceFolder?.uri;
//     if (this.workspaceFolderUri === uri) {
//       // OK
//     } else if (this.workspaceFolderUri === undefined && uri === undefined) {
//       // OK
//     } else {
//       this.workspaceFolderUri = uri;
//       this.dispose();
//       this.start();
//     }
//     return false;
//   }

//   private isStarted: Promise<void> | undefined = undefined;
//   async start() {
//     if (this.workspaceFolder === undefined) {
//       return false;
//     } else {
//       if (this.isStarted === undefined) {
//         this.isStarted = new Promise(async (resolve, reject) => {
//           try {
//             if (this.workspaceFolder === undefined) {
//               resolve();
//               return;
//             }
//             if (this.detailsFileWatcher === undefined) {
//               const found = await vscode.workspace.findFiles(
//                 new vscode.RelativePattern(
//                   this.workspaceFolderUri!,
//                   "**/details.json"
//                 ),
//                 "**/node_modules/**",
//                 2
//               );
//               if (found.length > 0) {
//                 this.detailsFileWatcher = new DetailsFileWatcher(
//                   this.workspaceFolder,
//                   found[0],
//                   (details) => {
//                     this.setDetailsJson(details);
//                   }
//                 );
//               } else {
//                 this.detailsFileWatcher = new DetailsFileWatcher(
//                   this.workspaceFolder,
//                   vscode.Uri.joinPath(this.workspaceFolder.uri, "details.json"),
//                   (details) => {
//                     this.setDetailsJson(details);
//                   }
//                 );
//               }
//               this.detailsFileWatcher.start();
//             }
//             if (this.cacheWatcherDetails === undefined) {
//               this.cacheWatcherDetails = new CacheFileWatcher(
//                 this.workspaceFolder,
//                 (cache) => {
//                   this.setCacheDetails(cache);
//                 }
//               );
//             }
//             if (this.cacheWatcherCode === undefined) {
//               this.cacheWatcherCode = new CacheFileWatcher(
//                 this.workspaceFolder,
//                 (cache) => {
//                   this.setCacheCode(cache);
//                 }
//               );
//             }
//             if (this.detailsFileWatcher !== undefined) {
//               await this.detailsFileWatcher.start();
//             }

//             resolve();
//           } catch (e) {
//             reject(e);
//           }
//         });
//       }
//     }
//     await this.isStarted;
//     return true;
//   }

//   setDetailsJson(details: DetailsJSON | undefined): void {
//     this.detailsConfiguration = details?.DetailsConfiguration;
//     this.detailsConfigurationUri = details?.detailsConfigurationUri;
//     this.detailsRoot = details?.DetailsRoot;
//     this.detailsRootUri = details?.detailsRootUri;
//     this.solutionFile = details?.SolutionFile;
//     this.detailsFolder = details?.DetailsFolder;
//     this.detailsFolderUri = details?.detailsFolderUri;

//     if (this.onStateChanged) {
//       this.onStateChanged();
//     }

//     if (this.detailsFolderUri !== undefined) {
//       if (this.cacheWatcherDetails !== undefined) {
//         this.cacheWatcherDetails.start(
//           vscode.Uri.joinPath(this.detailsFolderUri, "cache-details-links.json")
//         );
//       }
//       if (this.cacheWatcherCode !== undefined) {
//         this.cacheWatcherCode.start(
//           vscode.Uri.joinPath(this.detailsFolderUri, "cache-code-links.json")
//         );
//       }
//     }
//   }

//   cacheCode: CacheItem[] = [];
//   setCacheCode(cache: CacheItem[]) {
//     this.cacheCode = cache || [];
//   }

//   cacheDetails: CacheItem[] = [];
//   setCacheDetails(cache: CacheItem[]) {
//     this.cacheDetails = cache || [];
//   }

//   dispose() {
//     if (this.detailsFileWatcher) {
//       this.detailsFileWatcher.dispose();
//       this.detailsFileWatcher = undefined;
//     }
//     if (this.cacheWatcherCode) {
//       this.cacheWatcherCode.dispose();
//       this.cacheWatcherCode = undefined;
//     }
//     if (this.cacheWatcherDetails) {
//       this.cacheWatcherDetails.dispose();
//       this.cacheWatcherDetails = undefined;
//     }
//     this.isStarted = undefined;
//   }

//   async getDetailsRoot(token: vscode.CancellationToken) {
//     if (await this.start()) {
//       //return this.detailsRootUri;
//       // TODO: bad hack: get rid of this.
//       if (this.detailsFolderUri === undefined) {
//         return this.detailsFolderUri = vscode.Uri.joinPath( this.workspaceFolderUri!, "details");
//       } else{
//         return this.detailsFolderUri;
//       }
//     }
//     return undefined;
//   }

//   async getDetailsFolder(token: vscode.CancellationToken) {
//     if (await this.start()) {
//       // TODO: bad hack: get rid of this.
//       if (this.detailsFolderUri === undefined) {
//         return this.detailsFolderUri = vscode.Uri.joinPath( this.workspaceFolderUri!, "details");
//       } else{
//         return this.detailsFolderUri;
//       }
//     }
//     return undefined;
//   }

//   async getDetailsPath(targetPath: string, token: vscode.CancellationToken) {
//     const detailsRoot = await this.getDetailsFolder(token);
//     if (detailsRoot === undefined) {
//       return undefined;
//     }
//     // TODO: lookup in cache or service

//     // hack for now
//     const pos = targetPath.indexOf(".md");
//     if (pos > 0) {
//       targetPath = targetPath.substring(0, pos + 3);
//     }

//     const targetUri = vscode.Uri.joinPath(detailsRoot, targetPath);
//     return targetUri;
//   }

//   async getCodePath(targetPath: string, token: vscode.CancellationToken) {
//     const detailsRoot = await this.getDetailsRoot(token);
//     // if (detailsRoot === undefined) {
//     //   return undefined;
//     // }
//     // TODO: lookup in cache or service

//     // hack for now
//     const match = targetPath.match(/([-._A-Z0-9/\\]+)(\.cs)|(\.ts)|(\.html)/i);
//     if (match === null) {
//       return undefined;
//     }

//     {
//       const found = await vscode.workspace.findFiles(
//         new vscode.RelativePattern(this.workspaceFolderUri!, match[0])
//       );
//       if (found.length > 0) {
//         return found[0];
//       }
//     }
//     {
//       const found = await vscode.workspace.findFiles(
//         new vscode.RelativePattern(this.workspaceFolderUri!, "**/" + match[0])
//       );
//       if (found.length > 0) {
//         return found[0];
//       }
//     }
    
//     if (detailsRoot !== undefined) {
//       const found = await vscode.workspace.findFiles(
//         new vscode.RelativePattern(detailsRoot, "**/" + match[0])
//       );
//       if (found.length > 0) {
//         return found[0];
//       }
//     }
//     return undefined;
//     // const targetUri = vscode.Uri.joinPath(detailsRoot, targetPath);
//     // return targetUri;
//   }
// }
