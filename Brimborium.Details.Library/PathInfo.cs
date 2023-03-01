namespace Brimborium.Details;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class PathInfo : IEquatable<PathInfo> {

    private static char Slash = '/';
    private static char Separator = '#';
    public static PathInfo? _Empty;
    public static PathInfo Empty => _Empty ??= new PathInfo(string.Empty, string.Empty, string.Empty);

    public static PathInfo Parse(string logicalPath) {
        var (filename, partContent) = Split(logicalPath);
        return PathInfo.Create(logicalPath.AsSubString(), filename, partContent);

        static (SubString filename, SubString partContent) Split(string logicalPath) {
            var pos = logicalPath.IndexOf(Separator);
            if (pos < 0) {
                return (logicalPath.AsSubString(), SubString.Empty);
            } else {
                return (logicalPath.AsSubString(0, pos), logicalPath.AsSubString(pos + 1));
            }
        }
    }

    public static PathInfo Create(string filename, string contentPath) {
        return Create(SubString.Empty, filename.AsSubString(), contentPath.AsSubString());
    }

    public static PathInfo Create(SubString logicalPath, SubString filename, SubString contentPath) {
        var spanFilename = filename.AsSpan();
        var spanContentPath = contentPath.AsSpan();
        if (spanFilename.Contains('\\')) {
            throw new ArgumentException("filename contains \\");
        }
        if (spanContentPath.Contains('\\')) {
            throw new ArgumentException("contentPath contains \\");
        }
        if ((spanContentPath.Length > 0) && (spanContentPath[0] == Separator)) {
            spanContentPath = spanContentPath.Slice(1);
        }
        if ((spanContentPath.Length > 0) && (spanContentPath[0] != Slash)) {
            char[] buffer = new char[spanContentPath.Length + 1];
            buffer[0] = Slash;
            spanContentPath.CopyTo(buffer.AsSpan(1));
            spanContentPath = buffer.AsSpan();
            logicalPath = SubString.Empty;
        }

        if (logicalPath.IsNullOrEmpty()) {
            if ((spanFilename.Length == 0) && (spanContentPath.Length == 0)) {
                logicalPath = SubString.Empty;
            } else {
                char[] buffer = new char[spanFilename.Length + 1 + spanContentPath.Length];
                spanFilename.CopyTo(buffer.AsSpan());
                buffer[spanFilename.Length] = Separator;
                spanContentPath.CopyTo(buffer.AsSpan(spanFilename.Length + 1));
                logicalPath = new SubString(new string(buffer));
            }
        } else {
            if (contentPath.Length == 0) {
            } else if (logicalPath.Length != (filename.Length + 1 + contentPath.Length)) {
                throw new ArgumentException("logicalPath.Length!= (filename.Length + partContent.Length)");
            }
        }
        string logicalPathValue = logicalPath.ToString();
        string filenameValue = spanFilename.ToString();
        string contentPathValue = spanContentPath.ToString();
        return new PathInfo(
            logicalPathValue, filenameValue, contentPathValue, 
            GetContentLevel(contentPathValue), GetContentPathNormalized(contentPathValue)
            );
    }

    public static int GetContentLevel(string contentPath) {
        if (contentPath.Length == 0) {
            return 0;
        }
        int cntSlash = 0;
        for (int idx = contentPath.Length - 1; idx >= 0; idx--) {
            if (contentPath[idx] == '/') { cntSlash++; }
        }
        return cntSlash;
    }

    public static string GetContentPathNormalized(string contentPath) {
        var sb = new StringBuilder();
        sb.Append(contentPath);
        sb.Replace(" /", "/");
        sb.Replace("/ ", "/");
        sb.Replace(' ', '-');
        sb.Replace('_', '-');
        return sb.ToString();
    }

    public PathInfo(
        string logicalPath,
        string filename,
        string contentPath)
        : this(logicalPath, filename, contentPath, 
              GetContentLevel(contentPath), 
              GetContentPathNormalized(contentPath)) {
    }

    private PathInfo(
        string logicalPath,
        string filename,
        string contentPath,
        int contentLevel,
        string contentPathNormalized) {
        this.LogicalPath = logicalPath;
        this.FilePath = filename;
        this.ContentPath = contentPath;
        this.ContentLevel = contentLevel;
        this.ContentPathNormalized = contentPathNormalized;
        var spanFilename = filename.AsSpan();
        var spanContentPath = contentPath.AsSpan();
        if (spanFilename.Contains('\\')) {
            throw new ArgumentException("filename contains \\");
        }
        if (spanContentPath.Contains('\\')) {
            throw new ArgumentException("contentPath contains \\");
        }
        if (contentPath.Length == 0) {
        } else if (logicalPath.Length != (filename.Length + 1 + contentPath.Length)) {
            throw new ArgumentException("logicalPath.Length!= (filename.Length + partContent.Length)");
        }
    }

    /// <summary>
    /// The complete path
    /// </summary>
    public string LogicalPath { get; }
    public string FilePath { get; }

    /// <summary>
    /// the content path is the part after the filepath.
    /// It starts with a '/' except for the empty path.
    /// </summary>
    [JsonIgnore]
    public string ContentPath { get; }

    [JsonIgnore]
    public string ContentPathNormalized { get; }

    [JsonIgnore]
    public int ContentLevel { get; }

    public bool IsEmpty() => string.IsNullOrEmpty(this.LogicalPath);

    public override bool Equals(object? obj) {
        return base.Equals(obj as PathInfo);
    }

    public override int GetHashCode() {
        return HashCode.Combine(
                StringComparer.OrdinalIgnoreCase.GetHashCode(this.FilePath),
                StringComparer.OrdinalIgnoreCase.GetHashCode(this.ContentPathNormalized)
            );
    }

    public bool Equals(PathInfo? other) {
        if (other is null) { return false; }
        if (ReferenceEquals(this, other)) { return true; }
        return string.Equals(this.FilePath, other.FilePath, StringComparison.OrdinalIgnoreCase)
            && string.Equals(this.ContentPathNormalized, other.ContentPathNormalized, StringComparison.OrdinalIgnoreCase);
    }

    public bool IsContentPathEqual(PathInfo? other) {
        if (other is null) { return false; }
        if (ReferenceEquals(this, other)) { return true; }
        if (this.ContentLevel == other.ContentLevel) {
            return string.Equals(this.ContentPathNormalized, other.ContentPathNormalized, StringComparison.OrdinalIgnoreCase);
        }
        return false;
    }

    public bool IsContentPathParent(PathInfo? maybeParent) {
        if (maybeParent is null) { return false; }
        if (ReferenceEquals(this, maybeParent)) { return false; }
        if (this.ContentLevel == maybeParent.ContentLevel + 1) {
            if (this.ContentPathNormalized.StartsWith(maybeParent.ContentPathNormalized, StringComparison.OrdinalIgnoreCase)) {
                if (this.ContentPathNormalized.Length > maybeParent.ContentPathNormalized.Length
                    || this.ContentPathNormalized[maybeParent.ContentPathNormalized.Length] == '/') {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// This function checks if a given path is a child of the current path
    /// </summary>
    /// <param name="path">a child path</param>
    /// <returns>true if it is a child path</returns>
    public bool StartsWith(PathInfo path) {
        if (!string.Equals(this.FilePath, path.FilePath, StringComparison.OrdinalIgnoreCase)) {
            return false;
        }
        if (path.ContentPath.Length <= this.ContentPath.Length) {
            return false;
        }
        if (path.ContentPath[this.ContentPath.Length] != '/') {
            return false;
        }
        return this.ContentPath.StartsWith(path.ContentPath, StringComparison.OrdinalIgnoreCase);
    }

    public override string ToString() {
        return this.LogicalPath;
    }

    private string GetDebuggerDisplay() {
        return this.LogicalPath;
    }
}
