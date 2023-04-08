import * as vscode from "vscode";

async function exists(uri: vscode.Uri, fileType: vscode.FileType): Promise<boolean> {
    try {
        const stat = await vscode.workspace.fs.stat(uri);
        return stat.type === fileType;
    } catch (error) {
        return false;
    }
}
function parentUri(uri: vscode.Uri): vscode.Uri {
    const path=uri.toString();
    const lastSlash = path.lastIndexOf("/");
    if (lastSlash > 0) {
        return vscode.Uri.file(path.substring(0, lastSlash));
    } else {
        return uri;
    }
    //return vscode.Uri.joinPath(uri, "..");
}

export const fileUtilities = {
    exists,
    parentUri
};