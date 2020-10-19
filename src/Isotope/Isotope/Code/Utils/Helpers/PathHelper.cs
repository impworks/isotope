using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Isotope.Code.Utils.Helpers
{
    /// <summary>
    /// Helper class for working with folder paths.
    /// </summary>
    public static class PathHelper
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
        /// Returns the path of the immediately containing folder.
        /// </summary>
        public static string GetParentPath(string path)
        {
            return Normalize(Regex.Replace(path.TrimEnd('/'), @"\/[^\/]+$", ""));
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
    }
}