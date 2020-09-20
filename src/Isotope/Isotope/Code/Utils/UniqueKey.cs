using System;

namespace Isotope.Code.Utils
{
    /// <summary>
    /// Class for generating unique string keys.
    /// </summary>
    public class UniqueKey
    {
        /// <summary>
        /// Returns a new unique key.
        /// </summary>
        public static string Get()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                          .TrimEnd('=')
                          .Replace('+', '-')
                          .Replace('/', '_');
        }
    }
}