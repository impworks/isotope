using System;
using System.Linq;

namespace Isotope.Code.Utils
{
    /// <summary>
    /// Helper methods for ordering values.
    /// </summary>
    public static class OrderHelper
    {
        /// <summary>
        /// Returns an option from the list that matches the specified value, or the first option if none do.
        /// </summary>
        public static string TryGetOneOf(this string value, params string[] options)
        {
            return options.FirstOrDefault(x => x.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                ?? options[0];
        }
    }
}