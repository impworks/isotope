using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Isotope.Code.Utils.Helpers;

/// <summary>
/// Helper class for working with folder paths.
/// </summary>
public static partial class PathHelper
{
    /// <summary>
    /// Combines parts of a folder path.
    /// </summary>
    public static string Combine(params string[] paths)
    {
        var usefulParts = paths.Select(x => x?.Trim('/')).Where(x => !string.IsNullOrEmpty(x));
        var joined = '/' + string.Join('/', usefulParts);
        return joined.ToLower();
    }

    /// <summary>
    /// Returns the canonical version of the path.
    /// </summary>
    public static string Normalize(string path)
    {
        return '/' + path.Trim('/').ToLower();
    }

    /// <summary>
    /// Returns the prefix that all descendants of the folder share (always ends with a slash).
    /// Use with StartsWith() to avoid matching sibling folders that share a prefix (e.g. "/a" vs "/ab").
    /// </summary>
    public static string GetDescendantPrefix(string path)
    {
        return path.EndsWith('/') ? path : path + '/';
    }

    /// <summary>
    /// Checks if the path is equal to the root path or is nested inside it.
    /// </summary>
    public static bool IsSameOrDescendant(string path, string root)
    {
        return path == root || path.StartsWith(GetDescendantPrefix(root));
    }

    /// <summary>
    /// Returns the path relative to the root folder (with a leading slash).
    /// </summary>
    public static string GetRelativePath(string path, string root)
    {
        if (root == "/" || !IsSameOrDescendant(path, root))
            return path;

        var rel = path.Substring(root.Length);
        return rel.Length == 0 ? "/" : rel;
    }

    /// <summary>
    /// Returns the path of the immediately containing folder.
    /// </summary>
    public static string GetParentPath(string path)
    {
        return Normalize(GetPathRegex().Replace(path.TrimEnd('/'), ""));
    }

    /// <summary>
    /// Returns all parent paths from the top to current folder.
    /// </summary>
    public static IReadOnlyList<string> GetParentPaths(string folder)
    {
        var result = new List<string>();
        var tmp = new StringBuilder(folder.Length);
        var parts = folder.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);

        foreach (var part in parts)
        {
            tmp.Append('/');
            tmp.Append(part);
            result.Add(tmp.ToString());
        }

        return result;
    }

    [GeneratedRegex(@"\/[^\/]+$")]
    private static partial Regex GetPathRegex();
}