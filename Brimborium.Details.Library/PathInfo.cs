namespace Brimborium.Details;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class PathInfo : IEquatable<PathInfo> {

    private static char Slash = '/';
    private static char Separator = '#';
    private static char[] ArraySeparator = new char[] { '#' };
    public static PathInfo? _Empty;
    public static PathInfo Empty => _Empty ??= new PathInfo(string.Empty, string.Empty, string.Empty);

    public static PathInfo Parse(string logicalPath)
        => Parse(logicalPath.AsStringSlice());
    
    public static PathInfo Parse(StringSlice logicalPath) {
        var (filename, partLine, partContent) = Split(logicalPath);
        return PathInfo.Create(logicalPath, filename, partContent);

        static (StringSlice filename, StringSlice partLine, StringSlice partContent) Split(StringSlice logicalPath) {
            var (filePath,tail) = logicalPath.SplitInto(ArraySeparator);
            if (tail.IsNullOrEmpty()) {
                return (filePath, StringSlice.Empty, StringSlice.Empty);
            }
            var (part1, part2) = logicalPath.SplitInto(ArraySeparator);
            if (!part1.IsNullOrEmpty() && !part2.IsNullOrEmpty()) {
                return (filePath, part1, part2);
            }

            bool matchNumber = true;
            {
                var spanPart1 = part1.AsSpan();
                int idx = 0;
                for (; idx < spanPart1.Length; idx++) {
                    if (char.IsWhiteSpace(spanPart1[idx])) { continue; }
                    break;
                }
                for (; idx < spanPart1.Length; idx++) {
                    if (char.IsAsciiDigit(spanPart1[idx])) { continue; }
                }
                for (; idx < spanPart1.Length; idx++) {
                    if (char.IsWhiteSpace(spanPart1[idx])) { continue; }
                }
                matchNumber = (idx + 1 == spanPart1.Length);
            }

            if (matchNumber) {
                return (filePath, part1, StringSlice.Empty);
            } else {
                return (filePath, StringSlice.Empty, part1);
            }
        }
    }

    public static PathInfo Create(string filename, string contentPath) {
        return Create(StringSlice.Empty, filename.AsStringSlice(), contentPath.AsStringSlice());
    }

    public static PathInfo Create(StringSlice logicalPath, StringSlice filename, StringSlice contentPath) {
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
            logicalPath = StringSlice.Empty;
        }

        if (logicalPath.IsNullOrEmpty()) {
            if ((spanFilename.Length == 0) && (spanContentPath.Length == 0)) {
                logicalPath = StringSlice.Empty;
            } else {
                char[] buffer = new char[spanFilename.Length + 1 + spanContentPath.Length];
                spanFilename.CopyTo(buffer.AsSpan());
                buffer[spanFilename.Length] = Separator;
                spanContentPath.CopyTo(buffer.AsSpan(spanFilename.Length + 1));
                logicalPath = new StringSlice(new string(buffer));
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
