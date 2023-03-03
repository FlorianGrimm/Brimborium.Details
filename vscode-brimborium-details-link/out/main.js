"use strict";var L=Object.create;var w=Object.defineProperty;var E=Object.getOwnPropertyDescriptor;var b=Object.getOwnPropertyNames;var N=Object.getPrototypeOf,J=Object.prototype.hasOwnProperty;var I=(o,e)=>{for(var t in e)w(o,t,{get:e[t],enumerable:!0})},R=(o,e,t,i)=>{if(e&&typeof e=="object"||typeof e=="function")for(let s of b(e))!J.call(o,s)&&s!==t&&w(o,s,{get:()=>e[s],enumerable:!(i=E(e,s))||i.enumerable});return o};var h=(o,e,t)=>(t=o!=null?L(N(o)):{},R(e||!o||!o.__esModule?w(t,"default",{value:o,enumerable:!0}):t,o)),T=o=>R(w({},"__esModule",{value:!0}),o);var A={};I(A,{activate:()=>B,deactivate:()=>O});module.exports=T(A);var l=h(require("vscode"));var x=h(require("vscode"));var U=h(require("vscode")),f=class extends U.DocumentLink{constructor(t,i,s,a){super(t);this.filename=a;this.target=i,this.tooltip=s}};var k=class{constructor(e){this.state=e;this.regExpDetailscodeGlobal=new RegExp("detailscode://([^ #]*)([#][1-9]*)?([\xA7].*)?","g");this.regExpDetailscodeLocal=new RegExp("detailscode://([^ #]*)([#][1-9]*)?([\xA7].*)?")}provideDocumentLinks(e,t){let i=e.getText();if(t.isCancellationRequested)return;let s=[],a;for(;a=this.regExpDetailscodeGlobal.exec(i);){let n=e.positionAt(a.index),r=e.positionAt(a.index+a[0].length),v=new x.Range(n,r);if(s.push(new f(v,void 0,a[0],e.fileName)),t.isCancellationRequested)return}return s}async resolveDocumentLink(e,t){if(e.filename===void 0)return;let i=this.state.getWorkspaceStateByFileName(e.filename);if(i===void 0||await i.getDetailsRoot(t)===void 0||!e.tooltip)return;let a=e.tooltip.match(this.regExpDetailscodeLocal);if(a===null)return;let n=a[1];if(!n||t.isCancellationRequested)return;let r=await i.getCodePath(n,t);if(r!==void 0)return e.target=r,console.log("resolveDocumentLink: %s",r.toString()),e}};var W=h(require("vscode"));var D=class{constructor(e){this.state=e;let t="details://([^)#]*)([#][1-9]*)?([\xA7].*)?";this.regexpDetailsGlobal=new RegExp(t,"g"),this.regexpDetailsLocal=new RegExp(t)}provideDocumentLinks(e,t){let i=e.getText();if(t.isCancellationRequested)return;let s=this.state.getWorkspaceStateByFileName(e.fileName),a=[],n;for(;n=this.regexpDetailsGlobal.exec(i);){let r=e.positionAt(n.index),v=e.positionAt(n.index+n[0].length),P=new W.Range(r,v);if(a.push(new f(P,void 0,n[0],e.fileName)),t.isCancellationRequested)return}return a}async resolveDocumentLink(e,t){if(e.filename===void 0)return;let i=this.state.getWorkspaceStateByFileName(e.filename);if(i===void 0||await i.getDetailsRoot(t)===void 0||!e.tooltip)return;let a=e.tooltip.match(this.regexpDetailsLocal);if(a===null)return;let n=a[1];if(!n||t.isCancellationRequested)return;let r=await i.getDetailsPath(n,t);if(r!==void 0)return e.target=r,console.log("resolveDocumentLink: %s",r.toString()),e}};var d=h(require("vscode"));var g=h(require("vscode")),m=class{constructor(e,t){this.workspaceFolder=e;this.setCache=t;this.isStarted=void 0}async start(e){this.currentCacheUri&&this.currentCacheUri.fsPath===e.fsPath&&this.isStarted!==void 0&&await this.isStarted,this.isStarted===void 0&&(this.dispose(),this.isStarted=new Promise(async(t,i)=>{try{this.currentCacheUri=e;let s=new g.RelativePattern(this.workspaceFolder,g.workspace.asRelativePath(e));this.watcher=g.workspace.createFileSystemWatcher(s).onDidChange(a=>{a.toString()===e.toString()&&this.readCacheJson(a)}),await this.readCacheJson(e),t()}catch(s){i(s)}})),await this.isStarted}dispose(){this.watcher&&(this.watcher.dispose(),this.watcher=void 0),this.isStarted=void 0}async readCacheJson(e){var t=await g.workspace.fs.readFile(e);let i=Buffer.from(t).toString("utf8"),s=JSON.parse(i),a=[];for(let n of s)typeof n.name=="string"&&n.name&&typeof n.path=="string"&&n.path&&(typeof n.line=="number"||n.line===void 0)&&a.push(n);this.setCache(a)}};var c=h(require("vscode"));var p=class{constructor(e,t,i){this.workspaceFolder=e;this.detailsJsonUri=t;this.setDetails=i;this.isStarted=void 0;this.watcher=c.workspace.createFileSystemWatcher(this.detailsJsonUri.fsPath).onDidChange(s=>{this.readDetailsJson(s)})}async start(){this.isStarted===void 0&&(this.isStarted=new Promise(async(e,t)=>{try{var i=await c.workspace.findFiles(new c.RelativePattern(this.workspaceFolder,c.workspace.asRelativePath(this.detailsJsonUri)));i.length===1&&await this.readDetailsJson(i[0]),e()}catch(s){t(s)}})),await this.isStarted}async readDetailsJson(e){var t=await c.workspace.fs.readFile(e);let i=Buffer.from(t).toString("utf8"),s=JSON.parse(i),a={detailsConfigurationUri:void 0,detailsRootUri:void 0,detailsFolderUri:void 0,DetailsConfiguration:"",DetailsRoot:"",SolutionFile:"",DetailsFolder:""};if(s.DetailsConfiguration===""&&(s.DetailsConfiguration=void 0),s.DetailsRoot==="."&&(s.DetailsRoot=""),s.DetailsFolder===""&&(s.DetailsFolder="details"),typeof s.DetailsRoot=="string"&&s.DetailsRoot){let n=c.Uri.joinPath(e,"..",s.DetailsRoot);a.detailsRootUri=n,a.DetailsRoot=n.fsPath}else{let n=c.Uri.joinPath(e,"..");a.detailsRootUri=n,a.DetailsRoot=n.fsPath}typeof s.DetailsConfiguration=="string"&&s.DetailsConfiguration&&(a.detailsConfigurationUri=c.Uri.joinPath(e,"..",s.DetailsConfiguration),a.DetailsConfiguration=a.detailsConfigurationUri.fsPath),s.DetailsFolder&&a.detailsRootUri!==void 0&&(a.detailsFolderUri=c.Uri.joinPath(a.detailsRootUri,s.DetailsFolder),a.DetailsFolder=a.detailsFolderUri.fsPath),!(this.currentDetails&&this.currentDetails.DetailsConfiguration===a.DetailsConfiguration&&this.currentDetails.DetailsRoot===a.DetailsRoot&&this.currentDetails.SolutionFile===a.SolutionFile&&this.currentDetails.DetailsFolder===a.DetailsFolder)&&(a.detailsConfigurationUri&&a.detailsConfigurationUri!==this.currentDetails?.detailsConfigurationUri?(this.currentDetails=a,this.chainedWatcher=new p(this.workspaceFolder,a.detailsConfigurationUri,this.setDetails)):(this.chainedWatcher!==void 0&&(this.chainedWatcher.dispose(),this.chainedWatcher=void 0),this.currentDetails=a,this.setDetails({...a,detailsConfigurationUri:e,DetailsConfiguration:e.toString()})),this.isStarted=Promise.resolve())}dispose(){this.watcher?.dispose(),this.watcher=void 0}};var F=class{constructor(){this.workspaceStates=new Map}getWorkspaceStateIdByFileName(e){let t=d.workspace.getWorkspaceFolder(d.Uri.parse(e,!0));return t===void 0?"#":t.uri.toString()}getWorkspaceStateIdByWorkspaceFolder(e){return e===void 0?"#":e.uri.toString()}getWorkspaceStateByFileName(e){let t=d.workspace.getWorkspaceFolder(d.Uri.file(e)),i=t===void 0?"#":t.uri.toString(),s=this.workspaceStates.get(i);return s!==void 0&&s.ensureNotDirty()&&(s=void 0),s===void 0&&(s=new C(t,i),this.workspaceStates.set(i,s)),s}getWorkspaceStateById(e,t=void 0){let i=this.workspaceStates.get(e);return i!==void 0&&i.ensureNotDirty()&&(i=void 0),i===void 0&&t!==void 0&&(i=new C(t,e),this.workspaceStates.set(e,i)),i}addWorkspaceFolder(e){return this.getWorkspaceStateById(this.getWorkspaceStateIdByWorkspaceFolder(e),e)}removeWorkspaceFolder(e){let t=this.getWorkspaceStateIdByWorkspaceFolder(e),i=this.workspaceStates.get(t);i!==void 0&&i.dispose(),this.workspaceStates.delete(t)}dispose(){this.workspaceStates.forEach(e=>{e.dispose()}),this.workspaceStates.clear()}},C=class{constructor(e,t){this.workspaceFolder=e;this.id=t;this.isStarted=void 0;this.cacheCode=[];this.cacheDetails=[];this.workspaceFolderUri=this.workspaceFolder?.uri}ensureNotDirty(e=void 0){if(this.workspaceFolder===void 0&&e===void 0)return!1;if(this.workspaceFolder===void 0&&e!==void 0)return!0;let t=this.workspaceFolder?.uri;return this.workspaceFolderUri===t||this.workspaceFolderUri===void 0&&t===void 0||(this.workspaceFolderUri=t,this.dispose(),this.start()),!1}async start(){return this.workspaceFolder===void 0?!1:(this.isStarted===void 0&&(this.isStarted=new Promise(async(e,t)=>{try{if(this.workspaceFolder===void 0){e();return}if(this.detailsFileWatcher===void 0){let i=await d.workspace.findFiles(new d.RelativePattern(this.workspaceFolderUri,"**/details.json"),"**/node_modules/**",2);i.length>0?this.detailsFileWatcher=new p(this.workspaceFolder,i[0],s=>{this.setDetailsJson(s)}):this.detailsFileWatcher=new p(this.workspaceFolder,d.Uri.joinPath(this.workspaceFolder.uri,"details.json"),s=>{this.setDetailsJson(s)}),this.detailsFileWatcher.start()}this.cacheWatcherDetails===void 0&&(this.cacheWatcherDetails=new m(this.workspaceFolder,i=>{this.setCacheDetails(i)})),this.cacheWatcherCode===void 0&&(this.cacheWatcherCode=new m(this.workspaceFolder,i=>{this.setCacheCode(i)})),this.detailsFileWatcher!==void 0&&await this.detailsFileWatcher.start(),e()}catch(i){t(i)}})),await this.isStarted,!0)}setDetailsJson(e){this.detailsConfiguration=e?.DetailsConfiguration,this.detailsConfigurationUri=e?.detailsConfigurationUri,this.detailsRoot=e?.DetailsRoot,this.detailsRootUri=e?.detailsRootUri,this.solutionFile=e?.SolutionFile,this.detailsFolder=e?.DetailsFolder,this.detailsFolderUri=e?.detailsFolderUri,this.onStateChanged&&this.onStateChanged(),this.detailsFolderUri!==void 0&&(this.cacheWatcherDetails!==void 0&&this.cacheWatcherDetails.start(d.Uri.joinPath(this.detailsFolderUri,"cache-details-links.json")),this.cacheWatcherCode!==void 0&&this.cacheWatcherCode.start(d.Uri.joinPath(this.detailsFolderUri,"cache-code-links.json")))}setCacheCode(e){this.cacheCode=e||[]}setCacheDetails(e){this.cacheDetails=e||[]}dispose(){this.detailsFileWatcher&&(this.detailsFileWatcher.dispose(),this.detailsFileWatcher=void 0),this.cacheWatcherCode&&(this.cacheWatcherCode.dispose(),this.cacheWatcherCode=void 0),this.cacheWatcherDetails&&(this.cacheWatcherDetails.dispose(),this.cacheWatcherDetails=void 0),this.isStarted=void 0}async getDetailsRoot(e){if(await this.start())return this.detailsRootUri}async getDetailsFolder(e){if(await this.start())return this.detailsFolderUri}async getDetailsPath(e,t){let i=await this.getDetailsFolder(t);if(i===void 0)return;let s=e.indexOf(".md");return s>0&&(e=e.substring(0,s+3)),d.Uri.joinPath(i,e)}async getCodePath(e,t){if(await this.getDetailsRoot(t)===void 0)return;let s=e.match(/([-._A-Z0-9/\\]+)(\.cs)|(\.ts)|(\.html)/i);if(s===null)return;let a=await d.workspace.findFiles(new d.RelativePattern(this.workspaceFolderUri,"**/"+s[0]));if(a.length>0)return a[0]}};var y=h(require("vscode"));var S=class{constructor(e,t){this.commentPrefix=e;this.state=t;e=`(${e}[ 	]*)`;let i=e+"\xA7[ 	]*([^#\xA7\\r\\n]+)";this.regexpDetailsGlobal=new RegExp(i,"g"),this.regexpDetailsLocal=new RegExp(i)}provideDocumentLinks(e,t){let i=e.getText();if(t.isCancellationRequested)return;let s=[],a;for(;a=this.regexpDetailsGlobal.exec(i);){let n=e.positionAt(a.index),r=e.positionAt(a.index+a[0].length),v=new y.Range(n,r);if(s.push(new f(v,void 0,a[0],e.fileName)),t.isCancellationRequested)return}return s}async resolveDocumentLink(e,t){if(e.filename===void 0)return;let i=this.state.getWorkspaceStateByFileName(e.filename);if(i===void 0||await i.getDetailsRoot(t)===void 0||!e.tooltip)return;let a=e.tooltip.match(this.regexpDetailsLocal);if(a===null)return;let n=a[2];if(!n||t.isCancellationRequested)return;let r=await i.getDetailsPath(n,t);if(r!==void 0)return e.target=r,console.log("resolveDocumentLink: %s",r.toString()),e}};var u=new F;function B(o){console.log("vscode-brimborium-details-link activate"),o.subscriptions.push(u),o.subscriptions.push(l.workspace.onDidChangeWorkspaceFolders(e=>{e.removed.forEach(t=>{u.removeWorkspaceFolder(t)}),e.added.forEach(t=>{u.addWorkspaceFolder(t)})})),l.workspace.workspaceFolders?.forEach(e=>{u.addWorkspaceFolder(e)}),o.subscriptions.push(l.commands.registerCommand("vscode-brimborium-details-link.showDetails",()=>{j()})),o.subscriptions.push(l.languages.registerDocumentLinkProvider({language:"markdown"},new k(u))),o.subscriptions.push(l.languages.registerDocumentLinkProvider({language:"markdown"},new D(u))),o.subscriptions.push(l.languages.registerDocumentLinkProvider({language:"csharp"},new D(u))),o.subscriptions.push(l.languages.registerDocumentLinkProvider({language:"csharp"},new S("//",u)))}async function j(){l.window.showInformationMessage(JSON.stringify(u))}function O(){}0&&(module.exports={activate,deactivate});