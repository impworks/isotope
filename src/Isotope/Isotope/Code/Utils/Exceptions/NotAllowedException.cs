using System;

namespace Isotope.Code.Utils
{
    /// <summary>
    /// Exception for indicating that the user has insufficient permissions.
    /// </summary>
    public class NotAllowedException: Exception
    {
        public NotAllowedException(string message)
            : base(message)
        {
        }
    }
}