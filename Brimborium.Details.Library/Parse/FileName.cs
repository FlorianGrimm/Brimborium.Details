﻿namespace Brimborium.Details.Parse;

public class FileName : IEquatable<FileName> {
    private static FileName? _Empty;
    public static FileName Empty => _Empty ??= new FileName();

    private string? _RelativePath;
    private FileName? _RootFolder;
    private string? _AbsolutePath;

    public FileName() {
    }
    
    public static FileName FromAbsolutePath(string absolutePath) {
        return new FileName() {
            AbsolutePath = absolutePath
        };
    }

    public FileName Create(string path) {
        if (Path.IsPathFullyQualified(path)) {
            return this.CreateWithAbsolutePath(path);
        } else {
            return this.CreateWithRelativePath(path);
        }
    }

    public FileName CreateWithRelativePath(string relativePath) {
        return new FileName() {
            RootFolder = this,
            RelativePath = relativePath.Replace('\\', '/')
        };
    }

    public FileName CreateWithAbsolutePath(string absolutePath) {
        return new FileName() {
            RootFolder = this,
            AbsolutePath = absolutePath
        };
    }

    [JsonInclude]
    public string? RelativePath {
        get {
            if (this._RelativePath is null && this._AbsolutePath is not null && this._RootFolder?.AbsolutePath is not null) {
                var relativePathOS = Path.GetRelativePath(this._RootFolder.AbsolutePath, this._AbsolutePath);
                if (Path.DirectorySeparatorChar == '\\') {
                    this._RelativePath = relativePathOS.Replace('\\', '/');
                } else {
                    this._RelativePath = relativePathOS;
                }
            }
            return this._RelativePath;
        }

        set {
            if (value is null) {
                this._RelativePath = null;
            } else {
                if (Path.DirectorySeparatorChar == '\\') {
                    this._RelativePath = value.Replace('\\', '/');
                } else {
                    this._RelativePath = value;
                }
                this._AbsolutePath = null;
            }
        }
    }

    [JsonIgnore]
    public FileName? RootFolder {
        get {
            return this._RootFolder;
        }

        set {
            if (value is null && this._RootFolder is not null) {
                throw new InvalidOperationException("RootFolder can only set once");
            } else if (this._RootFolder is not null) {
                throw new InvalidOperationException("RootFolder can only set once");
            } else {
                this._RootFolder = value;
            }
        }
    }

    [JsonIgnore]
    public string? AbsolutePath {
        get {
            if (this._AbsolutePath is null && this._RelativePath is not null && this._RootFolder?.AbsolutePath is not null) {
                string relativePathOS;
                if (Path.DirectorySeparatorChar == '\\') {
                    relativePathOS = this._RelativePath.Replace('/', '\\');
                } else {
                    relativePathOS = this._RelativePath;
                }
                var absolutePath = Path.Combine(this._RootFolder.AbsolutePath, relativePathOS);
                if (absolutePath.Contains("..")) {
                    absolutePath = Path.GetFullPath(absolutePath);
                }
                this._AbsolutePath = absolutePath;
            }
            return _AbsolutePath;
        }

        set {
            this._AbsolutePath = value;
        }
    }

    public override string ToString() {
        if (this._RelativePath is not null) {
            return this._RelativePath;
        }
        if (this._AbsolutePath is not null) {
            return this._AbsolutePath;
        }
        return "";
    }

    public static bool operator ==(FileName? that, FileName? other) {
        if (ReferenceEquals(that, other)) { return true; }
        if (that is null) { return false; }
        if (other is null) { return false; }
        return that.Equals(other);
    }

    public static bool operator !=(FileName? that, FileName? other) {
        if (ReferenceEquals(that, other)) { return false; }
        if (that is null) { return true; }
        if (other is null) { return true; }
        return !that.Equals(other);
    }

    public override bool Equals(object? obj) {
        if (obj is null) { return false; }
        return this.Equals(obj as FileName);
    }

    public bool Equals(FileName? other) {
        if (other is null) { return false; }
        if (ReferenceEquals(this, other)) { return true; }
        if (this._RelativePath is not null && other._RelativePath is not null) {
            if (this._RootFolder is null) {
                if (other._RootFolder is null) {
                    // maybe
                } else {
                    return false;
                }
            } else {
                if (other._RootFolder is null) {
                    return false;
                } else {
                    if (!this._RootFolder.Equals(other._RootFolder)) {
                        return false;
                    }
                }
            }
            return StringComparer.InvariantCultureIgnoreCase.Equals(this._RelativePath, other._RelativePath);
        }
        return StringComparer.InvariantCultureIgnoreCase.Equals(this._AbsolutePath, other._AbsolutePath);
    }

    public override int GetHashCode() {
        return (this._RelativePath ?? this._AbsolutePath ?? string.Empty)
            .GetHashCode();
    }

    // Rebase changes the root folder of the FileName
    // if the root folder is the same, it returns the same instance
    // if the root folder is different it returns a new instance with the new root folder
    // if there is no root folder, it returns null
    public FileName? Rebase(FileName nextbase) {
        if (ReferenceEquals(this._RootFolder, nextbase)) {
            return this;
        }
        if (this._RootFolder?.AbsolutePath is not null
            && nextbase.RootFolder?.AbsolutePath is not null
            && string.Equals(this._RootFolder.AbsolutePath, nextbase.AbsolutePath, StringComparison.InvariantCultureIgnoreCase)
            ) {
            return this;
        }
        var thisAbsolutePath = this.AbsolutePath;
        if (thisAbsolutePath is null) { return null; }

        var otherAbsolutePath = nextbase.AbsolutePath;
        if (otherAbsolutePath is null) { return null; }

        return new FileName() { RootFolder = nextbase, AbsolutePath = this.AbsolutePath };
    }

    public FileName? GetParentDirectory() {
        var thisAbsolutePath = this.AbsolutePath;
        if (thisAbsolutePath is null) { return null; }
        var directoryName = Path.GetDirectoryName(thisAbsolutePath);
        if (directoryName is null) { return null; }
        return new FileName() { RootFolder = this._RootFolder, AbsolutePath = directoryName };
    }
}
