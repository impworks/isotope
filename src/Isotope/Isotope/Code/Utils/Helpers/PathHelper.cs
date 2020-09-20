using System.Linq;

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
    }
}