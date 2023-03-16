namespace Brimborium.Details.Parse;

[System.Text.Json.Serialization.JsonDerivedType(typeof(PathData), "PathData")]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class PathData : IEquatable<PathData> {

    private static char Slash = '/';
    private static char Separator = '#';
    private static char[] ArraySeparator = new char[] { '#' };
    public static PathData? _Empty;
    public static PathData Empty => _Empty ??= new PathData(string.Empty, string.Empty, 0, string.Empty);

    public static PathData Parse(string logicalPath)
        => Parse(logicalPath.AsStringSlice());

    public static PathData Parse(StringSlice logicalPath) {
        var (filename, partLine, partContent) = Split(logicalPath);
        if (!(!partLine.IsNullOrEmpty() && int.TryParse(partLine.AsSpan(), out var line))) {
            line = 0;
        }
        return Create(logicalPath, filename, line, partContent);

        static (StringSlice filename, StringSlice partLine, StringSlice partContent) Split(StringSlice logicalPath) {
            var (filePath, tail) = logicalPath.SplitInto(ArraySeparator);
            if (tail.IsNullOrEmpty()) {
                return (filePath, StringSlice.Empty, StringSlice.Empty);
            }
            var (part1, part2) = tail.SplitInto(ArraySeparator);

            part1 = part1.Trim();
            part2 = part2.Trim();

            if (!part2.IsNullOrEmpty()) {
                return (filePath, part1, part2);
            }
            // part 2 is empty
            if (part1.IsNullOrEmpty()) {
                return (filePath, StringSlice.Empty, StringSlice.Empty);
            }

            // part1 contains only digits?
            if (part1.TrimWhile((value, _) => char.IsDigit(value) ? 0 : +1).IsNullOrEmpty()) {
                return (filePath, part1, StringSlice.Empty);
            } else {
                return (filePath, StringSlice.Empty, part1);
            }
        }
    }

    public static PathData Create(string filename, string contentPath) {
        return Create(StringSlice.Empty, filename.AsStringSlice(), 0, contentPath.AsStringSlice());
    }

    public static PathData Create(string filename, int line, string contentPath) {
        return Create(StringSlice.Empty, filename.AsStringSlice(), line, contentPath.AsStringSlice());
    }

    public static PathData Create(StringSlice logicalPath, StringSlice filename, int line, StringSlice contentPath) {
        if (filename.Contains('\\')) {
            throw new ArgumentException("filename contains \\");
        }
        if (contentPath.Contains('\\')) {
            throw new ArgumentException("contentPath contains \\");
        }
        if (contentPath.Length > 0 && contentPath[0] == Separator) {
            contentPath = contentPath.Substring(1);
        }
        if (contentPath.Length > 0 && contentPath[0] != Slash) {
            var buffer = new char[contentPath.Length + 1];
            buffer[0] = Slash;
            contentPath.AsSpan().CopyTo(buffer.AsSpan(1));
            contentPath = new StringSlice(new string(buffer));
            logicalPath = StringSlice.Empty;
        }
        string logicalPathValue; //= logicalPath.ToString();

        //if (logicalPath.IsNullOrEmpty()) {
        if (filename.Length == 0 && line == 0 && contentPath.Length == 0) {
            return PathData.Empty;
        } else {
            var sb = StringBuilderPool.GetStringBuilder();
            sb.Append(filename);
            sb.Append(Separator);
            if (line > 0) {
                sb.Append(line);
            }
            sb.Append(Separator);
            sb.Append(contentPath);
            logicalPathValue = sb.ToString();
            StringBuilderPool.ReturnStringBuilder(sb);
        }
        //} else {
        // TODO: this does not respect line
        //if (contentPath.Length == 0) {
        //} else if (logicalPath.Length != filename.Length + 1 + contentPath.Length) {
        //    throw new ArgumentException("logicalPath.Length!= (filename.Length + partContent.Length)");
        //}
        //}
        
        var filenameValue = filename.ToString();
        var contentPathValue = contentPath.ToString();
        return new PathData(
            logicalPathValue, filenameValue, line, contentPathValue,
            GetContentLevel(contentPathValue), GetContentPathNormalized(contentPathValue)
            );
    }

    public static int GetContentLevel(string contentPath) {
        if (contentPath.Length == 0) {
            return 0;
        }
        var cntSlash = 0;
        for (var idx = contentPath.Length - 1; idx >= 0; idx--) {
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

    public PathData(
        string logicalPath,
        string filename,
        int line,
        string contentPath)
        : this(logicalPath, filename, line, contentPath,
              GetContentLevel(contentPath),
              GetContentPathNormalized(contentPath)) {
    }

    private PathData(
        string logicalPath,
        string filename,
        int line,
        string contentPath,
        int contentLevel,
        string contentPathNormalized) {
        this.LogicalPath = logicalPath;
        this.FilePath = filename;
        this.Line = line;
        this.ContentPath = contentPath;
        this.ContentLevel = contentLevel;
        this.ContentPathNormalized = contentPathNormalized;
        if (filename.Contains('\\')) {
            throw new ArgumentException("filename contains \\");
        }
        if (contentPath.Contains('\\')) {
            throw new ArgumentException("contentPath contains \\");
        }
        // TODO:this does not respect line
        //if (contentPath.Length == 0) {
        //} else if (logicalPath.Length != filename.Length + 1 + contentPath.Length) {
        //    throw new ArgumentException("logicalPath.Length!= (filename.Length + partContent.Length)");
        //}
    }

    /// <summary>
    /// The complete path
    /// </summary>
    public string LogicalPath { get; }

    [JsonIgnore]
    public string FilePath { get; }

    [JsonIgnore]
    public int Line { get; }

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
        return base.Equals(obj as PathData);
    }

    public override int GetHashCode() {
        return HashCode.Combine(
                StringComparer.OrdinalIgnoreCase.GetHashCode(this.FilePath),
                StringComparer.OrdinalIgnoreCase.GetHashCode(this.ContentPathNormalized)
            );
    }

    public bool Equals(PathData? other) {
        if (other is null) { return false; }
        if (ReferenceEquals(this, other)) { return true; }
        return string.Equals(this.FilePath, other.FilePath, StringComparison.OrdinalIgnoreCase)
            && string.Equals(this.ContentPathNormalized, other.ContentPathNormalized, StringComparison.OrdinalIgnoreCase);
    }

    public bool IsContentPathEqual(PathData? other) {
        if (other is null) { return false; }
        if (ReferenceEquals(this, other)) { return true; }
        if (this.ContentLevel == other.ContentLevel) {
            return string.Equals(this.ContentPathNormalized, other.ContentPathNormalized, StringComparison.OrdinalIgnoreCase);
        }
        return false;
    }

    public bool IsContentPathParent(PathData? maybeParent) {
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
    public bool StartsWith(PathData path) {
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

    public PathData WithFilePath(string filePath) => PathData.Create(filePath, this.Line, this.ContentPathNormalized);
    public PathData WithLine(int line) => PathData.Create(this.FilePath, line, this.ContentPathNormalized);
    public PathData WithContentPath(string contentPath) => PathData.Create(this.FilePath, this.Line, contentPath);

    public override string ToString() => this.LogicalPath;

    private string GetDebuggerDisplay() => this.LogicalPath;

    
}
